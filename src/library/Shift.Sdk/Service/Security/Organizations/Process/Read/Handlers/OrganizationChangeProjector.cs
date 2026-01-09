using Shift.Common.Timeline.Changes;

using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Read
{
    public class OrganizationChangeSubscriber
    {
        private readonly IOrganizationStore _organizationStore;

        public OrganizationChangeSubscriber(IChangeQueue publisher, IOrganizationStore organizationStore)
        {
            _organizationStore = organizationStore;

            publisher.Subscribe<OrganizationAccountSettingsModified>(Handle);
            publisher.Subscribe<OrganizationAchievementSettingsModified>(Handle);
            publisher.Subscribe<OrganizationAdministratorModified>(Handle);
            publisher.Subscribe<OrganizationAnnouncementModified>(Handle);
            publisher.Subscribe<OrganizationAssessmentSettingsModified>(Handle);
            publisher.Subscribe<OrganizationAutomaticCompetencyExpirationModified>(Handle);
            publisher.Subscribe<OrganizationClosed>(Handle);
            publisher.Subscribe<OrganizationContactSettingsModified>(Handle);
            publisher.Subscribe<OrganizationCreated>(Handle);
            publisher.Subscribe<OrganizationDeleted>(Handle);
            publisher.Subscribe<OrganizationDescriptionModified>(Handle);
            publisher.Subscribe<OrganizationEventSettingsModified>(Handle);
            publisher.Subscribe<OrganizationFieldsModified>(Handle);
            publisher.Subscribe<OrganizationGlossaryModified>(Handle);
            publisher.Subscribe<OrganizationGradebookSettingsModified>(Handle);
            publisher.Subscribe<OrganizationIdentificationModified>(Handle);
            publisher.Subscribe<OrganizationIntegrationSettingsModified>(Handle);
            publisher.Subscribe<OrganizationIssueSettingsModified>(Handle);
            publisher.Subscribe<OrganizationLocalizationModified>(Handle);
            publisher.Subscribe<OrganizationLocationModified>(Handle);
            publisher.Subscribe<OrganizationNCSHASettingsModified>(Handle);
            publisher.Subscribe<OrganizationOpened>(Handle);
            publisher.Subscribe<OrganizationParentModified>(Handle);
            publisher.Subscribe<OrganizationPlatformSettingsModified>(Handle);
            publisher.Subscribe<OrganizationPlatformUrlModified>(Handle);
            publisher.Subscribe<OrganizationPortalSettingsModified>(Handle);
            publisher.Subscribe<OrganizationRegistrationSettingsModified>(Handle);
            publisher.Subscribe<OrganizationSalesSettingsModified>(Handle);
            publisher.Subscribe<OrganizationSecretModified>(Handle);
            publisher.Subscribe<OrganizationSignInModified>(Handle);
            publisher.Subscribe<OrganizationSiteSettingsModified>(Handle);
            publisher.Subscribe<OrganizationStandardContentLabelsModified>(Handle);
            publisher.Subscribe<OrganizationStandardSettingsModified>(Handle);
            publisher.Subscribe<OrganizationSurveySettingsModified>(Handle);
            publisher.Subscribe<OrganizationTypeModified>(Handle);
            publisher.Subscribe<OrganizationUploadSettingsModified>(Handle);
            publisher.Subscribe<OrganizationUrlsModified>(Handle);
        }

        public void Handle(OrganizationAccountSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationAchievementSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationAdministratorModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationAnnouncementModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationAssessmentSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationAutomaticCompetencyExpirationModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationClosed e) => _organizationStore.Update(e);

        public void Handle(OrganizationContactSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationCreated e) => _organizationStore.Insert(e);

        public void Handle(OrganizationDeleted e) => _organizationStore.Delete(e);

        public void Handle(OrganizationDescriptionModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationEventSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationFieldsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationGlossaryModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationGradebookSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationIdentificationModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationIntegrationSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationIssueSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationLocalizationModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationLocationModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationNCSHASettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationOpened e) => _organizationStore.Update(e);

        public void Handle(OrganizationParentModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationPlatformSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationPlatformUrlModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationPortalSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationRegistrationSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationSalesSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationSecretModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationSignInModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationSiteSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationStandardContentLabelsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationStandardSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationSurveySettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationTypeModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationUploadSettingsModified e) => _organizationStore.Update(e);

        public void Handle(OrganizationUrlsModified e) => _organizationStore.Update(e);
    }
}