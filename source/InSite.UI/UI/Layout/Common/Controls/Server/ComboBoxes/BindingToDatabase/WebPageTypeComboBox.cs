using System;
using System.ComponentModel;
using System.Linq;

using InSite.UI.Layout.Common.Contents;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class WebPageTypeComboBox : ComboBox
    {
        [Flags]
        public enum DataItem
        {
            [Description("Folder")]
            Folder = 1,

            [Description("Page")]
            Page = 2,

            [Description("Block")]
            Block = 4
        }

        public DataItem IncludeItems
        {
            get => (DataItem)(ViewState[nameof(IncludeItems)] ?? default(DataItem));
            set => ViewState[nameof(IncludeItems)] = value;
        }

        public DataItem ExcludeItems
        {
            get => (DataItem)(ViewState[nameof(ExcludeItems)] ?? default(DataItem));
            set => ViewState[nameof(ExcludeItems)] = value;
        }

        private static readonly DataItem[] _defaultDataSource = new[]
        {
            DataItem.Folder,
            DataItem.Page,
            DataItem.Block
        };

        protected override ListItemArray CreateDataSource()
        {
            var query = _defaultDataSource.AsQueryable();

            if (IncludeItems != default)
                query = query.Where(x => IncludeItems.HasFlag(x));

            if (ExcludeItems != default)
                query = query.Where(x => !ExcludeItems.HasFlag(x));

            return new ListItemArray(query.Select(x => x.GetDescription()));
        }
    }

    public class WebPageTemplateComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var info in ControlPath.PageControlTypes)
                if (info.IsPublic)
                    list.Add(info.Name, info.Title);

            return list;
        }
    }
}