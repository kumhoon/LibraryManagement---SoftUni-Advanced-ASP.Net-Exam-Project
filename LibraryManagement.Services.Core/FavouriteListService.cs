﻿namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using static LibraryManagement.GCommon.Defaults.Text;
    public class FavouriteListService : IFavouriteListService
    {
        private readonly IFavouriteListRepository _favouriteListRepository;
        private readonly IMembershipRepository _membershipRepository;

        public FavouriteListService(IFavouriteListRepository repository, IMembershipRepository membershipRepository)
        {
            _favouriteListRepository = repository;
            _membershipRepository = membershipRepository;
        }

        public async Task<bool> AddToFavouritesAsync(Guid memberId, Guid bookId)
        {
            return await _favouriteListRepository.AddAsync(memberId, bookId);
        }

        public async Task<bool> RemoveFromFavouritesAsync(Guid memberId, Guid bookId)
        {
            return await _favouriteListRepository.RemoveAsync(memberId, bookId);
        }

        public async Task<bool> IsBookFavouriteAsync(Guid memberId, Guid bookId)
        {
            return await _favouriteListRepository.ExistsAsync(memberId, bookId);
        }

        public async Task<IEnumerable<BookIndexViewModel>> GetFavouriteBooksAsync(Guid memberId)
        {
            var favouriteBooks = await _favouriteListRepository.GetFavouriteBooksAsync(memberId);

            return favouriteBooks.Select(book => new BookIndexViewModel
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author?.Name ?? UnknownAuthor,
                Genre = book.Genre?.Name ?? UnknownGenre,
                PublishedDate = book.PublishedDate,
                ImageUrl = book.ImageUrl
            });
        }
    }
}
