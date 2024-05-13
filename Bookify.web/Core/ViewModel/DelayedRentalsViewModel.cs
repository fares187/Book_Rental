using Microsoft.Identity.Client;

namespace Bookify.web.Core.ViewModel
{
    public class DelayedRentalsViewModel
    {
        public int SubscriberId { get; set; }   
        public string SubscriberName { get; set; }
        public string SubscriberPhone { get; set; }
        public string BookTitle { get; set; }
        public string BookSerial { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExtendedOn { get; set; }
        public int DelayInDays { get; set; }

    }
}
