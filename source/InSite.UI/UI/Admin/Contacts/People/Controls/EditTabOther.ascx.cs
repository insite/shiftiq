using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class EditTabOther : BaseUserControl
    {
        #region Events

        public event AlertHandler StatusUpdated;

        private void OnAlert(AlertType type, string message) =>
            StatusUpdated?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        private Guid? UserIdentifier
        {
            get => (Guid?)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        #endregion

        #region Fields

        private Blowfish _blowfish;

        #endregion

        #region Initialization

        public void ApplyAccessControl()
        {
            SocialInsuranceNumber.Enabled = Identity.IsGranted(ActionName.Admin_Contacts_People_Edit_SocialInsuranceNumber);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RegionCombo.Visible = Organization.Identifier == OrganizationIdentifiers.RCABC;
            RegionInput.Visible = !RegionCombo.Visible;

            RegionCombo.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            RegionCombo.Settings.OrganizationIdentifier = Organization.Key;

            ReferrerIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            ReferrerIdentifier.ListFilter.CollectionName = CollectionName.Contacts_Settings_Referrers_Name;
            ReferrerIdentifier.RefreshData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.IsAssociation)
                OrganizationMultiView.SetActiveView(AssociationView);
            else if (Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC)
                OrganizationMultiView.SetActiveView(RcabcView);
            else if (Organization.OrganizationIdentifier == OrganizationIdentifiers.Inspire)
                OrganizationMultiView.SetActiveView(NcasView);

            OrganizationHeading.InnerText = Organization.Name + " Custom Fields";
        }

        #endregion

        #region Event handlers

        public void OnEmployerChanged(QGroup employer)
        {
            if (employer != null && employer.GroupRegion.IsNotEmpty())
                RegionCombo.Value = employer.GroupRegion;
        }

        #endregion

        #region Methods (set/get)

        public void SetInputValues(QPerson person)
        {
            UserIdentifier = person.UserIdentifier;

            var user = person.User;

            Language.Settings.IncludeLanguage = CurrentSessionState.Identity.Organization.Languages
                .Select(x => x.TwoLetterISOLanguageName)
                .Append(MultilingualString.DefaultLanguage)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            Language.RefreshData();

            Language.AllowBlank = string.IsNullOrEmpty(person.Language);
            Language.RefreshData();
            Language.Value = person.Language;

            TimeZone.Value = user.TimeZone;
            Honorific.Value = user.Honorific;

            if (SocialInsuranceNumber.Enabled)
                SocialInsuranceNumber.Text = GetBlowfish().DecipherString(person.SocialInsuranceNumber);
            else if (person.SocialInsuranceNumber.IsEmpty())
                SocialInsuranceNumber.Text = string.Empty;
            else
                SocialInsuranceNumber.Text = "******";

            SinModified.Text = person.SinModified.HasValue
                ? "Updated on " + person.SinModified.Format(User.TimeZone)
                : null;

            // BcpvpaView

            ShippingPreference.Text = person.ShippingPreference;

            // RcabcView

            RegionCombo.Value = person.Region;
            RegionInput.Text = person.Region;
            WebSiteUrl.Text = person.WebSiteUrl;
            UnionInfoRcabc.Text = person.EmployeeUnion;
            IsEnglishLearner.Checked = string.Equals(person.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase);

            // NcasView

            TradeworkerNumberNcas.Text = person.TradeworkerNumber;
            CredentialingCountry.Text = person.CredentialingCountry;

            if (!string.IsNullOrEmpty(person.Referrer))
            {
                var item = ReferrerIdentifier.FindOptionByText(person.Referrer);
                if (item != null)
                    item.Selected = true;
            }

            // PersonFieldCard

            var personFields = TPersonFieldSearch.Bind(x => x, new TPersonFieldFilter
            {
                UserIdentifier = UserIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });

            PersonFieldRepeater.DataSource = personFields;
            PersonFieldRepeater.DataBind();
            PersonFieldCard.Visible = personFields.Count > 0;
        }

        public void GetInputValues(QUser user, QPerson person)
        {
            var isRcabc = Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC;
            var isNcas = Organization.OrganizationIdentifier == OrganizationIdentifiers.Inspire;

            person.Language = Language.Value;
            user.TimeZone = TimeZone.Value;
            user.Honorific = Honorific.Value;

            if (SocialInsuranceNumber.Enabled)
                person.SocialInsuranceNumber = GetBlowfish().EncipherString(SocialInsuranceNumber.Text);


            // BcpvpaView

            person.ShippingPreference = ShippingPreference.Text;

            // RcabcView

            if (isRcabc)
            {
                person.EmployeeUnion = UnionInfoRcabc.Text;
                person.Region = RegionCombo.Value;
            }
            else
            {
                person.Region = RegionInput.Text;
            }

            person.WebSiteUrl = WebSiteUrl.Text;
            person.FirstLanguage = IsEnglishLearner.Checked ? "Not English" : "English";


            // NcasView

            if (isNcas)
            {
                person.TradeworkerNumber = TradeworkerNumberNcas.Text;
                person.CredentialingCountry = CredentialingCountry.Text;
                person.Referrer = ReferrerIdentifier.GetSelectedOption()?.Text;
            }
        }

        public void Save()
        {

        }

        #endregion

        #region Methods (helpers)

        private Blowfish GetBlowfish()
        {
            if (_blowfish == null)
                _blowfish = new Blowfish(EncryptionKey.Default);

            return _blowfish;
        }

        #endregion
    }
}