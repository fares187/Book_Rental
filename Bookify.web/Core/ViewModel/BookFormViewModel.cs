using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.web.Core.ViewModel;

public class BookFormViewModel
{
    public int Id { get; set; }

    [MaxLength(250, ErrorMessage = Error.max)]
    [Remote("UniqeItem", null!, AdditionalFields = "Id,AuthorId", ErrorMessage = Error.DublicatedBooks)]
    public string Title { get; set; } = null!;

    [Display(Name = "Author")]
    [Remote("UniqeItem", null!, AdditionalFields = "Id,Title", ErrorMessage = Error.DublicatedBooks)]
    public int AuthorId { get; set; }

    public IEnumerable<SelectListItem>? Authors { get; set; }

    [MaxLength(200, ErrorMessage = Error.max)]
    public string Publisher { get; set; } = null!;

    [Display(Name = "Publishing Date")]
    [AssertThat("PublishingDate <= Today()",ErrorMessage =Error.dataError)]
  
    public DateTime PublishingDate { get; set; } = DateTime.UtcNow;

    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }

    [MaxLength(50, ErrorMessage = Error.max)]
    public string Hall { get; set; } = null!;

    [Display(Name = "Is available for rental ")]
    public bool IsAvailableForRental { get; set; }

    public string Description { get; set; } = null!;
    [Display(Name = "Selected Categories")]
    public IList<int> SelectedCategories { get; set; } = new List<int>();
    public IEnumerable<SelectListItem>? Categories { get; set; }


}

