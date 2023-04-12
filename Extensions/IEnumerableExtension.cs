using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace CoreStoreMVC.Extensions
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Creates a list of SelectListItems from a given IEnumerable of objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the IEnumerable.</typeparam>
        /// <param name="items">The IEnumerable of objects.</param>
        /// <param name="selectedValue">The value of the item to be selected.</param>
        /// <returns>A list of SelectListItems.</returns>
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }

        /// <summary>
        /// Creates a list of SelectListItems from a given IEnumerable of objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the IEnumerable.</typeparam>
        /// <param name="items">The IEnumerable of objects.</param>
        /// <param name="selectedValue">The value of the item to be selected.</param>
        /// <returns>A list of SelectListItems.</returns>
        public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string selectedValue)
        {
            if (selectedValue is null)
                selectedValue = "";

            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}
