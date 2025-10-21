using System;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Jobs.Employers.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VGroupEmployerFilter>
    {
        #region Properties

        protected override string[] DefaultShowColumns => new[] { "Name", "Contact Name", "Email" };

        public override VGroupEmployerFilter Filter
        {
            get
            {
                var filter = new VGroupEmployerFilter
                {
                    Email = Email.Text,
                    Address = Address.Text,
                    Country = CountryName.Text,
                    Province = ProvinceName.Text,
                    City = CityName.Text,
                    IsApproved = IsApproved.ValueAsBoolean,
                    DateRegisteredSince = DateRegisteredSince.Value,
                    DateRegisteredBefore = DateRegisteredBefore.Value,

                    EmployerName = EmployerName.Text,
                    EmployerContactName = EmployerContactName.Text,
                    EmployerSize = EmployerSize.Value,
                    Industry = Industry.Value,
                    Sector = Sector.Value,

                    OrganizationIdentifier = Organization.Identifier,
                    GroupDepartmentIdentifiers = DepartmentGroupIdentifier.ValuesAsGuidArray
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                DepartmentGroupIdentifier.ValuesAsGuid = value.GroupDepartmentIdentifiers;

                Email.Text = value.Email;
                Address.Text = value.Address;
                CountryName.Text = value.Country;
                ProvinceName.Text = value.Province;
                CityName.Text = value.City;
                IsApproved.ValueAsBoolean = value.IsApproved;

                DateRegisteredSince.Value = value.DateRegisteredSince;
                DateRegisteredBefore.Value = value.DateRegisteredBefore;

                EmployerName.Text = value.EmployerName;
                EmployerContactName.Text = value.EmployerContactName;
                EmployerSize.Value = value.EmployerSize;
                Industry.Value = value.Industry;
                Sector.Value = value.Sector;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentGroupIdentifier.ListFilter.OrganizationIdentifier = Organization.Identifier;
            DepartmentGroupIdentifier.ListFilter.GroupType = "Department";
            DepartmentGroupIdentifier.ClearSelection();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SetupIndustrySelectors();
        }

        private void SetupIndustrySelectors()
        {
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Contacts_Settings_Industries_Name
            });

            Industry.LoadItems(data.Select(x => new ListItem
            {
                Value = x.ItemName,
                Text = x.ItemName
            }));

            Sector.Items.Clear();

            foreach (var item in data)
            {
                var group = new ComboBoxOptionGroup(item.ItemName);

                var sectors = item.ItemDescription.EmptyIfNull().Split(new string[] { "; " }, StringSplitOptions.None);
                foreach (var sector in sectors)
                    group.Items.Add(new ComboBoxOption(sector, sector));

                if (group.Items.Count > 0)
                    Sector.Items.Add(group);
            }

            if (Sector.Items.Count > 0)
                Sector.Items.Insert(0, new ComboBoxOption());
        }

        #endregion

        #region Helper methods

        public override void Clear()
        {
            DepartmentGroupIdentifier.ClearSelection();

            Email.Text = null;
            Address.Text = null;
            CountryName.Text = null;
            ProvinceName.Text = null;
            CityName.Text = null;
            IsApproved.ClearSelection();

            DateRegisteredSince.Value = null;
            DateRegisteredBefore.Value = null;

            EmployerName.Text = null;
            EmployerContactName.Text = null;
            EmployerSize.ClearSelection();
            Industry.ClearSelection();
            Sector.ClearSelection();
        }

        #endregion
    }
}