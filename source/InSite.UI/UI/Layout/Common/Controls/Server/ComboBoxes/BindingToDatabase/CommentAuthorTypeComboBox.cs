using System;
using System.Collections.ObjectModel;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class CommentAuthorTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        private static readonly ReadOnlyCollection<string> _items = Array.AsReadOnly(new[]
        {
            CommentAuthorType.Administrator,
            CommentAuthorType.Candidate
        });

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var item in _items)
                list.Add(item);

            return list;
        }

        public static bool IsValidValue(string value) =>
            !string.IsNullOrEmpty(value) ? _items.Contains(value) : throw new ArgumentNullException(nameof(value));
    }
}