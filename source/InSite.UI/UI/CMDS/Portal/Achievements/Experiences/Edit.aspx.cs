using System;

using InSite.Common.Web;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.User.Progressions.Controls;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Portal.Achievements.Experiences
{
    public partial class Edit : AdminBasePage, ICmdsUserControl
    {
        private string Parameters
        {
            get
            {
                return EmployeeUserIdentifier.HasValue && EmployeeUserIdentifier != User.UserIdentifier
                    ? $"?userid={EmployeeUserIdentifier}"
                    : string.Empty;
            }
        }

        private string SearchUrl =>  $"/ui/cmds/portal/achievements/credentials/search{Parameters}";

        private Guid Identifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

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

            CanBeSigned = Access.Write || EmployeeUserIdentifier == User.UserIdentifier;

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        private Guid WorkerUserIdentifier
        {
            get
            {
                if (EmployeeUserIdentifier.HasValue)
                    return EmployeeUserIdentifier.Value;

                if (ViewState[nameof(WorkerUserIdentifier)] == null)
                    ViewState[nameof(WorkerUserIdentifier)] = VCmdsCredentialAndExperienceSearch.Select(Identifier)?.UserIdentifier;

                return (Guid)(ViewState[nameof(WorkerUserIdentifier)] ?? Guid.Empty);
            }
        }

        private Guid? EmployeeUserIdentifier
        {
            set => ViewState[nameof(EmployeeUserIdentifier)] = value;
            get
            {
                if (ViewState[nameof(EmployeeUserIdentifier)] == null)
                    if (Guid.TryParse(Request["userID"], out var id))
                        ViewState[nameof(EmployeeUserIdentifier)] = id;

                return (Guid?)ViewState[nameof(EmployeeUserIdentifier)];
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();
                
                NewButton.NavigateUrl = $"/ui/cmds/portal/achievements/credentials/create{Parameters}";

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            EmployeeAchievementHelper.DeleteContactExperience(Identifier);
            HttpResponseHelper.Redirect(SearchUrl);
        }

        private void Open()
        {
            var user = UserSearch.Select(WorkerUserIdentifier);
            if (user == null)
                HttpResponseHelper.Redirect("/ui/cmds/portal/achievements/credentials/search", true);

            var experience = VCmdsCredentialAndExperienceSearch.Select(Identifier);
            if (experience == null)
                HttpResponseHelper.Redirect($"/ui/cmds/portal/achievements/credentials/search?userid={user.UserIdentifier}", true);

            PageHelper.AutoBindHeader(this, null, user.FullName);

            var item = new EmployeeAchievementDetails.EducationItem
            {
                Type = EducationItemType.Experience,
                ContactExperienceScore = experience.Score,
                ContactExperienceIsSuccess = experience.IsSuccess,
                AchievementType = experience.ContactExperienceType,
                AchievementTitle = experience.Title,
                Hours = experience.CreditHours,
                AuthorityName = experience.AuthorityName,
                AuthorityCity = experience.AuthorityCity,
                AuthorityProvince = experience.AuthorityProvince,
                AuthorityCountry = experience.AuthorityCountry,
                Completed = experience.Completed,
                Description = experience.Description,
                LifetimeMonths = experience.LifetimeMonths,
                Status = experience.Status,
                Expired = experience.Expired
            };

            AchievementDetails.SetInputValues(item, experience.UserIdentifier, user.UserIdentifier, false, experience.ExperienceIdentifier, UploadContainerType.ContactExperience);

            EmployeeUserIdentifier = experience.UserIdentifier;
        }

        private void Save()
        {
            var experience = VCmdsCredentialAndExperienceSearch.Select(Identifier);
            if (experience == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            var item = new EmployeeAchievementDetails.EducationItem();

            AchievementDetails.GetInputValues(item);

            experience.Score = item.ContactExperienceScore;
            experience.IsSuccess = item.ContactExperienceIsSuccess;
            experience.Title = item.AchievementTitle;
            experience.CreditHours = item.Hours;
            experience.AuthorityName = item.AuthorityName;
            experience.AuthorityCity = item.AuthorityCity;
            experience.AuthorityProvince = item.AuthorityProvince;
            experience.AuthorityCountry = item.AuthorityCountry;
            experience.Completed = item.Completed;
            experience.Description = item.Description;
            experience.LifetimeMonths = item.LifetimeMonths;
            experience.Status = item.Status;
            experience.Expired = item.Expired;

            ContactExperienceStore.Update(experience);
        }
    }
}
