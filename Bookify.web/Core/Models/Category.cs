using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow; 
        public DateTime? LastUpdatedOn { get; set; }
        public Category() { 
        this.CreatedOn = DateTime.Now;
        }   
    }
}
