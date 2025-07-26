namespace LibraryManagement.Services.Core.Tests.Services.BorrowingRecordTests
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using Moq;

    [TestFixture]
    public class BorrowingRecordTests
    {
        private BorrowingRecordService _borrowingRecordService;
        private Mock<IBorrowingRecordRepository> _borrowingRecordRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _borrowingRecordRepositoryMock = new Mock<IBorrowingRecordRepository>();
            _borrowingRecordService = new BorrowingRecordService(_borrowingRecordRepositoryMock.Object);
        }

        [Test]
        public async Task BorrowBookAsync_AlreadyBorrowedByMember_ReturnsAlreadyBorrowedByMember()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.HasActiveBorrowAsync(memberId, bookId))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.BorrowBookAsync(memberId, bookId);

            Assert.That(result, Is.EqualTo(BorrowResult.AlreadyBorrowedByMember));
            _borrowingRecordRepositoryMock.Verify(r => r.IsBookBorrowedAsync(It.IsAny<Guid>()), Times.Never);
            _borrowingRecordRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BorrowingRecord>()), Times.Never);
        }

        [Test]
        public async Task BorrowBookAsync_BookBorrowedByAnotherMember_ReturnsBookUnavailable()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.HasActiveBorrowAsync(memberId, bookId))
                .ReturnsAsync(false);

            _borrowingRecordRepositoryMock
                .Setup(r => r.IsBookBorrowedAsync(bookId))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.BorrowBookAsync(memberId, bookId);

            Assert.That(result, Is.EqualTo(BorrowResult.BookUnavailable));
            _borrowingRecordRepositoryMock.Verify(r => r.AddAsync(It.IsAny<BorrowingRecord>()), Times.Never);
        }

        [Test]
        public async Task BorrowBookAsync_BookAvailable_AddsRecordAndReturnsSuccess()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.HasActiveBorrowAsync(memberId, bookId))
                .ReturnsAsync(false);

            _borrowingRecordRepositoryMock
                .Setup(r => r.IsBookBorrowedAsync(bookId))
                .ReturnsAsync(false);


            var result = await _borrowingRecordService.BorrowBookAsync(memberId, bookId);


            Assert.That(result, Is.EqualTo(BorrowResult.Success));
            _borrowingRecordRepositoryMock.Verify(r => r.AddAsync(It.Is<BorrowingRecord>(r =>
                r.BookId == bookId &&
                r.MemberId == memberId
            )), Times.Once);
            _borrowingRecordRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetBorrowingHistoryAsync_WithRecords_ReturnsCorrectMappedViewModels()
        {

            var memberId = Guid.NewGuid();
            var records = new List<BorrowingRecord>
            {
                new BorrowingRecord
                {
                    BorrowDate = DateTime.UtcNow.AddDays(-5),
                    ReturnDate = DateTime.UtcNow,
                    Book = new Book { Title = "Book A" }
                },
                new BorrowingRecord
                {
                    BorrowDate = DateTime.UtcNow.AddDays(-10),
                    ReturnDate = null,
                    Book = new Book { Title = "Book B" }
                }
            };

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetByMemberIdAsync(memberId))
                .ReturnsAsync(records);

            var result = (await _borrowingRecordService.GetBorrowingHistoryAsync(memberId)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("Book A"));
            Assert.That(result[0].ReturnDate, Is.Not.Null);
            Assert.That(result[1].Title, Is.EqualTo("Book B"));
            Assert.That(result[1].ReturnDate, Is.Null);
        }

        [Test]
        public async Task GetBorrowingHistoryAsync_WithNullBookTitle_UsesFallbackValue()
        {
            
            var memberId = Guid.NewGuid();
            var records = new List<BorrowingRecord>
            {
                new BorrowingRecord
                {
                    BorrowDate = DateTime.UtcNow,
                    ReturnDate = null,
                    Book = null 
                }
            };

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetByMemberIdAsync(memberId))
                .ReturnsAsync(records);

            var result = (await _borrowingRecordService.GetBorrowingHistoryAsync(memberId)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Unknown Title")); 
        }

        [Test]
        public async Task GetBorrowingHistoryAsync_NoRecords_ReturnsEmptyList()
        {

            var memberId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetByMemberIdAsync(memberId))
                .ReturnsAsync(new List<BorrowingRecord>());

            var result = await _borrowingRecordService.GetBorrowingHistoryAsync(memberId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task ReturnBookAsync_WithValidRecord_ReturnsTrueAndUpdatesReturnDate()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var borrowRecord = new BorrowingRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow.AddDays(-3),
                ReturnDate = null
            };

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetActiveBorrowRecordAsync(memberId, bookId))
                .ReturnsAsync(borrowRecord);

            _borrowingRecordRepositoryMock
                .Setup(r => r.UpdateAsync(borrowRecord))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.ReturnBookAsync(memberId, bookId);

            Assert.That(result, Is.True);
            Assert.That(borrowRecord.ReturnDate, Is.Not.Null);
            Assert.That((DateTime.UtcNow - borrowRecord.ReturnDate.Value).TotalSeconds, Is.LessThan(5));
        }

        [Test]
        public void ReturnBookAsync_WhenRecordNotFound_ThrowsKeyNotFoundException()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetActiveBorrowRecordAsync(memberId, bookId))
                .ReturnsAsync((BorrowingRecord?)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _borrowingRecordService.ReturnBookAsync(memberId, bookId));

            StringAssert.Contains("No active borrowing record found for this book and member.", ex.Message); 
        }

        [Test]
        public void ReturnBookAsync_WhenUpdateFails_ThrowsException()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var borrowRecord = new BorrowingRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow.AddDays(-1),
                ReturnDate = null
            };

            _borrowingRecordRepositoryMock
                .Setup(r => r.GetActiveBorrowRecordAsync(memberId, bookId))
                .ReturnsAsync(borrowRecord);

            _borrowingRecordRepositoryMock
                .Setup(r => r.UpdateAsync(borrowRecord))
                .ReturnsAsync(false); 

            var ex = Assert.ThrowsAsync<Exception>(() =>
                _borrowingRecordService.ReturnBookAsync(memberId, bookId));

            StringAssert.Contains("Failed to update the borrowing record.", ex.Message); 
        }

        [Test]
        public async Task IsBookBorrowedAsync_WhenBookIsBorrowed_ReturnsTrue()
        {

            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.IsBookBorrowedAsync(bookId))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.IsBookBorrowedAsync(bookId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsBookBorrowedAsync_WhenBookIsNotBorrowed_ReturnsFalse()
        {

            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.IsBookBorrowedAsync(bookId))
                .ReturnsAsync(false);

            var result = await _borrowingRecordService.IsBookBorrowedAsync(bookId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task HasAnyActiveBorrowAsync_WhenActiveBorrowExists_ReturnsTrue()
        {

            var memberId = Guid.NewGuid();
            _borrowingRecordRepositoryMock
                .Setup(r => r.HasAnyActiveBorrowAsync(memberId))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.HasAnyActiveBorrowAsync(memberId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task HasAnyActiveBorrowAsync_WhenNoActiveBorrowExists_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();
            _borrowingRecordRepositoryMock
                .Setup(r => r.HasAnyActiveBorrowAsync(memberId))
                .ReturnsAsync(false);

            var result = await _borrowingRecordService.HasAnyActiveBorrowAsync(memberId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsBookBorrowedByMemberAsync_WhenBookIsBorrowedByMember_ReturnsTrue()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.HasActiveBorrowAsync(memberId, bookId))
                .ReturnsAsync(true);

            var result = await _borrowingRecordService.IsBookBorrowedByMemberAsync(memberId, bookId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsBookBorrowedByMemberAsync_WhenBookIsNotBorrowedByMember_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _borrowingRecordRepositoryMock
                .Setup(r => r.HasActiveBorrowAsync(memberId, bookId))
                .ReturnsAsync(false);

            var result = await _borrowingRecordService.IsBookBorrowedByMemberAsync(memberId, bookId);

            Assert.That(result, Is.False);
        }
    }
}
