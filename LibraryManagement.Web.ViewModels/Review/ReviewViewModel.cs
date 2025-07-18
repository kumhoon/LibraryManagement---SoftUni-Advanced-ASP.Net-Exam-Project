using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Web.ViewModels.Review
{
    public class ReviewViewModel
    {
        public Guid ReviewId { get; set; }

        public Guid BookId { get; set; }
        public int Rating { get; set; } 

        public string? Content { get; set; }

    }
}
