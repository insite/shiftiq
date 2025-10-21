using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Accounts.Developers.Controls
{
    public partial class Details : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UserIdentifier.AutoPostBack = true;
            UserIdentifier.ValueChanged += UserIdentifier_ValueChanged;

            AddAddress.Click += AddAddress_Click;

            AddressesValidator.ServerValidate += AddressesValidator_ServerValidate;
        }

        private void UserIdentifier_ValueChanged(object sender, EventArgs e)
        {
            if (!UserIdentifier.HasValue)
                return;

            var user = UserSearch.Select(UserIdentifier.Value.Value);
            TokenName.Text = user.FullName;
            TokenEmail.Text = user.Email;
        }

        private void AddAddress_Click(object sender, EventArgs e)
        {
            var addresses = GetAddresses();

            addresses.Add(string.Empty);

            BindAddressRepeater(addresses);
        }

        private void AddressesValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var errors = new List<string>();

            foreach (var address in GetAddresses())
            {
                if (string.IsNullOrEmpty(address))
                    continue;

                var parts = address.Split('.');
                if (parts.Length != 4 || !parts.All(x => int.TryParse(x, out var num) && num >= 0 && num <= 255))
                    errors.Add($"<strong>{address}</strong> is not valid IP address");
            }

            args.IsValid = errors.Count == 0;

            if (!args.IsValid)
                ((BaseValidator)source).ErrorMessage = string.Join("<br/>", errors);
        }

        public void SetDefaultInputValues(string secret)
        {
            TokenSecret.Text = secret;
            TokenExpired.DefaultTimeZone = User.TimeZone.Id;
            TokenExpired.Value = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, User.TimeZone).AddMonths(3);
            OrganizationIdentifier.Values = null;

            BindAddressRepeater(new[] { string.Empty });
        }

        public void SetInputValues(QPersonSecret secret)
        {
            UserIdentifier.Value = secret.Person?.UserIdentifier ?? Guid.Empty;

            TokenName.Text = secret.Person?.User?.FullName ?? string.Empty;
            TokenEmail.Text = secret.Person?.User?.Email ?? string.Empty;

            TokenSecret.Text = secret.SecretValue;
            TokenExpired.Value = secret.SecretExpiry;

            var organization = secret.Person?.Organization;

            if (organization == null || organization.OrganizationCode == "*")
                OrganizationIdentifier.Values = null;
            else
                OrganizationIdentifier.Values = new[] { organization.OrganizationIdentifier };
        }

        public void GetInputValues(QPersonSecret secret)
        {
            if (!UserIdentifier.HasValue)
                return;

            if (!OrganizationIdentifier.HasValue)
                return;

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value.Value, OrganizationIdentifier.Values.FirstOrDefault());

            if(person == null) 
                return;

            secret.PersonIdentifier = person.PersonIdentifier;
            secret.SecretName = TokenName.Text;
            secret.SecretValue = TokenSecret.Text;
            secret.SecretExpiry = TokenExpired.Value.Value;
            secret.SecretType = "Token";
            secret.PersonIdentifier = person.PersonIdentifier;
        }

        private void BindAddressRepeater(IEnumerable<string> data)
        {
            AddressRepeater.DataSource = data;
            AddressRepeater.DataBind();
        }

        private List<string> GetAddresses()
        {
            var result = new List<string>();

            foreach (RepeaterItem item in AddressRepeater.Items)
            {
                var input = (ITextBox)item.FindControl("Address");

                result.Add(input.Text);
            }

            return result;
        }
    }
}