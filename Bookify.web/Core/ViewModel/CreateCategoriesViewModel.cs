using Bookify.web.Core.Const;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace Bookify.web.Core.ViewModel
{
    public class CreateCategoriesViewModel
    {
        public int id { get; set; } 
        [MaxLength(250, ErrorMessage = Error.max), Display(Name = "Category")]
        [Remote("UniqeItem", "Categories",AdditionalFields ="id" ,ErrorMessage =Error.Dublicated),
            RegularExpression(RegaxStatic.CharactersOnly_Eng, ErrorMessage = Error.OnlyEnglishLetter)]
        public string Name { get; set; } = null!;
    }
}
