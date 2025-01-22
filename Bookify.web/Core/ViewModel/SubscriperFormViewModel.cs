using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.web.Core.ViewModel
{
	public class SubscriperFormViewModel
	{
		public string? key { get; set; }

		[MaxLength(100),Required,
			RegularExpression(RegaxStatic.CharactersOnly_Eng,ErrorMessage =Error.OnlyEnglishLetter)]
		public string FirstName { get; set; } = null!;

        [MaxLength(100), Required,
            RegularExpression(RegaxStatic.CharactersOnly_Eng,ErrorMessage =Error.OnlyEnglishLetter)]
        public string LastName { get; set; } = null!;
		[Display(Name ="DateOfBirth")]
		public DateTime dateOfBirth { get; set; }
		
        [MaxLength(20), Required,
            RegularExpression(RegaxStatic.nationalId,ErrorMessage =Error.nationalIdValidation)]
        [Remote("UniqeNationalId", null!, AdditionalFields = "key", ErrorMessage = Error.Dublicated)]
        public string NationalId { get; set; } = null!;
		
        [MaxLength(15), Required,
            RegularExpression(RegaxStatic.MobileNumber,ErrorMessage =Error.phoneNumbers)]
        [Remote("UniqeMobileNumber", null!, AdditionalFields = "key", ErrorMessage = Error.Dublicated)]
        public string MobileNumber { get; set; } = null!;
		[Display(Name ="Has WhatsApp?")]
		public bool HasWhatApp { get; set; }
		[EmailAddress]
        [Remote("UniqeEmail", null!, AdditionalFields = "key", ErrorMessage = Error.Dublicated)]
        public string Email { get; set; } = null!;



        public string? ImageUrl { get; set; }

        public string? ThumbnailUrl { get; set; }

        [RequiredIf("key == ''", ErrorMessage =Error.image)]
        public IFormFile? Image { get; set; } = null!;
		

		[MaxLength(200)]
		public string Address { get; set; } = null!;


		

		public int AreaId { get; set; }
		public IEnumerable<SelectListItem>? Areas {
			get;
			set; }
		public int GovernorateId { get; set; }
        public IEnumerable<SelectListItem>? Governorates { get; set; }
        //[Display(ResourceType = typeof(Resources), Name = nameof(Resources.ImageUrl))]
     

    }
}
