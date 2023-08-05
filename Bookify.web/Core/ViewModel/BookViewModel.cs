using Bookify.web.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class BookViewModel
    {

        public int Id { get; set; }
        
        public string Title { get; set; } = null!;

        public string AuthorName { get; set; } = null!;
   
  
        public string Publisher { get; set; } = null!;
        public DateTime PublishingDaTe { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }


        public string Hall { get; set; } = null!;
        public bool IsAvailableForRental { get; set; }
        public string Description { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedOn { get; set; }

        public IEnumerable<string> categories { get; set; } = null!;
    }
}
