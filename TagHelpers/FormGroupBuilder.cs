using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;
using System.Text.Encodings.Web;

namespace DynamicFormTagHelper.TagHelpers
{
    public static class FormGroupBuilder
    {
        public static async Task<string> GetFormGroup(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {

            string label = await buildLabelHtml(generator, property, viewContext, encoder);
            string input = await buildInputHtml(generator, property, viewContext, encoder);
            string validation = await buildValidationMessage(generator, property, viewContext, encoder);
            return $@"<div class='form-group'>
                {label}
                {input}
                {validation}
</div>";
        }

        private static async Task<string> buildLabelHtml(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {
            TagHelper label = new LabelTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };
            return await GetGeneratedContent("label", TagMode.StartTagAndEndTag, label, encoder: encoder);
        }

        private static async Task<string> buildInputHtml(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {
            TagHelper input = new InputTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };

            return await GetGeneratedContent("input",
                TagMode.SelfClosing,
                input,
                attributes: new TagHelperAttributeList { new TagHelperAttribute("class", "form-control")
                },
                encoder: encoder
                );
        }

        private static async Task<string> buildValidationMessage(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {
            TagHelper validationMessage = new ValidationMessageTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };
            return await GetGeneratedContent("span", TagMode.StartTagAndEndTag, validationMessage, encoder: encoder);
        }

        private static async Task<string> GetGeneratedContent(string tagName, TagMode tagMode,
            ITagHelper tagHelper, HtmlEncoder encoder, TagHelperAttributeList attributes = null )
        {
            if (attributes == null)
            {
                attributes = new TagHelperAttributeList();
            }

            TagHelperOutput output = new TagHelperOutput(tagName, attributes, (arg1, arg2) =>
            {
                return Task.Factory.StartNew<TagHelperContent>(() => new DefaultTagHelperContent());
            })
            {
                TagMode = tagMode
            };
            TagHelperContext context = new TagHelperContext(attributes, new Dictionary<object, object>(), Guid.NewGuid().ToString());
            
            await tagHelper.ProcessAsync(context, output);

            return output.RenderTag(encoder);
        }

        private static string RenderTag(this TagHelperOutput output, HtmlEncoder encoder)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, encoder);
                return writer.ToString();
            }
        }
    }
}
