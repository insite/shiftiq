using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.Cmds;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Custom.CMDS.User.Progressions.Controls
{
    public partial class EmployeeAchievementDetails : UserControl
    {
        #region Classes

        public class EducationItem
        {
            public EducationItemType Type { get; set; }

            public Guid? CredentialAchievement { get; set; }
            public string CredentialAuthorityReference { get; set; } // CreditIdentifier
            public bool CredentialIsRequired { get; set; }
            public DateTimeOffset? CredentialAssigned { get; set; }
            public DateTimeOffset? CredentialExpirationReminderDelivered0 { get; set; }
            public DateTimeOffset? CredentialExpirationReminderDelivered1 { get; set; }
            public DateTimeOffset? CredentialExpirationReminderDelivered2 { get; set; }
            public DateTimeOffset? CredentialExpirationReminderDelivered3 { get; set; }
            public bool CredentialIsInTrainingPlan { get; set; }

            public decimal? ContactExperienceScore { get; set; }
            public bool ContactExperienceIsSuccess { get; set; }

            public string AchievementType { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementDescription { get; set; }
            public decimal? Hours { get; set; }
            public Guid? AuthorityIdentifier { get; set; }
            public string AuthorityName { get; set; }
            public string AuthorityType { get; set; }
            public string AuthorityCity { get; set; }
            public string AuthorityProvince { get; set; }
            public string AuthorityCountry { get; set; }
            public DateTime? Completed { get; set; }
            public string Description { get; set; }
            public int? LifetimeMonths { get; set; }
            public string Status { get; set; }
            public DateTime? Expired { get; set; }
        }

        #endregion

        #region Events

        public event EventHandler SignedOff;
        private void OnSignedOff()
        {
            SignedOff?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Methods (security)

        public void ApplySecurityPermissions(bool canEdit, bool canEditGrades, bool hasValidationAccess)
        {
            HasValidationAccess = hasValidationAccess;

            var isImpersonation = CurrentSessionState.Identity.IsImpersonating;

            SubType.Enabled = hasValidationAccess && !isImpersonation;
            AchievementSelector.Enabled = hasValidationAccess && !isImpersonation;
            IsTimeSensitiveYes.Enabled = hasValidationAccess;
            IsTimeSensitiveNo.Enabled = hasValidationAccess;

            DateCompleted.Enabled = hasValidationAccess && !isImpersonation;
            ValidationStatus.Enabled = hasValidationAccess && !isImpersonation;

            IsInTrainingPlan.Enabled = hasValidationAccess;
            EnableSignOff.Enabled = canEditGrades && !isImpersonation;

            GradePercent.Enabled = canEditGrades;

            DownloadGrid.AllowEdit = canEdit;

            Title.Enabled = hasValidationAccess;
            IsRequired.Enabled = hasValidationAccess;
            Hours.Enabled = hasValidationAccess;
            Number.Enabled = hasValidationAccess;
            Comment.Enabled = hasValidationAccess;

            AccreditorName.Enabled = canEdit;
            AccreditorCity.Enabled = canEdit;
            AccreditorProvince.Enabled = canEdit;
            AccreditorCountry.Enabled = canEdit;

            var webUser = CurrentSessionState.Identity;
            var canEditTimeSensitivity = webUser.IsInRole(CmdsRole.Programmers)
                                         || webUser.IsInRole(CmdsRole.SystemAdministrators)
                                         || webUser.IsInRole(CmdsRole.OfficeAdministrators)
                                         || webUser.IsInRole(CmdsRole.FieldAdministrators);

            TimeSensitivePanel.Visible = canEditTimeSensitivity; // && AchievementSelector.Value.HasValue;

            IsTimeSensitiveYes.Enabled = canEditTimeSensitivity && !isImpersonation;
            IsTimeSensitiveNo.Enabled = canEditTimeSensitivity && !isImpersonation;
            ValidForCount.Enabled = canEditTimeSensitivity && !isImpersonation;

            AdminPanel1.Visible = hasValidationAccess;
            AdminPanel2.Visible = hasValidationAccess;
        }

        #endregion

        #region Properties

        private Guid? EmployeeID
        {
            get => ViewState[nameof(EmployeeID)] as Guid?;
            set => ViewState[nameof(EmployeeID)] = value;
        }

        private Guid? AchievementIdentifier
        {
            get => ViewState[nameof(AchievementIdentifier)] as Guid?;
            set => ViewState[nameof(AchievementIdentifier)] = value;
        }

        private bool HasValidationAccess
        {
            get => (bool)ViewState[nameof(HasValidationAccess)];
            set => ViewState[nameof(HasValidationAccess)] = value;
        }

        private bool IsTimeSensitive
        {
            get => (ViewState[nameof(IsTimeSensitive)] as bool?) ?? false;
            set => ViewState[nameof(IsTimeSensitive)] = value;
        }

        private bool CanBeSigned
        {
            get => ViewState[nameof(CanBeSigned)] as bool? ?? false;
            set => ViewState[nameof(CanBeSigned)] = value;
        }

        #endregion

        #region Methods (initialization)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubType.ValueChanged += SubType_ValueChanged;
            SubType.AutoPostBack = true;

            AchievementSelector.AutoPostBack = true;
            AchievementSelector.ValueChanged += AchievementSelector_ValueChanged;

            UniqueAchievementValidator.ServerValidate += UniqueAchievementValidator_ServerValidate;

            ValidForCountRequired.ServerValidate += ValidForCountRequired_ServerValidate;

            AchievementSummary.SignedOff += AchievementSummary_SignedOff;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            trDateExpired.Style["display"] = IsTimeSensitiveYes.Checked ? "" : "none";
            trDateNotified.Style["display"] = IsTimeSensitiveYes.Checked ? "" : "none";
            trValidFor.Style["display"] = IsTimeSensitiveYes.Checked ? "" : "none";

            ValidationStatus.Enabled = HasValidationAccess && !CurrentSessionState.Identity.IsImpersonating;

            IsSuccessField.Visible = SubType.Value == AchievementTypes.Module && ValidationStatus.Value == EmployeeAchievementStatuses.Completed;
        }

        #endregion

        #region Methods (event handling)

        private void SubType_ValueChanged(object sender, EventArgs e)
        {
            InitSubType(null);
            ShowAchievementDetails();
            ToggleSignOff();
        }

        private void AchievementSelector_ValueChanged(object o, EventArgs e)
        {
            ShowAchievementDetails();
            ToggleSignOff();
            EnableSignOff.Checked = EnableSignOff.Enabled;
        }

        private void UniqueAchievementValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!AchievementSelector.Enabled)
                return;

            var achievement = AchievementSelector.Value;
            if (achievement.HasValue && AchievementIdentifier != achievement.Value)
            {
                var exists = VCmdsCredentialSearch.Select(EmployeeID.Value, achievement.Value) != null;
                args.IsValid = !exists;
            }
        }

        private void ValidForCountRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !IsTimeSensitiveYes.Checked || ValidForCount.ValueAsInt.HasValue;
        }

        private void AchievementSummary_SignedOff(object sender, EventArgs e)
        {
            OnSignedOff();
        }

        #endregion

        #region Methods (data binding)

        public void GetInputValues(EducationItem item)
        {
            item.Type = AchievementSelector.HasValue ? EducationItemType.Credential : EducationItemType.Experience;

            item.CredentialAchievement = AchievementSelector.Value;
            item.CredentialAuthorityReference = !string.IsNullOrEmpty(Number.Text) ? Number.Text : null;
            item.CredentialIsRequired = IsRequired.Checked;

            item.ContactExperienceScore = GradePercent.ValueAsDecimal / 100m;
            item.ContactExperienceIsSuccess = IsSuccess.Checked;

            item.AchievementType = SubType.Value.IfNullOrEmpty(AchievementTypes.OtherAchievement);

            item.AchievementTitle = Title.Text;

            item.Hours = Hours.ValueAsDecimal;
            item.AuthorityName = AccreditorName.Text;
            item.AuthorityCity = AccreditorCity.Text;
            item.AuthorityProvince = AccreditorProvince.Text;
            item.AuthorityCountry = AccreditorCountry.Text;

            if (EnableSignOff.Checked)
            {
                item.AuthorityType = "Self";
                // TODO: Set the authority identifier to the user. This means the user is the authority for granting himself the credential.
                // item.AuthorityIdentifier = item.UserIdentifier;
            }
            else
            {
                item.AuthorityType = null;
                item.AuthorityIdentifier = null;
            }

            item.Status = ValidationStatus.Value;
            item.Completed = TimeZones.GetDateUtc(DateCompleted.Value);
            item.Description = Comment.Text;
            item.LifetimeMonths = IsTimeSensitiveYes.Checked ? ValidForCount.ValueAsInt : null;
            item.CredentialIsInTrainingPlan = IsInTrainingPlan.Checked;

            if (item.Type == EducationItemType.Experience)
            {
                item.Expired = item.Completed.HasValue && item.LifetimeMonths.HasValue
                    ? TimeZones.GetDateUtc(item.Completed.Value.AddMonths(item.LifetimeMonths.Value))
                    : null;

                if (item.Expired.HasValue && item.Expired.Value < DateTime.Now)
                    item.Status = EmployeeAchievementStatuses.Expired;
            }
        }

        public void SetDefaultInputValues(Guid contactEmployeeID, Guid containerGuid)
        {
            EmployeeID = contactEmployeeID;

            ValidationStatus.Value = "Pending";
            ExpirationDate.Text = "None";

            SubType.EnsureDataBound();

            AchievementSelector.Filter.AchievementType = SubType.Value;
            AchievementSelector.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            AchievementSelector.Filter.GlobalOrCompanySpecific = SubType.Value == "Module";
            AchievementSelector.Value = null;

            DownloadGrid.LoadData(containerGuid, "None");

            ToggleSignOff();

            AssignedField.Visible = false;

            AchievementSummary.HidePanels();
        }

        public void SetInputValues(EducationItem item, Guid userKey, Guid userIdentifier, bool canBeSigned, Guid containerGuid, string containerType)
        {
            var user = CurrentSessionState.Identity.User;

            EmployeeID = userKey;
            CanBeSigned = canBeSigned;

            AchievementIdentifier = item.CredentialAchievement;

            SubType.EnsureDataBound();
            SubType.Value = item.AchievementType;
            Hours.ValueAsDecimal = item.Hours;
            Title.Text = item.AchievementTitle;
            AchievementDescription.InnerHtml = Markdown.ToHtml(item.AchievementDescription);
            AccreditorName.Text = item.AuthorityName;
            AccreditorCity.Text = item.AuthorityCity;
            AccreditorProvince.Text = item.AuthorityProvince;
            AccreditorCountry.Text = item.AuthorityCountry;

            EnableSignOff.Checked = (item.AuthorityType == "Self");

            DateCompleted.Value = item.Completed;
            IsInTrainingPlan.Checked = item.CredentialIsInTrainingPlan;
            LoadAttachments(item.CredentialAchievement);
            Comment.Text = item.Description;
            ValidationStatus.Value = item.Status;
            ExpirationDate.Text = item.Expired.HasValue ? item.Expired.Value.Date.Format() : "N/A";

            IsTimeSensitive = item.LifetimeMonths.HasValue && item.LifetimeMonths != 0;
            if (IsTimeSensitive)
                IsTimeSensitiveYes.Checked = true;
            else
                IsTimeSensitiveNo.Checked = true;

            ValidForCount.ValueAsInt = item.LifetimeMonths;

            GradePercent.ValueAsDecimal = item.ContactExperienceScore * 100m;
            IsSuccess.Checked = item.ContactExperienceIsSuccess;

            DateAssigned.Text = item.CredentialAssigned.HasValue ? TimeZones.FormatDateOnly(item.CredentialAssigned.Value, user.TimeZone) : "N/A";

            Number.Text = item.CredentialAuthorityReference;
            IsRequired.Checked = item.CredentialIsRequired;
            Notified0.Text = item.CredentialExpirationReminderDelivered0.HasValue ? item.CredentialExpirationReminderDelivered0.Value.DateTime.Format() : "N/A";
            Notified1.Text = item.CredentialExpirationReminderDelivered1.HasValue ? item.CredentialExpirationReminderDelivered1.Value.DateTime.Format() : "N/A";
            Notified2.Text = item.CredentialExpirationReminderDelivered2.HasValue ? item.CredentialExpirationReminderDelivered2.Value.DateTime.Format() : "N/A";
            Notified3.Text = item.CredentialExpirationReminderDelivered3.HasValue ? item.CredentialExpirationReminderDelivered3.Value.DateTime.Format() : "N/A";

            SubType.Visible = item.AchievementType != AchievementTypes.OtherAchievement;
            SubtypeLiteral.Visible = !SubType.Visible;

            if (item.Type == EducationItemType.Credential)
            {
                AchievementSummary.LoadData(userIdentifier, item.CredentialAchievement.Value, canBeSigned);

                if (CurrentSessionState.Identity.IsImpersonating)
                    AchievementSummary.DisableSignOffButton();
            }

            AchievementSummary.Visible = AchievementSummary.ContainsVisiblePanel() &&
                item.Type == EducationItemType.Credential;

            InitSubType(item.CredentialAchievement);
            AchievementSelector.Value = item.CredentialAchievement;
            ShowAchievementDetails();

            var webUser = CurrentSessionState.Identity;
            FileManagerPanel.Visible = true;

            DownloadGrid.LoadData(containerGuid, containerType);

            DownloadGrid.AllowEdit =
                webUser.IsInRole(CmdsRole.Programmers)
                || webUser.IsInRole(CmdsRole.SystemAdministrators)
                || webUser.IsInRole(CmdsRole.OfficeAdministrators)
                || webUser.IsInRole(CmdsRole.FieldAdministrators);

            ToggleSignOff();

            AchievementSelector.Enabled = false;
            SubType.Enabled = false;
            Title.Enabled = item.Type == EducationItemType.Experience;

            trDateNotified.Visible = item.Type == EducationItemType.Credential;
        }

        private void LoadAttachments(Guid? achievementId)
        {
            if (achievementId == null)
                return;

            var uploads = UploadRepository.SelectAllAchievementUploads(achievementId.Value, null);

            AttachmentPanel.Visible = uploads.Rows.Count > 0;

            AttachmentList.LoadUploads(uploads);
        }

        #endregion

        #region Helper methods

        private void InitSubType(Guid? achievementId)
        {
            Guid? achievementOrganizationId = null;
            if (achievementId != null)
                achievementOrganizationId = VCmdsAchievementSearch.Select(achievementId.Value)?.OrganizationIdentifier;

            var isOther = string.IsNullOrEmpty(SubType.Value) || SubType.Value == "Other Achievements";

            AchievementSelector.Filter.AchievementType = SubType.Value;
            AchievementSelector.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            AchievementSelector.Filter.GlobalOrCompanySpecific = SubType.Value == "Module" || (achievementOrganizationId == OrganizationIdentifiers.CMDS);
            AchievementSelector.Value = null;

            AchievementRow.Visible = !isOther;

            IsInTrainingPlan.Enabled = !isOther;
            IsRequired.Enabled = !isOther;
            SettingsPanel.Visible = !isOther;
        }

        private void ShowAchievementDetails()
        {
            var hasValue = AchievementSelector.HasValue;

            NumberField.Visible = hasValue;
            GradePanel.Visible = !hasValue;

            if (!hasValue)
                return;

            var achievement = VCmdsAchievementSearch.Select(AchievementSelector.Value.Value);
            if (achievement == null)
                return;

            Title.Text = achievement.AchievementTitle;

            if (IsTimeSensitive || achievement.ValidForCount.HasValue)
            {
                IsTimeSensitiveYes.Checked = true;
                IsTimeSensitiveNo.Checked = false;

                if (achievement.ValidForCount.HasValue)
                {
                    trValidFor.Style["display"] = "";
                    ValidForCount.ValueAsInt = string.Equals(achievement.ValidForUnit, ValidForUnits.Years, StringComparison.OrdinalIgnoreCase)
                        ? 12 * achievement.ValidForCount.Value
                        : achievement.ValidForCount.Value;

                    AchievementValidForCount.InnerText = $"Valid for {achievement.ValidForCount.Value} months";
                }
            }
            else
            {
                IsTimeSensitiveYes.Checked = false;
                IsTimeSensitiveNo.Checked = true;

                trValidFor.Style["display"] = "none";
            }

            if (achievement.AchievementLabel == "Orientation")
            {
                IsTimeSensitiveYes.Enabled = false;
                IsTimeSensitiveNo.Enabled = false;
                ValidForCount.Enabled = false;
            }
        }

        private void ToggleSignOff()
        {
            EnableSignOff.Enabled = EmployeeAchievementHelper.TypeAllowsSignOff(SubType.Value) && AchievementSelector.HasValue;
            EnableSignOff.Text = EnableSignOff.Enabled ? string.Empty : "Sign off cannot be enabled for this type of achievement";
        }

        #endregion
    }
}