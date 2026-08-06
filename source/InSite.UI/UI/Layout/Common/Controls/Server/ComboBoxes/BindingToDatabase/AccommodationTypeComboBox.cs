using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class AccommodationTypeComboBox : ComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AccommodationTypeComboBoxSettings Settings { get; }

        public AccommodationTypeComboBox()
        {
            Settings = new AccommodationTypeComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();

        protected override ComboBoxItem LoadItem(ListItem item)
        {
            if (item is AccommodationTypeComboBoxSettings.GroupItem group)
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

    public class AccommodationTypeMultiComboBox : MultiComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AccommodationTypeComboBoxSettings Settings { get; }

        public AccommodationTypeMultiComboBox()
        {
            Settings = new AccommodationTypeComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();

        protected override ComboBoxItem LoadItem(ListItem item)
        {
            if (item is AccommodationTypeComboBoxSettings.GroupItem group)
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

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AccommodationTypeComboBoxSettings : StateBagProxy
    {
        #region Classes

        internal class GroupItem : ListItem
        {
            public List<ListItem> Items { get; } = new List<ListItem>();
        }

        #endregion

        #region Properties

        public IEnumerable<string> AdditionalOptions
        {
            get => (string[])GetValue();
            set
            {
                var array = value?.Where(x => x.HasValue()).Distinct().OrderBy(x => x).ToArray();
                SetValue(array.IsEmpty() ? null : array);
            }
        }

        #endregion

        #region Construction

        public AccommodationTypeComboBoxSettings(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion

        #region Methods

        public ListItemArray CreateDataSource()
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
