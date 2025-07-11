﻿using System;
using System.Collections.Generic;
namespace LibraryManagement.Web.ViewModels.Book
{
    public class BookDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string AuthorName { get; set; } = null!;

        public string Genre { get; set; } = null!;

        public DateTime PublishedDate { get; set; }

        public string? ImageUrl { get; set; }

        public string CreatorId { get; set; } = null!;

        public bool IsApprovedMember { get; set; }

        public bool HasBorrowedThisBook { get; set; }
    }
}
