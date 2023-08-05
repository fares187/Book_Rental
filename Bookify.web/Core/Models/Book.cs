using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.web.Core.Models
{
    [Index(nameof(Title),nameof(AuthorId),IsUnique =true)]
    public class Book
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Title { get; set; } = null!;
        
        public int  AuthorId {get; set;}
        public Author? Author { get; set; }
        [MaxLength(200)]
        public string Publisher { get; set; } = null!;
        public DateTime PublishingDaTe { get; set; }    
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        public string? ImagePublicId { get; set; }
        [MaxLength(50)]
        public string Hall { get; set; }=null!;
        public bool IsAvailableForRental { get; set;} 
        public string Description { get; set; } =null!; 
        public bool IsDeleted { get; set; }
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedOn { get; set; }
        
        public ICollection<BookCategory> categories { get; set; } = new List<BookCategory>();  
    }

}
//https://res.cloudinary.com/macto/image/upload/v1690688192/pjaolvdlaf4glhjpowwf.png
//https://res.cloudinary.com/macto/image/upload/c_thumb,w_200,g_face/v1690688192/pjaolvdlaf4glhjpowwf.png
