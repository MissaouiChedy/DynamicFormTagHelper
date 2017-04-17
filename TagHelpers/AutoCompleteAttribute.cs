using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicFormTagHelper.TagHelpers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class AutoCompleteAttribute : Attribute
    {
        public string SuggestionsProperty { get; set; }

        public string SuggestionsEndpoint { get; set; }
    }
}
