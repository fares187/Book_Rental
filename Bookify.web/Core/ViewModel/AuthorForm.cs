using Bookify.web.Core.Const;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class AuthorForm
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage =Error.max),Display(Name="Author")]
        [Remote("UniqeItem", "Authors", AdditionalFields = "Id", ErrorMessage =Error.Dublicated),
            RegularExpression(RegaxStatic.CharactersOnly_Eng, ErrorMessage = Error.OnlyEnglishLetter)]
        public string Name { get; set; } = null!;
    }
}
