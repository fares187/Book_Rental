using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class CreateCategoriesViewModel
    {
        public int id { get; set; } 
        [MaxLength(250)]
        public string Name { get; set; } = null!;
    }
}
