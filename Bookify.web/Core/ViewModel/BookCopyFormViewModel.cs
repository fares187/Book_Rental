using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class BookCopyFormViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Display(Name = "Is Available For Rental")]
        [Required]
        public bool IsAvailableForRental { get; set; }
     [Display(Name = "Edition Number"),Range(1,1000,ErrorMessage =Error.EditionNum)]
        [Required]
        public int EditionNumber { get; set; }
        public bool showRentalInput { get; set; }   

    }
}
