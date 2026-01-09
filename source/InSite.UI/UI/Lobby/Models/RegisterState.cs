using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.SessionState;

using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Lobby
{
    public class RegisterState
    {
        private HttpSessionState _session { get; set; }
        private NameValueCollection _parameters { get; set; }
        private string _formKey { get; set; }

        public RegisterState(OrganizationState organization, HttpSessionState session, NameValueCollection parameters, string formKey)
        {
            Organization = organization;
            _session = session;
            _parameters = parameters;
            _formKey = formKey;
        }

        public Guid ClientFormKey
        {
            get => Guid.TryParse(_formKey, out var guid) ? guid : Guid.Empty;
            set => _formKey = value.ToString();
        }

        public bool IsExpired
            => ClientFormKey == Guid.Empty || ServerFormKey == null || ServerFormKey.IsSubmitted || ClientFormKey != ServerFormKey.Id;

        public RegisterFormValidationKey ServerFormKey
        {
            get => (RegisterFormValidationKey)GetSessionValue();
            set => SetSessionValue(value);
        }

        public OrganizationState Organization { get; set; }

        public string OrganizationLogoHtml
        {
            get
            {
                if (!OrganizationLogoUrl.HasValue())
                    return Organization.Name;
                else
                    return $"<img src='{OrganizationLogoUrl}' alt='' style='max-width:408px' />";
            }
        }

        public string OrganizationLogoUrl
        {
            get
            {
                var logoUrl = string.Empty;

                if (Organization != null)
                {
                    if (Organization.PlatformCustomization.PlatformUrl.Logo.HasValue())
                        logoUrl = Organization.PlatformCustomization.PlatformUrl.Logo;

                    else if (ServiceLocator.Partition.IsE03())
                        logoUrl = "/library/images/logos/cmds.png";
                }

                return logoUrl;
            }
        }

        public string RequestedGroupName
            => HttpUtility.UrlDecode(_parameters["group"]).NullIfEmpty();

        private object GetSessionValue([CallerMemberName] string name = null)
        {
            return string.IsNullOrEmpty(name)
                ? null
                : _session[typeof(InSite.UI.Layout.Lobby.Controls.SignInBasePage) + "." + name];
        }

        public bool IsOrganizationAccountClosed(OrganizationState organization)
            => organization != null && organization.AccountStatus == AccountStatus.Closed;

        private void SetSessionValue(object value, [CallerMemberName] string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                _session[typeof(InSite.UI.Layout.Lobby.Controls.SignInBasePage) + "." + name] = value;
        }
    }
}