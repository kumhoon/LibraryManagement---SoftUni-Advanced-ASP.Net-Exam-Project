namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.BorrowingRecord;

    public class BorrowingRecordService : IBorrowingRecordService
    {
        private readonly IBorrowingRecordRepository _borrowingRecordRepository;
        public BorrowingRecordService(IBorrowingRecordRepository borrowingRecordRepository)
        {
            _borrowingRecordRepository = borrowingRecordRepository;
        }
        public async Task<BorrowResult> BorrowBookAsync(Guid memberId, Guid bookId)
        {
            
            var alreadyBorrowed = await _borrowingRecordRepository.HasActiveBorrowAsync(memberId, bookId);
            if (alreadyBorrowed != null)
            {
                return BorrowResult.AlreadyBorrowedByMember;
            }

            var isBorrowedByOther = await _borrowingRecordRepository.IsBookBorrowedAsync(bookId);
            if (isBorrowedByOther)
            {
                return BorrowResult.BookUnavailable;
            }

            var record = new BorrowingRecord
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                BookId = bookId,
                BorrowDate = DateTime.UtcNow
            };

            await _borrowingRecordRepository.AddAsync(record);
            await _borrowingRecordRepository.SaveChangesAsync();

            return BorrowResult.Success;
        }

        public async Task<IEnumerable<BorrowingRecordViewModel>> GetBorrowingHistoryAsync(Guid memberId)
        {
            var records = await _borrowingRecordRepository
                .GetByMemberIdAsync(memberId);

            return records
                .Select(r => new BorrowingRecordViewModel
            {
                Title = r.Book?.Title ?? "Unknown Title",
                BorrowDate = r.BorrowDate,
                ReturnDate = r.ReturnDate
            });
        }

        public async Task<bool> IsBookBorrowedAsync(Guid bookId)
        {
            return await _borrowingRecordRepository.IsBookBorrowedAsync(bookId);
        }

        public async Task<bool> ReturnBookAsync(Guid memberId, Guid bookId)
        {
            var record = await this._borrowingRecordRepository
                .HasActiveBorrowAsync(memberId, bookId);

            if (record == null) 
            { 
                return false;
            }

            record.ReturnDate = DateTime.UtcNow;

            return await _borrowingRecordRepository
                .UpdateAsync(record);
        }

        public async Task<bool> HasAnyActiveBorrowAsync(Guid memberId)
        {
            return await _borrowingRecordRepository.HasAnyActiveBorrowAsync(memberId);
        }

        public async Task<bool> IsBookBorrowedByMemberAsync(Guid memberId, Guid bookId)
        {
            var record = await _borrowingRecordRepository.HasActiveBorrowAsync(memberId, bookId);
            return record != null;
        }
    }

}
