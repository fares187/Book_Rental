using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.web.Core.ViewModel
{
    public class AreasViewModel
    {
        public IEnumerable<SelectListItem> item { get; set; }    
    }
}
