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
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace DynamicFormTagHelper.TagHelpers
{
    public static class FormGroupBuilder
    {
        public static Task<string> GetFormGroup(ModelExplorer property, IHtmlGenerator generator, ViewContext viewContext, HtmlEncoder encoder, OverridingConfiguration config, IHtmlHelper html)
        {
            if (property.ModelType.IsSimpleType())
            {
                return _getFormGroupForSimpleProperty(property, generator, viewContext, encoder, config, html);
            }
            else
            {
                return _getFormGroupsForComplexProperty(property, generator, viewContext, encoder, config, html);
            }
        }
        
        private static async Task<string> _getFormGroupForSimpleProperty(ModelExplorer property, IHtmlGenerator generator, ViewContext viewContext, HtmlEncoder encoder, OverridingConfiguration config, IHtmlHelper html)
        {
            string label = await buildLabelHtml(generator, property, viewContext, encoder, config);
            string input;
            var inputConfig = config.GetByPropertyFullName(getFullPropertyName(property));
            if (inputConfig != null)
            {
                input = renderTemplate(property, inputConfig.InputTemplatePath, viewContext, encoder, html);
            }
            else
            {
                input = await buildInputHtml(generator, property, viewContext, encoder);
            }

            string validation = await buildValidationMessage(generator, property, viewContext, encoder);
            return $@"<div class='form-group'>
                {label}
                {input}
                {validation}
</div>";
        }
        private static string renderTemplate(ModelExplorer property, string path, ViewContext viewContext, HtmlEncoder encoder, IHtmlHelper html)
        {
            (html as IViewContextAware).Contextualize(viewContext);
            
            return html.Editor(property.Metadata.PropertyName, path).RenderTag(encoder);
        }
        private static async Task<string> _getFormGroupsForComplexProperty(ModelExplorer property, IHtmlGenerator generator, ViewContext viewContext, HtmlEncoder encoder, OverridingConfiguration config, IHtmlHelper html)
        {
            StringBuilder builder = new StringBuilder();

            string label = await buildLabelHtml(generator, property, viewContext, encoder, config);
            foreach (var prop in property.Properties)
            {
                builder.Append(await GetFormGroup(prop, generator, viewContext, encoder, config, html));
            }

            return $@"<div class='form-group'>
                    {label}
                    <div class=""sub-form-group"">
                        {builder.ToString()}
                    </div>
</div>";
        }

        private static async Task<string> buildLabelHtml(IHtmlGenerator generator, ModelExplorer property, 
            ViewContext viewContext, HtmlEncoder encoder, OverridingConfiguration config)
        {
            TagHelper label = new LabelTagHelper(generator)
            {
                For = new ModelExpression(getFullPropertyName(property), property),
                ViewContext = viewContext
            };

            var propConfig = config.GetByPropertyFullName(getFullPropertyName(property));
            return await GetGeneratedContent("label", TagMode.StartTagAndEndTag, label, encoder, new TagHelperAttributeList() { new TagHelperAttribute("class", propConfig?.LabelClasses)});
        }

        private static async Task<string> buildInputHtml(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {
           TagHelper input = new InputTagHelper(generator)
           {
                For = new ModelExpression(getFullPropertyName(property), property),
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
        private static string getFullPropertyName(ModelExplorer property)
        {
            List<string> nameComponents = new List<String> { property.Metadata.PropertyName };

            while (!string.IsNullOrEmpty(property.Container.Metadata.PropertyName))
            {
                nameComponents.Add(property.Container.Metadata.PropertyName);
                property = property.Container;
            }

            nameComponents.Reverse();
            return string.Join(".", nameComponents);
        }
        private static async Task<string> buildValidationMessage(IHtmlGenerator generator, ModelExplorer property, ViewContext viewContext, HtmlEncoder encoder)
        {
            TagHelper validationMessage = new ValidationMessageTagHelper(generator)
            {
                For = new ModelExpression(getFullPropertyName(property), property),
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
                return Task.Run<TagHelperContent>(() => new DefaultTagHelperContent());
            })
            {
                TagMode = tagMode
            };
            TagHelperContext context = new TagHelperContext(attributes, new Dictionary<object, object>(), Guid.NewGuid().ToString());

            tagHelper.Init(context);
            await tagHelper.ProcessAsync(context, output);

            return output.RenderTag(encoder);
        }

        #region Utility_Extension_Methods
        private static bool IsSimpleType(this Type propertyType)
        {
            Type[] simpleTypes = new Type[]
            {
                typeof(string), typeof(bool), typeof(byte), typeof(char), typeof(DateTime), typeof(DateTimeOffset),
                typeof(decimal), typeof(double), typeof(Guid), typeof(short), typeof(int), typeof(long), typeof(float),
                typeof(TimeSpan), typeof(ushort), typeof(uint),typeof(ulong)
            };
            if (propertyType.IsConstructedGenericType && propertyType.Name.Equals("Nullable`1"))
            {
                return IsSimpleType(propertyType.GenericTypeArguments.First());
            }
            else
            {
                return (!propertyType.IsArray && !propertyType.IsPointer && simpleTypes.Contains(propertyType));
            }
        }
        private static string RenderTag(this IHtmlContent output, HtmlEncoder encoder)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, encoder);
                return writer.ToString();
            }
        }
        #endregion
    }
}
