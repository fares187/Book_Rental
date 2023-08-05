using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedOn { get; set; }
    }
}
 