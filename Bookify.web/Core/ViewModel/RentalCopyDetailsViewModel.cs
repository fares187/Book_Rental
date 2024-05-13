

namespace Bookify.web.Core.ViewModel
{
    public class RentalCopyDetailsViewModel
    {
        public DateTime RentalDate { get; set; }
        
        public DateTime EndDate { get; set; } 
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExtendedOn { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get;set; } 
        public string? ImageThumbnailUrl { get; set; }

    }
}
