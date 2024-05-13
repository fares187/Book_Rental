using Bookify.web.Core.Enums;

namespace Bookify.web.Core.Models
{
    public class RentalCopy
    {
        public int RentalId { get; set; }   
        public Rental? Rental { get; set; }
        public int CopyBookId { get; set; } 
        public BookCopy? CopyBook { get; set; }
        public DateTime RentalDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration);
        public DateTime? ReturnDate { get; set; } 
        public DateTime? ExtendedOn { get; set; } 
    }
}
