using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicFormTagHelper.TagHelpers
{
    public static class FormGroupBuilder
    {
        public static async Task<string> GetFormGroup(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext)
        {

            string label = await buildLabelHtml(generator, property, viewContext);
            string input = await buildInputHtml(generator, property, viewContext);
            string validation = await buildValidationMessage(generator, property, viewContext);
            return $@"<div class='form-group'>
                {label}
                {input}
                {validation}
</div>";
        }

        private static async Task<string> buildLabelHtml(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext)
        {
            TagHelper label = new LabelTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };
            return await GetGeneratedContent("label", TagMode.StartTagAndEndTag, label);
        }

        private static async Task<string> buildInputHtml(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext)
        {
            TagHelper input = new InputTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };

            return await GetGeneratedContent("input",
                TagMode.SelfClosing,
                input,
                attributes: new TagHelperAttributeList { new TagHelperAttribute("class", "form-control") }
                );
        }

        private static async Task<string> buildValidationMessage(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext)
        {
            TagHelper validationMessage = new ValidationMessageTagHelper(generator)
            {
                For = new ModelExpression(property.Metadata.PropertyName, property),
                ViewContext = viewContext
            };
            return await GetGeneratedContent("span", TagMode.StartTagAndEndTag, validationMessage);
        }

        private static async Task<string> GetGeneratedContent(string tagName, TagMode tagMode,
            ITagHelper tagHelper, TagHelperAttributeList attributes = null)
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
            return output.renderTag();
        }

        private static string renderTag(this TagHelperOutput output)
        {
            switch (output.TagMode)
            {
                case TagMode.StartTagAndEndTag:
                    return $"<{output.TagName} {output.Attributes.renderAttributes()}>{output.Content.GetContent()}</{output.TagName}>";
                case TagMode.SelfClosing:
                    if (output.TagName != null)
                    {
                        return $"<{output.TagName} {output.Attributes.renderAttributes()}/>";
                    }
                    else
                    {
                        return output.Content.GetContent();
                    }
                case TagMode.StartTagOnly:
                    return $"<{output.TagName} {output.Attributes.renderAttributes()}>";
                default:
                    throw new Exception("UNKOWN TAG MODE");
            }
        }

        private static string renderAttributes(this TagHelperAttributeList attributes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var attr in attributes)
            {
                builder.Append($"{attr.Name}=\"{attr.Value}\" ");
            }
            return builder.ToString();
        }
    }
}
