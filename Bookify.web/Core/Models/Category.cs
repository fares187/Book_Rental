using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; } = DateTime.Now; 
        public DateTime? LastUpdatedOn { get;}
        public Category() { 
        this.CreatedOn = DateTime.Now;
        }   
    }
}
