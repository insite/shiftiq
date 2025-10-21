using System;
using System.Collections.Generic;

using Shift.Common;

using ListItem = Shift.Common.ListItem;

namespace InSite.Common.Web.UI
{
    public class ActivityComboBox : ComboBox
    {
        private class GroupItem : ListItem
        {
            public List<ListItem> Items { get; } = new List<ListItem>();
        }

        public Guid CourseIdentifier
        {
            get => (Guid?)ViewState[nameof(CourseIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public Guid? ExcludeActivityIdentifier
        {
            get => (Guid?)ViewState[nameof(ExcludeActivityIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(ExcludeActivityIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var outline = Persistence.CourseSearch.SelectCourseListItem(CourseIdentifier);
            if (outline == null)
                return list;

            foreach (var module in outline.Modules)
            {
                var group = new GroupItem { Text = module.Name };

                foreach (var activity in module.Activities)
                    group.Items.Add(new ListItem
                    {
                        Text = activity.Name,
                        Value = activity.Identifier.ToString(),
                        Enabled = activity.Identifier != ExcludeActivityIdentifier
                    });

                list.Add(group);
            }

            return list;
        }

        protected override ComboBoxItem LoadItem(ListItem item)
        {
            if (item is GroupItem group)
            {
                var optionGroup = new ComboBoxOptionGroup(group.Text);

                foreach (var gi in group.Items)
                    optionGroup.Items.Add((ComboBoxOption)base.LoadItem(gi));

                return optionGroup;
            }
            else
                return base.LoadItem(item);
        }
    }
}