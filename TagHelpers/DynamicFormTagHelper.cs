using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using System.Text.Encodings.Web;

namespace DynamicFormTagHelper.TagHelpers
{
    public class DynamicFormTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-action")]
        public string TargetAction { get; set; }
        
        [HtmlAttributeName("asp-model")]
        public ModelExpression Model { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private HtmlEncoder _encoder { get; set; }

        private IHtmlGenerator _htmlGenerator;

        public DynamicFormTagHelper(IHtmlGenerator htmlGenerator, HtmlEncoder encoder)
        {
            _htmlGenerator = htmlGenerator;
            _encoder = encoder;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ModelExplorer prop in Model.ModelExplorer.Properties)
            {
                if (prop.Metadata.PropertySetter != null)
                {
                    builder.Append(await FormGroupBuilder.GetFormGroup(_htmlGenerator, prop, ViewContext, _encoder));
                }
            }
            builder.Append(@"<div class='form-group'>
                          <button type='submit' class='btn btn-primary'>Create</button>
                    </div>");

            output.TagName = "form";
            output.Attributes.Add(new TagHelperAttribute("method", "post"));
            output.Attributes.Add(new TagHelperAttribute("action", TargetAction));
            output.Content.SetHtmlContent(builder.ToString());

        }
    }
}
