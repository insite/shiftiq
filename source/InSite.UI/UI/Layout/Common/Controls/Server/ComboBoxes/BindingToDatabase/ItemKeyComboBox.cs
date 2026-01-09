using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ItemKeyComboBox : ComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItemKeyComboBoxSettings Settings { get; }

        public ItemKeyComboBox()
        {
            Settings = new ItemKeyComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    public class ItemKeyMultiComboBox : MultiComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItemKeyComboBoxSettings Settings { get; }

        public ItemKeyMultiComboBox()
        {
            Settings = new ItemKeyComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ItemKeyComboBoxSettings : StateBagProxy
    {
        #region Properties

        public bool AllowDataBinding
        {
            get => (bool)(GetValue() ?? true);
            set => SetValue(value);
        }

        public string CollectionName
        {
            get => ((string)GetValue()).IfNullOrEmpty(Shift.Constant.CollectionName.Utilities_Defaults_Classification_Status);
            set => SetValue(value);
        }

        public Guid? OrganizationIdentifier
        {
            get => (Guid?)GetValue();
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

        #endregion

        #region Construction

        public ItemKeyComboBoxSettings(string prefix, StateBag viewState)
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

                foreach (var item in query.OrderBy(x => x.ItemNumber).ThenBy(x => x.ItemName))
                    list.Add(
                        item.ItemNumber.ToString(),
                        item.ItemDescription.IsNotEmpty()
                            ? item.ItemName + ": " + item.ItemDescription
                            : item.ItemName);
            }

            return list;
        }


        #endregion
    }
}