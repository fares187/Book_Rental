namespace Bookify.web.Core.ViewModel
{
    public class ReportRentalsBooksViewModel
    {
        public string subscriber { get; set; }  
        public DateTime rentalDate { get; set; }    
        public DateTime endDate { get; set;}
        public string bookName { get; set; }
        public bool IsDeleted {  get; set; } 
 
    }
}
