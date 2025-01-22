using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bookify.web.Helpers
{
    [HtmlTargetElement("a", Attributes="activator-tag")]
    public class Activator : TagHelper
    {
        public string? ActivatorTag { get; set; }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContextData { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(ActivatorTag)) return;
            var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            if (currentController.Equals(ActivatorTag))
            {
                if (output.Attributes.ContainsName("class"))
                {
                    output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value}active");
                }
                else
                {
                    output.Attributes.SetAttribute("class", "active");
                }
            }
        }
        
    }
}
