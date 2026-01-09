using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Individual.Controls
{
    public partial class AddressEditor : UserControl
    {
        #region Classes

        [Serializable]
        private class AddressModel
        {
            public Guid AddressIdentifier { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string Description { get; set; }
            public string PostalCode { get; set; }
            public string Province { get; set; }
            public string Street1 { get; set; }
            public string Street2 { get; set; }

            public AddressModel(QGroupAddress address)
            {
                City = address.City;
                Country = address.Country;
                Description = address.Description;
                PostalCode = address.PostalCode;
                Province = address.Province;
                Street1 = address.Street1;
                Street2 = address.Street2;
            }

            public AddressModel(QPersonAddress address)
            {
                AddressIdentifier = address.AddressIdentifier;
                City = address.City;
                Country = address.Country;
                Description = address.Description;
                PostalCode = address.PostalCode;
                Province = address.Province;
                Street1 = address.Street1;
                Street2 = address.Street2;
            }

            public AddressModel(Address address)
            {
                AddressIdentifier = address.AddressIdentifier;
                City = address.City;
                Country = address.Country;
                Description = address.Description;
                PostalCode = address.PostalCode;
                Province = address.Province;
                Street1 = address.Street1;
                Street2 = address.Street2;
            }
        }

        #endregion

        #region Properties

        public Unit FieldsWidth
        {
            get
            {
                EnsureChildControls();

                return Street1.Width;
            }
            set
            {
                EnsureChildControls();

                Description.Width = value;
                Street1.Width = value;
                Street2.Width = value;
                City.Width = value;
                Country.Width = value;
                Province.Width = value;
                PostalCode.Width = value;
            }
        }

        public AddressField HiddenFields
        {
            get { return (AddressField)(ViewState[nameof(HiddenFields)] ?? AddressField.None); }
            set { ViewState[nameof(HiddenFields)] = value; }
        }

        public bool ReadOnly
        {
            get { return ViewState[nameof(ReadOnly)] != null && (bool)ViewState[nameof(ReadOnly)]; }
            set { ViewState[nameof(ReadOnly)] = value; }
        }

        public AddressField RequiredFields
        {
            get { return (AddressField)(ViewState[nameof(RequiredFields)] ?? AddressField.None); }
            set { ViewState[nameof(RequiredFields)] = value; }
        }

        private AddressModel AddressData
        {
            get => Session[UniqueID + nameof(AddressModel)] as AddressModel;
            set => Session[UniqueID + nameof(AddressModel)] = value;
        }

        private AddressModel EmployerAddressData
        {
            get => Session[UniqueID + nameof(EmployerAddressData)] as AddressModel;
            set => Session[UniqueID + nameof(EmployerAddressData)] = value;
        }

        private AddressModel HomeAddressData
        {
            get => Session[UniqueID + nameof(HomeAddressData)] as AddressModel;
            set => Session[UniqueID + nameof(HomeAddressData)] = value;
        }

        #endregion

        #region Setting and getting input values

        private void SetInputValues(AddressModel address, string type = null)
        {
            if (type != null) AddressHeading.InnerText = Common.LabelHelper.GetTranslation($"{type} Address");

            Description.Text = address.Description;
            Street1.Text = address.Street1;
            Street2.Text = address.Street2;

            City.Text = address.City;
            Province.Text = address.Province;
            PostalCode.Text = address.PostalCode;
            if(address.Country.HasValue())
                Country.Value = ServiceLocator.CountrySearch.SelectByName(address.Country)?.Identifier;
            else
            {
                var orgCountry = CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada";
                Country.Value = ServiceLocator.CountrySearch.SelectByName(orgCountry)?.Identifier;
            }

            SetNamesPerCountry(address.Country);
        }

        public void SetUserInputValues(string type, QPersonAddress address, QGroupAddress employerAddress = null, string employerName = null, QPersonAddress homeAddress = null)
        {
            if (address == null)
                address = new QPersonAddress { AddressIdentifier = UniqueIdentifier.Create() };

            AddressData = new AddressModel(address);
            if (employerAddress != null) EmployerAddressData = new AddressModel(employerAddress);
            if (homeAddress != null) HomeAddressData = new AddressModel(homeAddress);

            SetInputValues(AddressData, type);

            if (employerAddress != null &&
                (
                    employerAddress.Street1.HasValue() || 
                    employerAddress.Street2.HasValue() || 
                    employerAddress.City.HasValue() || 
                    employerAddress.Province.HasValue() || 
                    employerAddress.PostalCode.HasValue() || 
                    employerAddress.Country.HasValue())
                )
            {
                EmployerAddressPanel.Visible = true;
                EmployerAddressTitle.InnerText = Common.LabelHelper.GetTranslation($"Employer {type} Address");
                EmployerName.Text = employerName;
                EmployerAddress.Text = LocationHelper.ToHtml(employerAddress.Street1, employerAddress.Street2, employerAddress.City, employerAddress.Province, employerAddress.PostalCode, employerAddress.Country, null, null);
            }

            if (homeAddress != null &&
                (
                    homeAddress.Street1.HasValue() ||
                    homeAddress.Street2.HasValue() ||
                    homeAddress.City.HasValue() ||
                    homeAddress.Province.HasValue() ||
                    homeAddress.PostalCode.HasValue() ||
                    homeAddress.Country.HasValue())
                )
            {
                HomeAddressPanel.Visible = true;
                HomeAddress.Text = homeAddress.ToHtml();
            }
        }

        public void GetInputValues(QPersonAddress address)
        {
            address.Description = Description.Text;
            address.Street1 = Street1.Text;
            address.Street2 = Street2.Text;
            address.City = City.Text;
            address.Province = Province.Text;
            address.PostalCode = PostalCode.Text;
            address.Country = ServiceLocator.CountrySearch.SelectById(Country.Value)?.Name ?? string.Empty;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CopyEmployerAddress.Click += CopyEmployerAddress_Click;
            CopyHomeAddress.Click += CopyHomeAddress_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetReadOnly(ReadOnly);
            SetVisible(HiddenFields);

            base.OnPreRender(e);
        }

        #endregion

        private void SetReadOnly(bool isReadOnly)
        {
            Street1.ReadOnly = isReadOnly;
            Street2.ReadOnly = isReadOnly;
            City.ReadOnly = isReadOnly;
            Province.Visible = !isReadOnly;
            Country.Visible = !isReadOnly;

            PostalCode.ReadOnly = isReadOnly;
        }

        private void SetVisible(AddressField hidden)
        {
            DescriptionField.Visible = !hidden.HasFlag(AddressField.Description);
            Street1Field.Visible = !hidden.HasFlag(AddressField.Street1);
            Street2Field.Visible = !hidden.HasFlag(AddressField.Street2);
            CityField.Visible = !hidden.HasFlag(AddressField.City);
            CountryField.Visible = !hidden.HasFlag(AddressField.Country);
            ProvinceField.Visible = !hidden.HasFlag(AddressField.Province);
            PostalCodeField.Visible = !hidden.HasFlag(AddressField.PostalCode);
        }

        private void SetNamesPerCountry(string country)
        {
            if (country == "United States")
            {
                ProvinceFieldLabel.InnerHtml = Common.LabelHelper.GetTranslation("State");
                PostalCodeFieldLabel.InnerHtml = Common.LabelHelper.GetTranslation("ZIP Code");
            }
            else
            {
                ProvinceFieldLabel.InnerHtml = Common.LabelHelper.GetTranslation("Province");
                PostalCodeFieldLabel.InnerHtml = Common.LabelHelper.GetTranslation("Postal Code");
            }
        }

        #region Event handlers

        private void CopyEmployerAddress_Click(object sender, EventArgs e)
        {
            SetInputValues(EmployerAddressData);
        }

        private void CopyHomeAddress_Click(object sender, EventArgs e)
        {
            SetInputValues(HomeAddressData);
        }

        #endregion
    }
}
