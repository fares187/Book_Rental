using Bookify.web.Core.Const;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class UserFormViewModel
    {
        public string? Id { get;set; }
        [MaxLength(100, ErrorMessage = "the max length for full name is 100"),Display(Name ="Full Name"),
            RegularExpression(RegaxStatic.CharactersOnly_Eng,ErrorMessage =Error.OnlyEnglishLetter)]
        public string fullname { get; set; } = null!;

        [Remote("UniqeUsername", null!, ErrorMessage = Error.Email_O_Usrname)]
        [MaxLength(100, ErrorMessage = "the max length for user name is 100"),Display(Name ="User Name")
            ,RegularExpression(RegaxStatic.username,ErrorMessage =Error.Username_validation)]
        public string UserName { get; set; } = null!;
        [Remote("UniqeEmail", null!, ErrorMessage = Error.Email_O_Usrname)]
        [MaxLength(200, ErrorMessage = "the max length for Email is 200")]
        public string Email { get; set; } = null!;

    
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 8)]
        [DataType(DataType.Password),
            RegularExpression(RegaxStatic.password,ErrorMessage =Error.password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }=null!;

        [Display(Name ="Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
