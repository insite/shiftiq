using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class AccommodationTypeComboBox : ComboBox
    {
        #region Classes

        private class GroupItem : ListItem
        {
            public List<ListItem> Items { get; } = new List<ListItem>();
        }

        #endregion

        #region Properties

        public IEnumerable<string> AdditionalOptions
        {
            get => (string[])ViewState[nameof(AdditionalOptions)];
            set
            {
                var array = value?.Where(x => x.HasValue()).Distinct().OrderBy(x => x).ToArray();

                ViewState[nameof(AdditionalOptions)] = array.IsEmpty() ? null : array;
            }
        }

        #endregion

        #region Data binding

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

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                CollectionName = CollectionName.Activities_Exams_Accommodation_Type
            });
            var additional = new List<string>(AdditionalOptions ?? new string[0]);

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];

                list.Add(new ListItem { Text = item.ItemName, Value = item.ItemName });

                additional.RemoveAll(x => x.Equals(item.ItemName, StringComparison.OrdinalIgnoreCase));
            }

            if (additional.Count > 0)
            {
                var group = new GroupItem { Text = "Additional Types" };

                foreach (var name in additional)
                    group.Items.Add(new ListItem { Text = name, Value = name });

                list.Add(group);
            }

            return list;
        }

        #endregion
    }
}