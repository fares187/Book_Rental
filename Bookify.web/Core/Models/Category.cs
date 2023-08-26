using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
    [Index(nameof(Name),IsUnique =true)]   
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow; 
        public DateTime? LastUpdatedOn { get; set; } 
        public ICollection<BookCategory> Books = new List<BookCategory>();

        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? LastUpdatedById { get; set; }
        public ApplicationUser? LastUpdatedBy { get; set; }
    }
}
