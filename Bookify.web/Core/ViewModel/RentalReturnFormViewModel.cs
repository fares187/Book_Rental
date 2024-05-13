using Bookify.web.Core.Const;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.web.Core.ViewModel
{
    public class RentalReturnFormViewModel
    {
        public int Id { get; set; }
        [AssertThat("(TotalDelayInDays ==0 && PenaltyPaid==false) " +
            "|| (PenaltyPaid==true)",ErrorMessage =Error.penalty)]
        [Display(Name = "Penalty Paid?")]
        public bool PenaltyPaid { get; set; }
        public IList<RentalCopyViewModel> rentalCopies { get; set; } = new List<RentalCopyViewModel>();
        public List<ReturnCopyViewModel> SelectedCopies { get; set; } = new();
        public bool AllowExtend { get; set; }
        public int TotalDelayInDays
        {
            get
            {
                return rentalCopies.Sum(c => c.DelayInDays);
            }
        }
    }
}
