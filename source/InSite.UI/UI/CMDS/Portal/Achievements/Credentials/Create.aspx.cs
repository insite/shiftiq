using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.User.Progressions.Controls;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.User.Progressions.Forms
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        private const string SearchUrl = "/ui/cmds/portal/achievements/credentials/search";

        private Guid ContainerGuid => (Guid)(ViewState[nameof(ContainerGuid)]
            ?? (ViewState[nameof(ContainerGuid)] = UniqueIdentifier.Create()));

        private Guid PersonID => Guid.TryParse(Request["userID"], out var personID) ? personID : User.UserIdentifier;

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (!Access.Configure && !Access.Administrate)
                HttpResponseHelper.Redirect(SearchUrl);

            var hasValidationAccess = Access.Configure || Access.Administrate;
            var canEditGrades = Access.Configure;

            AchievementDetails.ApplySecurityPermissions(true, canEditGrades, hasValidationAccess);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var user = UserSearch.Bind(PersonID, x => new { x.FullName });

                PageHelper.AutoBindHeader(this, null, user.FullName);

                AchievementDetails.SetDefaultInputValues(PersonID, ContainerGuid);

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var item = new EmployeeAchievementDetails.EducationItem();

            AchievementDetails.GetInputValues(item);

            if (!item.Completed.HasValue && item.Status == "Valid")
            {
                EditorStatus.AddMessage(AlertType.Error, "Please select the date Completed for this training.");
                return;
            }

            if (item.Completed.HasValue && item.Status == "Pending")
            {
                EditorStatus.AddMessage(AlertType.Error, "If this training is not yet Completed then please clear the Completed field. If it is Completed then please select Valid for the current Status.");
                return;
            }

            Guid entityThumbprint;
            string uploadContainerType;
            string redirectUrl;

            if (item.Type == EducationItemType.Credential)
            {
                var userIdentifier = UserSearch.Select(PersonID).UserIdentifier;
                var credentialIdentifier = ServiceLocator.AchievementSearch.GetCredentialIdentifier(null, item.CredentialAchievement.Value, PersonID);

                var error = CreateCredential(PersonID, credentialIdentifier, item);
                if (error != null)
                {
                    EditorStatus.AddMessage(AlertType.Error, error);
                    return;
                }

                entityThumbprint = credentialIdentifier;
                uploadContainerType = UploadContainerType.Workflow;

                redirectUrl = $"/ui/cmds/portal/achievements/credentials/edit?achievement={item.CredentialAchievement}&user={userIdentifier}&status=saved";
            }
            else
            {
                var contactExperience = CreateContactExperience(PersonID, item);

                entityThumbprint = contactExperience.ExperienceIdentifier;
                uploadContainerType = UploadContainerType.Workflow;

                redirectUrl = $"/ui/cmds/portal/achievements/experiences/edit?id={contactExperience.ExperienceIdentifier}&status=saved";
            }

            var names = UploadSearch.Bind(x => x.Name, x => x.ContainerIdentifier == ContainerGuid && x.UploadType == UploadType.CmdsFile);
            foreach (var name in names)
                CmdsUploadProvider.Current.Move(ContainerGuid, name, entityThumbprint, name, uploadContainerType);

            HttpResponseHelper.Redirect(redirectUrl);
        }

        private static ContactExperience CreateContactExperience(Guid userKey, EmployeeAchievementDetails.EducationItem item)
        {
            var contactExperience = new ContactExperience();

            contactExperience.ContactExperienceType = item.AchievementType;
            contactExperience.CreditHours = item.Hours;
            contactExperience.Title = item.AchievementTitle;
            contactExperience.AuthorityName = item.AuthorityName;
            contactExperience.AuthorityCity = item.AuthorityCity;
            contactExperience.AuthorityProvince = item.AuthorityProvince;
            contactExperience.AuthorityCountry = item.AuthorityCountry;
            contactExperience.Status = item.Status;
            contactExperience.Completed = item.Completed;
            contactExperience.Description = item.Description;
            contactExperience.LifetimeMonths = item.LifetimeMonths;
            contactExperience.Score = item.ContactExperienceScore;
            contactExperience.IsSuccess = item.ContactExperienceIsSuccess;
            contactExperience.Expired = item.Expired;

            contactExperience.UserIdentifier = userKey;
            contactExperience.ExperienceIdentifier = UniqueIdentifier.Create();

            ContactExperienceStore.Insert(contactExperience);

            return contactExperience;
        }

        public static string CreateCredential(Guid userKey, Guid credentialIdentifier, EmployeeAchievementDetails.EducationItem item)
        {
            var userIdentifier = userKey;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(item.CredentialAchievement.Value);
            var lifetime = item.LifetimeMonths;
            if (lifetime == null && achievement.ExpirationLifetimeQuantity.HasValue && achievement.ExpirationLifetimeUnit != null)
            {
                if (achievement.ExpirationLifetimeQuantity.Value > 0)
                {
                    if (StringHelper.Equals(achievement.ExpirationLifetimeUnit, "Month"))
                        lifetime = achievement.ExpirationLifetimeQuantity;

                    if (StringHelper.Equals(achievement.ExpirationLifetimeUnit, "Year"))
                        lifetime = achievement.ExpirationLifetimeQuantity * 12;
                }
            }

            var credential = ServiceLocator.AchievementSearch.GetCredential(item.CredentialAchievement.Value, userIdentifier);
            if (credential != null)
                return $"The specified achievement is already <a href='/ui/cmds/portal/achievements/credentials/edit?user={userIdentifier}&achievement={item.CredentialAchievement}'>assigned</a> to this person.";

            var expiration = lifetime.HasValue
                ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = lifetime.Value, Unit = "Month" } }
                : null;

            var necessity = item.CredentialIsRequired ? "Mandatory" : "Optional";
            var priority = item.CredentialIsInTrainingPlan ? "Planned" : "Unplanned";
            var authorityLocation = (item.AuthorityCity ?? "") + ", " + (item.AuthorityProvince ?? "") + ", " + (item.AuthorityCountry ?? "");

            var commands = new List<Command>
            {
                new CreateCredential(credentialIdentifier, Organization.OrganizationIdentifier, item.CredentialAchievement.Value, userIdentifier, DateTimeOffset.UtcNow),
                new ChangeCredentialExpiration(credentialIdentifier, expiration),
                new TagCredential(credentialIdentifier, necessity, priority),
                new ChangeCredentialAuthority(credentialIdentifier, item.AuthorityIdentifier, item.AuthorityName, item.AuthorityType, authorityLocation, item.CredentialAuthorityReference, item.Hours)
            };

            if (item.Status == "Valid")
            {
                var organization = credential?.OrganizationIdentifier ?? Organization.OrganizationIdentifier;
                var person = ServiceLocator.ContactSearch.GetPerson(userIdentifier, organization);
                var grant = new GrantCredential(credentialIdentifier, item.Completed ?? DateTimeOffset.UtcNow, null, null, person?.EmployerGroupIdentifier, person?.EmployerGroupStatus);
                commands.Add(grant);
            }

            if (!string.IsNullOrEmpty(item.Description))
                commands.Add(new DescribeCredential(credentialIdentifier, item.Description));

            if (item.Status == "Expired")
                commands.Add(new ExpireCredential(credentialIdentifier, DateTimeOffset.UtcNow));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            return null;
        }
    }
}
