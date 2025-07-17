using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Web.ViewModels.Author
{
    public class AuthorWithBooksViewModel
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> Books { get; set; } = new List<string>();
    }
}
