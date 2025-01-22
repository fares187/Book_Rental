using Microsoft.Identity.Client;

namespace Bookify.web.Core.ViewModel
{
    public class SubscriptionViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; } 
        public string Status {

            get
            {
                return DateTime.Today > EndDate? "Expired": (DateTime.Today < StartDate)? "Not Now" :"Active";
            }
        }
    }
}
