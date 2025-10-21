using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;

using Shift.Common;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationBambora : BaseUserControl
    {
        protected string BamboraMerchantId => ServiceLocator.AppSettings.Integration.Bambora.TestAccount.MerchantId;

        protected string BamboraPasscode => ServiceLocator.AppSettings.Integration.Bambora.TestAccount.Passcode;

        public void SetInputValues(OrganizationState organization)
        {
            var bambora = organization.Integrations?.Bambora;
            if (bambora?.Enabled != true)
                return;

            var dev = bambora.Get(EnvironmentName.Development);
            DevMerchant.Text = dev?.Merchant;
            DevPasscode.Text = dev?.Passcode;

            var sandbox = bambora.Get(EnvironmentName.Sandbox);
            SandboxMerchant.Text = sandbox?.Merchant;
            SandboxPasscode.Text = sandbox?.Passcode;

            var prod = bambora.Get(EnvironmentName.Production);
            ProdMerchant.Text = prod?.Merchant;
            ProdPasscode.Text = prod?.Passcode;

            SetDefaultTextBoxValue(ServiceLocator.AppSettings.Integration.Bambora.TestAccount.MerchantId, DevMerchant, SandboxMerchant, ProdMerchant);
            SetDefaultTextBoxValue(ServiceLocator.AppSettings.Integration.Bambora.TestAccount.Passcode, DevPasscode, SandboxPasscode, ProdPasscode);
        }

        private static void SetDefaultTextBoxValue(string defaultValue, params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                if (textBox.Text.IsEmpty())
                    textBox.Text = defaultValue;
            }
        }

        public void GetInputValues(OrganizationState organization)
        {
            var endpoints = new List<BamboraEndpoint>();

            if (DevMerchant.Text.HasValue() && DevPasscode.Text.HasValue())
                endpoints.Add(new BamboraEndpoint { Environments = new[] { EnvironmentName.Development, EnvironmentName.Local }, Merchant = DevMerchant.Text, Passcode = DevPasscode.Text });

            if (SandboxMerchant.Text.HasValue() && SandboxPasscode.Text.HasValue())
                endpoints.Add(new BamboraEndpoint { Environments = new[] { EnvironmentName.Sandbox }, Merchant = SandboxMerchant.Text, Passcode = SandboxPasscode.Text });

            if (ProdMerchant.Text.HasValue() && ProdPasscode.Text.HasValue())
                endpoints.Add(new BamboraEndpoint { Environments = new[] { EnvironmentName.Production }, Merchant = ProdMerchant.Text, Passcode = ProdPasscode.Text });

            if (organization.Integrations == null)
                organization.Integrations = new OrganizationIntegrations();

            organization.Integrations.Bambora = endpoints.Count > 0
                ? new BamboraIntegration { Enabled = true, Endpoints = endpoints.ToArray() }
                : new BamboraIntegration { Enabled = false };
        }
    }
}