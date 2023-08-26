using Bookify.web.Core.Const;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class UserPasswordViewModel
    {

        public string Id { get; set; } = null!;
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password),
            RegularExpression(RegaxStatic.password, ErrorMessage = Error.password)]
        public string Password { get; set; }= null!;
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }  =null!;
    }
}
