using Bookify.web.Core.Models;
using Bookify.web.Core.Utilites;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class BooksReportViewModel
    {
        [Display(Name = "Authors")]
        public List<int>? SelectedAuthors { get; set; }  = new List<int>();  
        public IEnumerable<SelectListItem> Author { get; set; }=new List<SelectListItem>();

        [Display(Name = "Categories")]
        public List<int>? SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem> Categories { get; set; }=new List<SelectListItem>(); 
        public PaginatedList<Book> Data { get; set; }  
    }
}
