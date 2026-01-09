using System;

using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Read
{
    public interface IOrganizationStore
    {
        Guid StartTransaction(Guid organizationId);
        void CancelTransaction(Guid transactionId);
        void CommitTransaction(Guid transactionId);

        void Update(OrganizationAccountSettingsModified e);
        void Update(OrganizationAchievementSettingsModified e);
        void Update(OrganizationAdministratorModified e);
        void Update(OrganizationAnnouncementModified e);
        void Update(OrganizationAssessmentSettingsModified e);
        void Update(OrganizationAutomaticCompetencyExpirationModified e);
        void Update(OrganizationClosed e);
        void Update(OrganizationContactSettingsModified e);
        void Insert(OrganizationCreated e);
        void Delete(OrganizationDeleted e);
        void Update(OrganizationDescriptionModified e);
        void Update(OrganizationEventSettingsModified e);
        void Update(OrganizationFieldsModified e);
        void Update(OrganizationGlossaryModified e);
        void Update(OrganizationGradebookSettingsModified e);
        void Update(OrganizationIdentificationModified e);
        void Update(OrganizationIntegrationSettingsModified e);
        void Update(OrganizationIssueSettingsModified e);
        void Update(OrganizationLocalizationModified e);
        void Update(OrganizationLocationModified e);
        void Update(OrganizationNCSHASettingsModified e);
        void Update(OrganizationOpened e);
        void Update(OrganizationParentModified e);
        void Update(OrganizationPlatformSettingsModified e);
        void Update(OrganizationPlatformUrlModified e);
        void Update(OrganizationPortalSettingsModified e);
        void Update(OrganizationRegistrationSettingsModified e);
        void Update(OrganizationSalesSettingsModified e);
        void Update(OrganizationSecretModified e);
        void Update(OrganizationSignInModified e);
        void Update(OrganizationSiteSettingsModified e);
        void Update(OrganizationStandardContentLabelsModified e);
        void Update(OrganizationStandardSettingsModified e);
        void Update(OrganizationSurveySettingsModified e);
        void Update(OrganizationTypeModified e);
        void Update(OrganizationUploadSettingsModified e);
        void Update(OrganizationUrlsModified e);
    }
}