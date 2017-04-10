using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicFormTagHelper.TagHelpers
{
    public enum ChoicesTypes
    {
        DEFAULT,
        RADIO
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class ItemsSourceAttribute : Attribute
    {
        public string ItemsProperty { get; set; }
        public Type ItemsEnum { get; set; }
        public ChoicesTypes ChoicesType { get; set; } = ChoicesTypes.DEFAULT;

        public IEnumerable<SelectListItem> GetItems(ModelExplorer explorer)
        {
            if ((ItemsEnum != null) && (ItemsEnum.GetTypeInfo().IsEnum))
            {
                var items = new List<SelectListItem>();
                MemberInfo[] enumItems = ItemsEnum.GetTypeInfo().GetMembers(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < enumItems.Length; i++)
                {
                    items.Add(new SelectListItem() { Value = i.ToString(), Text = enumItems[i].Name });
                }

                return items;
            }
            else
            {
                var properties = explorer.Properties.Where(p => p.Metadata.PropertyName.Equals(ItemsProperty));
                if (properties.Count() == 1)
                {
                    return properties.First().Model as IEnumerable<SelectListItem>;
                }
                return new List<SelectListItem>();
            }
        }
    }

}
