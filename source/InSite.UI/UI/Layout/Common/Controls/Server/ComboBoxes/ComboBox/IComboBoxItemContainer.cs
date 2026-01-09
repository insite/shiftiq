using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    internal interface IComboBoxItemContainer
    {
        IEnumerable<ComboBoxItem> Items { get; }
    }

    internal static class ComboBoxItemContainerExt
    {
        public static IEnumerable<ComboBoxItem> FlattenItems(this IComboBoxItemContainer container)
        {
            foreach (var item in container.Items)
            {
                yield return item;

                if (item is IComboBoxItemContainer subcontainer)
                    foreach (var subitem in subcontainer.FlattenItems())
                        yield return subitem;
            }
        }

        public static IEnumerable<IComboBoxOption> FlattenOptions(this IComboBoxItemContainer container)
        {
            foreach (var item in container.Items)
            {
                if (item is IComboBoxOption option)
                    yield return option;

                if (item is IComboBoxItemContainer subcontainer)
                    foreach (var subitem in subcontainer.FlattenOptions())
                        yield return subitem;
            }
        }

        public static IComboBoxOption FindOptionByValue(this IComboBoxItemContainer container, string value, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (value.IsNotEmpty())
            {
                foreach (var option in container.FlattenOptions())
                    if (option.Value.IsNotEmpty() && string.Equals(option.Value, value, comparison))
                        return option;
            }

            return null;
        }

        public static IComboBoxOption FindOptionByValue(this IComboBoxItemContainer container, string value, bool ignoreCase) =>
            FindOptionByValue(container, value, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

        public static IComboBoxOption FindOptionByText(this IComboBoxItemContainer container, string text, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (text.IsNotEmpty())
            {
                foreach (var option in container.FlattenOptions())
                    if (option.Text.IsNotEmpty() && string.Equals(option.Text, text, comparison))
                        return option;
            }

            return null;
        }

        public static IComboBoxOption FindOptionByText(this IComboBoxItemContainer container, string text, bool ignoreCase) =>
            FindOptionByText(container, text, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

        public static ComboBoxItem FindItemByText(this IComboBoxItemContainer container, string text, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (text.IsNotEmpty())
            {
                foreach (var item in container.FlattenItems())
                    if (item.Text.IsNotEmpty() && string.Equals(item.Text, text, comparison))
                        return item;
            }

            return null;
        }

        public static ComboBoxItem FindItemByText(this IComboBoxItemContainer container, string text, bool ignoreCase) =>
            FindItemByText(container, text, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
    }
}
