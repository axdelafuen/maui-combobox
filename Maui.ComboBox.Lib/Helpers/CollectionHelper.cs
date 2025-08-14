using System.Collections;

namespace Maui.ComboBox.Helpers
{
    public static class CollectionHelper
    {
        /// <summary>
        /// Gets the item at the specified index from a collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">Index of the desired object.</param>
        /// <returns>The founded object or null.</returns>
        public static object? GetItemAt(ICollection collection, int index)
        {
            if (collection is IList list)
                return list[index];
            int i = 0;
            foreach (var item in collection)
            {
                if (i == index) return item;
                i++;
            }
            return null;
        }
    }
}
