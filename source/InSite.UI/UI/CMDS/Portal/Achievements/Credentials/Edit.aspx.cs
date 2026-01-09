using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.User.Progressions.Controls;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.User.Progressions.Forms
{
    public partial class Edit : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        #region Security

        private bool CanBeSigned
        {
            set => ViewState[nameof(CanBeSigned)] = value;
            get => (bool?)ViewState[nameof(CanBeSigned)] ?? false;
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            var hasValidationAccess = Access.Administrate || Access.Configure;

            Access = Access.SetWrite(hasValidationAccess);

            AchievementDetails.ApplySecurityPermissions(Access.Write, hasValidationAccess, hasValidationAccess);

            if (!Access.Write)
                Access = Access.SetWrite(Access.Configure);

            CanBeSigned = Access.Write || UserIdentifier == User.UserIdentifier;

            DeleteButton.Visible = CanDelete && hasValidationAccess;
        }

        #endregion

        #region Properties

        private string SearchUrl => $"/ui/cmds/portal/achievements/credentials/search?userid={UserIdentifier}";

        private Guid? AchievementIdentifier => Guid.TryParse(Request["achievement"], out var id) && id != Guid.Empty ? id : (Guid?)null;

        private Guid? UserIdentifier => Guid.TryParse(Request["user"], out var id) && id != Guid.Empty ? id : (Guid?)null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementDetails.SignedOff += AchievementSummary_SignedOff;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                NewButton.NavigateUrl = $"/ui/cmds/portal/achievements/credentials/create?userid={UserIdentifier}";
                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        #endregion

        #region Event handlers

        private void AchievementSummary_SignedOff(object sender, EventArgs e)
        {
            Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            if (Save())
            {
                Open();
                SetStatus(EditorStatus, StatusType.Saved);
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            EmployeeAchievementHelper.DeleteUserAchievement(UserIdentifier.Value, AchievementIdentifier.Value);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        #endregion

        #region Load and save

        private void Open()
        {
            var searchUrl = "/ui/cmds/portal/achievements/credentials/search";
            var searchUrlWithUser = $"{searchUrl}?userid={UserIdentifier}";

            if (UserIdentifier == null || AchievementIdentifier == null)
                HttpResponseHelper.Redirect(searchUrl, true);

            var userName = UserSearch.GetFullName(UserIdentifier.Value);
            if (userName == UserNames.Someone)
                HttpResponseHelper.Redirect(searchUrl, true);

            var credential = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == UserIdentifier && x.AchievementIdentifier == AchievementIdentifier);
            if (credential == null)
                HttpResponseHelper.Redirect(searchUrlWithUser, true);

            PageHelper.AutoBindHeader(this, null, userName);

            var locationParts = !string.IsNullOrEmpty(credential.AuthorityLocation)
                ? credential.AuthorityLocation.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : null;

            var authorityCity = locationParts != null && locationParts.Length >= 1 ? locationParts[0].Trim() : null;
            var authorityProvince = locationParts != null && locationParts.Length >= 2 ? locationParts[1].Trim() : null;
            var authorityCountry = locationParts != null && locationParts.Length >= 3 ? locationParts[2].Trim() : null;

            var item = new EmployeeAchievementDetails.EducationItem
            {
                Type = EducationItemType.Credential,
                CredentialAchievement = credential.AchievementIdentifier,
                CredentialAuthorityReference = credential.AuthorityReference,
                CredentialIsRequired = credential.CredentialIsMandatory,
                CredentialAssigned = credential.CredentialAssigned,
                CredentialExpirationReminderDelivered0 = credential.CredentialExpirationReminderDelivered0,
                CredentialExpirationReminderDelivered1 = credential.CredentialExpirationReminderDelivered1,
                CredentialExpirationReminderDelivered2 = credential.CredentialExpirationReminderDelivered2,
                CredentialExpirationReminderDelivered3 = credential.CredentialExpirationReminderDelivered3,
                CredentialIsInTrainingPlan = credential.IsInTrainingPlan,
                AchievementType = credential.AchievementLabel,
                AchievementTitle = credential.AchievementTitle,
                AchievementDescription = credential.AchievementDescription,
                Hours = credential.CredentialHours,
                AuthorityIdentifier = credential.AuthorityIdentifier,
                AuthorityName = credential.AuthorityName,
                AuthorityType = credential.AuthorityType,
                AuthorityCity = authorityCity,
                AuthorityProvince = authorityProvince,
                AuthorityCountry = authorityCountry,
                Completed = TimeZones.GetDate(credential.CredentialGranted, User.TimeZone),
                Description = credential.CredentialDescription,
                LifetimeMonths = credential.CredentialExpirationLifetimeQuantity,
                Status = credential.CredentialStatus,
                Expired = TimeZones.GetDate(credential.CredentialExpirationExpected, User.TimeZone),
            };

            AchievementDetails.SetInputValues(item, credential.UserIdentifier, credential.UserIdentifier, CanBeSigned, credential.CredentialIdentifier, UploadContainerType.Workflow);
            var save = Access.Write && VCmdsAchievementSearch.Count(x => x.AchievementIdentifier == credential.AchievementIdentifier) > 0;

            SaveButton.Visible = save;
        }

        private bool Save()
        {
            var credential = ServiceLocator.AchievementSearch.GetCredential(AchievementIdentifier.Value, UserIdentifier.Value);
            if (credential == null)
                HttpResponseHelper.Redirect("/ui/cmds/portal/achievements/credentials/search", true);

            var item = new EmployeeAchievementDetails.EducationItem();

            AchievementDetails.GetInputValues(item);

            if (!item.Completed.HasValue && item.Status == "Valid")
            {
                EditorStatus.AddMessage(AlertType.Error, "Please select the date Completed for this training.");
                return false;
            }

            if (item.Completed.HasValue && item.Status == "Pending")
            {
                EditorStatus.AddMessage(AlertType.Error, "If this training is not yet Completed then please clear the Completed field. If it is Completed then please select Valid for the current Status.");
                return false;
            }

            var commands = new List<Command>();

            var expiration = new Expiration(credential.CredentialExpirationType, credential.CredentialExpirationFixedDate, credential.CredentialExpirationLifetimeQuantity, credential.CredentialExpirationLifetimeUnit);

            if (item.LifetimeMonths.HasValue
                && (credential.CredentialExpirationType != ExpirationType.Relative.ToString()
                    || credential.CredentialExpirationLifetimeUnit != "Month"
                    || credential.CredentialExpirationLifetimeQuantity != item.LifetimeMonths
                )
                || item.LifetimeMonths == null && credential.CredentialExpirationLifetimeUnit != null
            )
            {
                expiration = item.LifetimeMonths.HasValue
                    ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = item.LifetimeMonths.Value, Unit = "Month" } }
                    : null;

                commands.Add(new ChangeCredentialExpiration(credential.CredentialIdentifier, expiration));
            }

            if (item.CredentialIsRequired != (credential.CredentialNecessity == "Mandatory")
             || item.CredentialIsInTrainingPlan != (credential.CredentialPriority == "Planned")
            )
            {
                var necessity = item.CredentialIsRequired ? "Mandatory" : "Optional";
                var priority = item.CredentialIsInTrainingPlan ? "Planned" : "Unplanned";
                commands.Add(new TagCredential(credential.CredentialIdentifier, necessity, priority));
            }

            var completed = item.Completed.ToDateTimeOffset(TimeSpan.Zero);
            var authorityLocation = (item.AuthorityCity ?? "") + ", " + (item.AuthorityProvince ?? "") + ", " + (item.AuthorityCountry ?? "");

            var isChanged =
                   !string.Equals(item.AuthorityName, credential.AuthorityName)
                || !string.Equals(item.AuthorityType, credential.AuthorityType)
                || !string.Equals(authorityLocation, credential.AuthorityLocation)
                || !string.Equals(item.CredentialAuthorityReference, credential.AuthorityReference)
                || item.Hours != credential.CredentialHours;

            if (isChanged)
            {
                commands.Add(new ChangeCredentialAuthority(
                    credential.CredentialIdentifier,
                    item.AuthorityIdentifier,
                    item.AuthorityName,
                    item.AuthorityType,
                    authorityLocation,
                    item.CredentialAuthorityReference,
                    item.Hours));
            }

            if (!string.Equals(item.Description, credential.CredentialDescription))
                commands.Add(new DescribeCredential(credential.CredentialIdentifier, item.Description));

            var expectedExpiry = CredentialState.CalculateExpectedExpiry(expiration, completed);

            if (completed.HasValue)
            {
                var now = DateTimeOffset.Now;

                if (credential.CredentialGranted != completed || (credential.CredentialStatus != "Valid" && item.Status == "Valid"))
                {
                    var person = ServiceLocator.ContactSearch.GetPerson(credential.UserIdentifier, credential.OrganizationIdentifier);
                    commands.Add(new GrantCredential(credential.CredentialIdentifier, completed ?? DateTimeOffset.UtcNow, null, null, person?.EmployerGroupIdentifier, person?.EmployerGroupStatus));
                }

                if (expectedExpiry != null && expectedExpiry < now)
                {
                    if (credential.CredentialExpirationExpected == null || credential.CredentialExpirationExpected != expectedExpiry || credential.CredentialStatus != "Expired")
                        commands.Add(new ExpireCredential(credential.CredentialIdentifier, expectedExpiry.Value));
                }
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            return true;
        }

        private bool VarianceExceedsOneDay(DateTimeOffset? a, DateTimeOffset? b)
        {
            // Check for nulls.
            if (a.HasValue && !b.HasValue)
                return true;
            if (!a.HasValue && b.HasValue)
                return true;
            if (!a.HasValue && !b.HasValue)
                return false;

            // Count the number of hours between a and b.
            var x = a.Value;
            var y = b.Value;
            return Math.Abs((x - y).TotalHours) > 24;
        }

        #endregion

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/search") ? $"userid={UserIdentifier}" : null;
    }
}