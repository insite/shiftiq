using System;
using System.Web.UI;

using InSite.UI.Portal.Assessments.Attempts.Utilities;

namespace InSite.UI.Lobby.Events
{
    public partial class Login : Layout.Lobby.LobbyBasePage
    {
        private LoginController _controller;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _controller = new LoginController(Organization, ErrorMessage);

            IsAgree.AutoPostBack = true;
            IsAgree.CheckedChanged += (x, y) => { SubmitButton.Visible = IsAgree.Checked; };

            SubmitButton.Click += SubmitButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Page.Title = "Sign In";
            Welcome.Text = GetDisplayHtml($"{ActionModel.ActionUrl}#Welcome", null);
            PersonCode.EmptyMessage = GetDisplayText($"{ActionModel.ActionUrl}#PersonCode.EmptyMessage", null);
            Disclaimer.Text = GetDisplayHtml($"{ActionModel.ActionUrl}#Disclaimer", null);

            ValidateSafeExamBrowserVersion();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !IsAgree.Checked)
                return;

            _controller.Submit(PersonCode.Text.Trim(), ExamEventPassword.Text.Trim());
        }

        private void ValidateSafeExamBrowserVersion()
        {
            var currentVersionString = AttemptHelper.GetSebVersion(Request.UserAgent);

            // Safe Exam Browser is not detected.

            if (string.IsNullOrEmpty(currentVersionString))
                return;

            var requiredVersionString = ServiceLocator.AppSettings.Integration.SafeExamBrowser?.MinimumVersionRequired;

            // There is no configuration setting to indicate the minimum required version for Safe Exam Browser. Assume
            // all versions are OK.

            if (string.IsNullOrEmpty(requiredVersionString))
                return;

            try
            {
                var currentVersion = new Version(currentVersionString);

                var requiredVersion = new Version(requiredVersionString);

                var alert = (currentVersion >= requiredVersion)
                    ? $"<div class='alert alert-success mb-3'>Safe Exam Browser version {currentVersionString} is supported. Please sign in.</div>"
                    : $"<div class='alert alert-danger mb-3'>Safe Exam Browser version {currentVersionString} is <strong>not</strong> supported. Safe Exam Browser must be upgraded to version {requiredVersionString} (or higher) <strong>before</strong> starting your exam.</div>";

                SafeExamBrowserVersion.Text = alert;
            }
            catch (Exception)
            {
                // If a parsing failure occurs on either version string then the failover is our original logic.

                // The minimum required version of Safe Exam Browser is detected:
                if (string.Compare(currentVersionString, requiredVersionString, true) >= 0)
                    SafeExamBrowserVersion.Text = $"<div class='alert alert-success mb-3'>Safe Exam Browser version {currentVersionString} is supported. Please sign in.</div>";

                // An old unsupported version of Safe Exam Browser is detected:
                else if (string.Compare(currentVersionString, requiredVersionString, true) < 0)
                    SafeExamBrowserVersion.Text = $"<div class='alert alert-danger mb-3'>Safe Exam Browser version {currentVersionString} is <strong>not</strong> supported. Safe Exam Browser must be upgraded to version {requiredVersionString} (or higher) <strong>before</strong> starting your exam.</div>";
            }
        }
    }
}