namespace Bookify.web.Core.Models
{
    public class Rental
    {
        public int Id { get; set; } 
        public int SubscriberId { get; set; } 

        public Subscriper? subscriber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public bool PenaltyPaid { get; set; }
        public ICollection<RentalCopy> RentalCopies { get; set; }=new List<RentalCopy>();   


        public bool IsDeleted { get; set; }

        public string? CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LastUpdatedById { get; set; }

        public ApplicationUser? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
    }
}
