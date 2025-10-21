using System;

using InSite.Common.Web.UI;
using InSite.Domain.Accounts.Tenants.Models;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfigurationPlatform : BaseUserControl
    {
        public void SetInputValues(OrganizationState organization)
        {
            var custom = organization.PlatformCustomization;

            SelfRegistrationStatus.Value = organization.PlatformCustomization.UserRegistration.RegistrationMode.ToString();
            AutomaticApproval.Checked = organization.PlatformCustomization.UserRegistration.AutomaticApproval;
            ConvertProvinceAbbreviation.Checked = organization.PlatformCustomization.UserRegistration.ConvertProvinceAbbreviation;
            LockPublishedQuestions.ValueAsBoolean = organization.Toolkits.Assessments.LockPublishedQuestions;
            StandardContentLabels.Text = organization.StandardContentLabels;
            AutomaticCompetencyExpirationType.Value = custom.AutomaticCompetencyExpiration.Type.GetName();
            AutomaticCompetencyExpirationMonth.Text = custom.AutomaticCompetencyExpiration.Month.HasValue ? custom.AutomaticCompetencyExpiration.Month.ToString() : string.Empty;
            AutomaticCompetencyExpirationDay.Text = custom.AutomaticCompetencyExpiration.Day.HasValue ? custom.AutomaticCompetencyExpiration.Day.ToString() : string.Empty;

            BindStandardContentLabels(organization.Identifier);
        }

        public void GetInputValues(OrganizationState organization)
        {
            var custom = organization.PlatformCustomization;

            var userRegistration = custom.UserRegistration;
            userRegistration.RegistrationMode = SelfRegistrationStatus.Value.ToEnum<UserRegistrationMode>();
            userRegistration.AutomaticApproval = AutomaticApproval.Checked;
            userRegistration.ConvertProvinceAbbreviation = ConvertProvinceAbbreviation.Checked;

            organization.Toolkits.Assessments.LockPublishedQuestions = LockPublishedQuestions.ValueAsBoolean ?? false;
            organization.StandardContentLabels = StandardContentLabels.Text;
            custom.AutomaticCompetencyExpiration.Type = AutomaticCompetencyExpirationType.Value.ToEnum<OrganizationExpirationType>();

            if (custom.AutomaticCompetencyExpiration.Type == OrganizationExpirationType.Date)
            {
                int temp;

                if (string.IsNullOrEmpty(AutomaticCompetencyExpirationMonth.Text))
                    custom.AutomaticCompetencyExpiration.Month = null;
                else if (int.TryParse(AutomaticCompetencyExpirationMonth.Text, out temp))
                    custom.AutomaticCompetencyExpiration.Month = temp;

                if (string.IsNullOrEmpty(AutomaticCompetencyExpirationDay.Text))
                    custom.AutomaticCompetencyExpiration.Day = null;
                else if (int.TryParse(AutomaticCompetencyExpirationDay.Text, out temp))
                    custom.AutomaticCompetencyExpiration.Day = temp;
            }
            else
            {
                custom.AutomaticCompetencyExpiration.Month = null;
                custom.AutomaticCompetencyExpiration.Day = null;
            }
        }

        private void BindStandardContentLabels(Guid organizationId)
        {
            var counts = ServiceLocator.ContentSearch.GetStandardContentLabelCounts(organizationId);

            StandardContentLabelsRepeater.Visible = counts.Count > 0;
            StandardContentLabelsRepeater.DataSource = counts;
            StandardContentLabelsRepeater.DataBind();
        }
    }
}