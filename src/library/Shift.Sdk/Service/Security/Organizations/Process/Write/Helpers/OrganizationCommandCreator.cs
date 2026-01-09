using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Write
{
    public static class OrganizationCommandCreator
    {
        public static List<ICommand> Create(OrganizationState state, bool isNew)
        {
            var result = new List<ICommand>();

            if (isNew)
                result.Add(new CreateOrganization(state.OrganizationIdentifier, null));

            result.Add(new ModifyOrganizationAccountSettings(state.OrganizationIdentifier, state.Toolkits.Accounts));
            result.Add(new ModifyOrganizationAchievementSettings(state.OrganizationIdentifier, state.Toolkits.Achievements));
            result.Add(new ModifyOrganizationAdministrator(state.OrganizationIdentifier, state.AdministratorUserIdentifier, state.AdministratorGroupIdentifier));
            result.Add(new ModifyOrganizationAnnouncement(state.OrganizationIdentifier, state.AccountWarning));
            result.Add(new ModifyOrganizationAssessmentSettings(state.OrganizationIdentifier, state.Toolkits.Assessments));
            result.Add(new ModifyOrganizationAutomaticCompetencyExpiration(state.OrganizationIdentifier, state.PlatformCustomization.AutomaticCompetencyExpiration));
            result.Add(new ModifyOrganizationContactSettings(state.OrganizationIdentifier, state.Toolkits.Contacts));
            result.Add(new ModifyOrganizationDescription(state.OrganizationIdentifier, state.CompanyDescription));
            result.Add(new ModifyOrganizationEventSettings(state.OrganizationIdentifier, state.Toolkits.Events));
            result.Add(new ModifyOrganizationFields(state.OrganizationIdentifier, state.Fields));
            result.Add(new ModifyOrganizationGlossary(state.OrganizationIdentifier, state.GlossaryIdentifier));
            result.Add(new ModifyOrganizationGradebookSettings(state.OrganizationIdentifier, state.Toolkits.Gradebooks));
            result.Add(new ModifyOrganizationIdentification(state.OrganizationIdentifier, state.OrganizationCode, state.CompanyName, state.CompanyDomain));
            result.Add(new ModifyOrganizationIntegrationSettings(state.OrganizationIdentifier, state.Integrations));
            result.Add(new ModifyOrganizationIssueSettings(state.OrganizationIdentifier, state.Toolkits.Issues));
            result.Add(new ModifyOrganizationLocalization(state.OrganizationIdentifier, state.Languages.Select(x => x.TwoLetterISOLanguageName).ToArray(), state.TimeZone.Id));
            result.Add(new ModifyOrganizationLocation(state.OrganizationIdentifier, state.PlatformCustomization.TenantLocation));
            result.Add(new ModifyOrganizationNCSHASettings(state.OrganizationIdentifier, state.Toolkits.NCSHA));
            result.Add(new ModifyOrganizationParent(state.OrganizationIdentifier, state.ParentOrganizationIdentifier));
            result.Add(new ModifyOrganizationPlatformSettings(
                state.OrganizationIdentifier,
                state.PlatformCustomization.InlineInstructionsUrl,
                state.PlatformCustomization.InlineLabelsUrl,
                state.PlatformCustomization.SafeExamBrowserUserAgentSuffix));
            result.Add(new ModifyOrganizationPlatformUrl(state.OrganizationIdentifier, state.PlatformCustomization.PlatformUrl));
            result.Add(new ModifyOrganizationPortalSettings(state.OrganizationIdentifier, state.Toolkits.Portal));
            result.Add(new ModifyOrganizationRegistrationSettings(state.OrganizationIdentifier, state.PlatformCustomization.UserRegistration));
            result.Add(new ModifyOrganizationSalesSettings(state.OrganizationIdentifier, state.Toolkits.Sales));
            result.Add(new ModifyOrganizationSecret(state.OrganizationIdentifier, state.OrganizationSecret));
            result.Add(new ModifyOrganizationSignIn(state.OrganizationIdentifier, state.PlatformCustomization.SignIn));
            result.Add(new ModifyOrganizationSiteSettings(state.OrganizationIdentifier, state.Toolkits.Sites));
            result.Add(new ModifyOrganizationStandardContentLabels(state.OrganizationIdentifier, state.GetStandardContentLabels()));
            result.Add(new ModifyOrganizationStandardSettings(state.OrganizationIdentifier, state.Toolkits.Standards));
            result.Add(new ModifyOrganizationSurveySettings(state.OrganizationIdentifier, state.Toolkits.Surveys));
            result.Add(new ModifyOrganizationType(state.OrganizationIdentifier, state.OrganizationType));
            result.Add(new ModifyOrganizationUploadSettings(state.OrganizationIdentifier, state.PlatformCustomization.UploadSettings));
            result.Add(new ModifyOrganizationUrls(state.OrganizationIdentifier, state.PlatformCustomization.TenantUrl));

            return result;
        }
    }
}
