using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DynamicFormTagHelper.TagHelpers
{
    public class PropertyTweakingConfiguration
    {
        public ModelExpression Property { get; set; }
        public string InputTemplatePath { get; set; }
        public string LabelClasses { get; set; }
        public string InputClasses { get; set; }
        public string ValidationClasses { get; set; }
    }

    public class TweakingConfiguration
    {
        private List<PropertyTweakingConfiguration> _propertyEntries = new List<PropertyTweakingConfiguration>();

        public void Add(PropertyTweakingConfiguration propertyConfig)
        {
            _propertyEntries.Add(propertyConfig);
        }

        public PropertyTweakingConfiguration GetByPropertyFullName(string fullName)
        {
            var found = _propertyEntries.Where(e => e.Property.Name.Equals(fullName));
            if (found.Count() > 0)
            {
                return found.First();
            }
            else
            {
                return null;
            }
        }
    }

    [RestrictChildren("tweak")]
    public class DynamicFormTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-action")]
        public string TargetAction { get; set; }
        
        [HtmlAttributeName("asp-model")]
        public ModelExpression Model { get; set; }

        [HtmlAttributeName("asp-submit-button-text")]
        public string SubmitButtonText { get; set; }

        [HtmlAttributeName("asp-submit-button-classes")]
        public string SubmitButtonClasses { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private HtmlEncoder _encoder { get; set; }

        private IHtmlGenerator _htmlGenerator;

        private IHtmlHelper _htmlHelper;

        public DynamicFormTagHelper(IHtmlGenerator htmlGenerator, HtmlEncoder encoder, IHtmlHelper htmlHelper)
        {
            _htmlGenerator = htmlGenerator;
            _encoder = encoder;
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            FormGroupBuilder formGroupBuilder = new FormGroupBuilder(_htmlGenerator, ViewContext, 
                _encoder, _htmlHelper);

            TweakingConfiguration tweakingConfig = new TweakingConfiguration();
            context.Items.Add("TweakingConfig", tweakingConfig);
            await output.GetChildContentAsync();

            StringBuilder builder = new StringBuilder();
            foreach (ModelExplorer prop in Model.ModelExplorer.Properties)
            {
                if (prop.Metadata.PropertySetter != null)
                {
                    builder.Append(await formGroupBuilder.GetFormGroup(prop, tweakingConfig));
                }
            }

            if (string.IsNullOrEmpty(SubmitButtonText)) SubmitButtonText = "Create";
            builder.Append($@"<div class=""form-group"">
                          <button type=""submit"" class=""{mergeClasses(SubmitButtonClasses)}"">{SubmitButtonText}</button>
                    </div>");

            output.TagName = "form";
            output.Attributes.Add(new TagHelperAttribute("method", "post"));
            output.Attributes.Add(new TagHelperAttribute("action", TargetAction));
            output.Content.SetHtmlContent(builder.ToString());
        }
        private string mergeClasses(string userProvidedClasses)
        {
            userProvidedClasses = Regex.Replace(userProvidedClasses, "btn\b", "");
            userProvidedClasses = userProvidedClasses.Replace("btn-primary", "");
            return $"btn btn-primary {userProvidedClasses.Trim()}";
        }
    }

    [HtmlTargetElement(ParentTag = "dynamic-form")]
    public class TweakTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-property")]
        public ModelExpression TweakedProperty { get; set; }

        [HtmlAttributeName("asp-input-path")]
        public string InputTemplatePath { get; set; }

        [HtmlAttributeName("asp-label-classes")]
        public string LabelClasses { get; set; }

        [HtmlAttributeName("asp-input-classes")]
        public string InputClasses { get; set; }

        [HtmlAttributeName("asp-validation-classes")]
        public string ValidationClasses { get; set; }
        
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TweakingConfiguration overridingConfig = context.Items["TweakingConfig"] as TweakingConfiguration;
            overridingConfig.Add(new PropertyTweakingConfiguration
            {
                Property = TweakedProperty,
                InputTemplatePath = InputTemplatePath,
                LabelClasses = LabelClasses,
                InputClasses = InputClasses,
                ValidationClasses = ValidationClasses
            });
            output.SuppressOutput();
            return Task.CompletedTask;
        }
    }
}
