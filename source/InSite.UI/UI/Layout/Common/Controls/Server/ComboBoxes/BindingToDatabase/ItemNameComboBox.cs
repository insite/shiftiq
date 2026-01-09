using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ItemNameComboBox : ComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItemNameComboBoxSettings Settings { get; }

        public ItemNameComboBox()
        {
            Settings = new ItemNameComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    public class ItemNameMultiComboBox : MultiComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItemNameComboBoxSettings Settings { get; }

        public ItemNameMultiComboBox()
        {
            Settings = new ItemNameComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ItemNameComboBoxSettings : StateBagProxy
    {
        #region Properties

        public bool AllowDataBinding
        {
            get => (bool)(GetValue() ?? true);
            set => SetValue(value);
        }

        public bool AllowNullSearch
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public Guid? OrganizationIdentifier
        {
            get => (Guid?)GetValue();
            set => SetValue(value);
        }

        public bool UseAlphabeticalOrder
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public bool UseSequenceOrder
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public bool UseCurrentOrganization
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public bool UseGlobalOrganizationIfEmpty
        {
            get => (bool)(GetValue() ?? false);
            set => SetValue(value);
        }

        public string CollectionName
        {
            get => ((string)GetValue()).IfNullOrEmpty(Shift.Constant.CollectionName.Utilities_Defaults_Classification_Status);
            set => SetValue(value);
        }

        #endregion

        #region Construction

        public ItemNameComboBoxSettings(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        #endregion

        #region Methods

        public ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            if (AllowDataBinding)
            {
                var filter = new TCollectionItemFilter
                {
                    OrganizationIdentifier = UseCurrentOrganization
                        ? CurrentSessionState.Identity.Organization.OrganizationIdentifier
                        : OrganizationIdentifier,
                    CollectionName = CollectionName
                };

                if (filter.OrganizationIdentifier.HasValue && UseGlobalOrganizationIfEmpty && !TCollectionItemCache.Exists(filter))
                    filter.OrganizationIdentifier = OrganizationIdentifiers.Global;

                var query = TCollectionItemCache.Query(filter);

                if (UseAlphabeticalOrder)
                    query = query.OrderBy(x => x.ItemName).ThenBy(x => x.ItemNumber);
                else if (UseSequenceOrder)
                    query = query.OrderBy(x => x.ItemSequence).ThenBy(x => x.ItemNumber).ThenBy(x => x.ItemName);
                else
                    query = query.OrderBy(x => x.ItemNumber).ThenBy(x => x.ItemName);

                var items = query.ToArray();

                foreach (var item in items)
                    list.Add(item.ItemName, !string.IsNullOrEmpty(item.ItemDescription) ? $"{item.ItemName} ({item.ItemDescription})" : item.ItemName);

                if (AllowNullSearch)
                    list.Add("null", "- Unknown -");
            }

            return list;
        }

        #endregion
    }
}
