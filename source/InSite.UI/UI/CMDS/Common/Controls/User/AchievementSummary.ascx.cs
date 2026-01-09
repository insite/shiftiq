using System;
using System.Web.UI;

using InSite.Common.Web.Cmds;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Cmds.Controls.Training.EmployeeAchievements
{
    public partial class AchievementSummary : UserControl
    {
        #region Events

        public event EventHandler SignedOff;

        private void OnSignedOff()
        {
            SignedOff?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        private Guid UserIdentifier
        {
            get { return (Guid)ViewState[nameof(UserIdentifier)]; }
            set { ViewState[nameof(UserIdentifier)] = value; }
        }

        private Guid AchievementIdentifier
        {
            get { return (Guid)ViewState[nameof(AchievementIdentifier)]; }
            set { ViewState[nameof(AchievementIdentifier)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SignOffButton.Click += SignOffButton_Click;
            RenewSignOffButton.Click += RenewSignOffButton_Click;
        }

        #endregion

        #region Event handlers

        private void SignOffButton_Click(object sender, EventArgs e)
        {
            SignOffAchievement();
            OnSignedOff();
        }

        private void RenewSignOffButton_Click(object sender, EventArgs e)
        {
            SignOffAchievement();
            OnSignedOff();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Visible = ContainsVisiblePanel();
        }

        #endregion

        #region Database operations

        private void SignOffAchievement()
        {
            EmployeeAchievementHelper.SignOff(UserIdentifier, AchievementIdentifier);

            var progression = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == UserIdentifier && x.AchievementIdentifier == AchievementIdentifier);

            LoadData(UserIdentifier, AchievementIdentifier, EmployeeAchievementHelper.CanSignOff(progression, UserIdentifier));
        }

        #endregion

        #region Public methods

        public bool ContainsVisiblePanel()
        {
            return CoursePanel.Visible ||
                SignOffPanel.Visible ||
                RenewSignOffPanel.Visible ||
                AttachmentPanel.Visible;
        }

        public void HidePanels()
        {
            CoursePanel.Visible = false;
            SignOffPanel.Visible = false;
            RenewSignOffPanel.Visible = false;
            AttachmentPanel.Visible = false;
        }

        public void LoadData(Guid userIdentifier, Guid achievementIdentifier, bool canBeSigned)
        {
            var identity = CurrentSessionState.Identity;
            UserIdentifier = userIdentifier;
            AchievementIdentifier = achievementIdentifier;

            var employeeAchievement = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.AchievementIdentifier == achievementIdentifier);

            if (employeeAchievement != null && canBeSigned)
            {
                var allowSelfDeclaredAchievementType = EmployeeAchievementHelper.TypeAllowsSignOff(employeeAchievement.AchievementLabel);

                var allowSelfDeclaredAchievement = employeeAchievement.AchievementAllowSelfDeclared &&
                    employeeAchievement.UserIdentifier == identity.User.UserIdentifier;

                var allowSelfDeclaredCredential = employeeAchievement.AuthorityType == "Self";

                var isIssued = EmployeeAchievementHelper.IsSignedOff(employeeAchievement);

                var isMyCredential = userIdentifier == identity.User.UserIdentifier;

                // In the E03 (CMDS) partition the business rule is slightly different. Somehow this needs to be made
                // configurable per partition (and possibly per organization), but in the meantime this is hard-coded
                // for simplicity.

                // FIXME: The rule for E03 should be improved so that sign-off is also allowed if the achievement allows
                // self-declaration AND the credential is issued AND the authority type on the credential is Self AND the
                // credential was issued by the person to whom it is assigned. In other words, if I uploaded my own
                // certificate in the portal, and my certificate later expires, then I should be allowed to redeclare it.

                var allowSignOff = ServiceLocator.Partition.IsE03()
                    ? (allowSelfDeclaredAchievementType && allowSelfDeclaredCredential)
                    : allowSelfDeclaredAchievement;

                SignOffPanel.Visible = allowSignOff && !isIssued && isMyCredential;

                RenewSignOffButton.Visible = allowSignOff && isIssued && isMyCredential;

                if (employeeAchievement.CredentialGranted.HasValue)
                {
                    var person = UserSearch.Select(employeeAchievement.UserIdentifier);
                    RenewSignOffText.Text =
                        $"{person.FullName} read the document and signed off as fully understanding the content, and accepting any requirements on " +
                        $"{employeeAchievement.CredentialGranted.Value.ToLocalTime():MMMM d, yyyy}.";
                }

                LoadModule(employeeAchievement);
                LoadAttachments(employeeAchievement);
            }
        }

        public void DisableSignOffButton()
        {
            SignOffButton.Enabled = false;
            SignOffButton.CssClass = "btn btn-primary disabled";
            RenewSignOffButton.Enabled = false;
            RenewSignOffButton.CssClass = "btn btn-primary disabled";
        }

        #endregion

        #region Helper methods

        private void LoadModule(VCmdsCredential credential)
        {
            var organization = CurrentSessionState.Identity.Organization.Code;

            CoursePanel.Visible = false;

            if (AchievementTypes.IsAchievementTypeIssuedByCourseCompletion(credential.AchievementLabel))
            {
                var course = CourseSearch.BindCourseFirst(x => x, x => x.Gradebook.AchievementIdentifier == credential.AchievementIdentifier);
                if (course != null && TGroupPermissionSearch.IsAccessAllowed(course.CourseIdentifier, CurrentSessionState.Identity))
                {
                    var launchUrl = Custom.CMDS.Portal.Courses.CmdsCourseLink.GetCourseLink(course.CourseIdentifier);
                    CoursePanel.Visible = true;
                    CourseLink.Text = course.CourseName;
                    CourseLaunch.NavigateUrl = launchUrl;
                    CourseLink.NavigateUrl = launchUrl;
                    CourseType.Text = AchievementTypes.Display(credential.AchievementLabel, organization);
                    CourseDescription.Text = GetContentBody(course.CourseIdentifier);
                }
            }
            else if (credential.AchievementLabel == "Orientation")
            {
                var course = CourseSearch.BindCourseFirst(x => x, x => x.Gradebook.AchievementIdentifier == credential.AchievementIdentifier);
                if (course != null && TGroupPermissionSearch.IsAccessAllowed(course.CourseIdentifier, CurrentSessionState.Identity))
                {
                    var launchUrl = Custom.CMDS.Portal.Courses.CmdsCourseLink.GetOrientationLink(course.CourseIdentifier);
                    CoursePanel.Visible = true;
                    CourseLink.Text = course.CourseName;
                    CourseLaunch.NavigateUrl = launchUrl;
                    CourseLink.NavigateUrl = launchUrl;
                    CourseType.Text = credential.AchievementLabel;
                    CourseDescription.Text = GetContentBody(course.CourseIdentifier);
                }
            }
        }

        private static string GetContentBody(Guid container)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(container);

            if (content != null)
                return content.Body.GetHtml();

            return null;
        }

        private void LoadAttachments(VCmdsCredential employeeAchievement)
        {
            var uploads = UploadRepository.SelectAllAchievementUploads(employeeAchievement.AchievementIdentifier, null);

            AttachmentPanel.Visible = uploads.Rows.Count > 0;
            AttachmentList.LoadUploads(uploads);
        }

        #endregion
    }
}