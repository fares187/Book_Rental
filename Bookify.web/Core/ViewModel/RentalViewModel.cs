using Bookify.web.Core.Models;

namespace Bookify.web.Core.ViewModel
{
    public class RentalViewModel
    {
        public int Id { get; set; }
      

        public SubscriperViewModel? subscriber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public bool PenaltyPaid { get; set; }
        public IEnumerable<RentalCopyViewModel> RentalCopies { get; set; } = new List<RentalCopyViewModel>();

        public int TotalDelayInDays { 
            get {

                return RentalCopies.Sum(b=>b.DelayInDays);
            } 
        }
        public int numberOfCopies
        {
            get
            {

                return RentalCopies.Count();
            }
        }

        public bool IsDeleted { get; set; }

        public string? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LastUpdatedById { get; set; }

        public ApplicationUser? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
    }
}
