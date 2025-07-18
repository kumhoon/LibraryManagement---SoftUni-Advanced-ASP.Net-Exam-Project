using LibraryManagement.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Web.ViewModels.Review
{
    public class BookReviewsViewModel
    {
        public Guid BookId { get; set; }
        public double AverageRating { get; set; }        
        public int TotalReviews { get; set; }          
        public PagedResult<ReviewDisplayViewModel> Reviews { get; set; } = new ();
    }
}
