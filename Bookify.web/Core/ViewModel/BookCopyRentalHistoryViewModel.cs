namespace Bookify.web.Core.ViewModel
{
    public class BookCopyRentalHistoryViewModel
    {
        public int Serial { get; set; } 
        
        public string SubName { get; set; }
        public string SubImageUrl { get; set; }
        public string SubThumbnailImageUrl { get; set; }   
        public DateTime RentalDate { get; set; }
        public DateTime? ExtendOn { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate{ get; set; }

    }
}
