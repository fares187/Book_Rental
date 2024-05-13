namespace Bookify.web.Core.ViewModel
{
    public class RentalDetailsViewModel
    {
        public int id { get; set;}
        public DateTime RentalDate { get; set;} 
        public IEnumerable<RentalCopyDetailsViewModel> Copies{ get; set; }

    }
}
