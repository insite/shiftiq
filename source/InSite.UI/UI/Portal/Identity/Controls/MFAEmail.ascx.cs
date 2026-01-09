using System;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.UI.Portal.Accounts.Users.Controls
{
    public partial class MFAEmail : BaseUserControl
    {
        public event EventHandler Exited;
        public event EventHandler Saved;

        public string RecoveryPhrases => RecoveryPhrasesTextBox.Text;

        private string ConfirmationCodeStored
        {
            get => (string)ViewState[nameof(ConfirmationCodeStored)];
            set => ViewState[nameof(ConfirmationCodeStored)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CodeValidator.ServerValidate += CodeValidator_ServerValidate;

            BackButton.Click += BackButton_Click;
            ContinueButton.Click += ContinueButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void CodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Steps.GetActiveView() != Step2
                || string.Equals(ConfirmationCodeStored, ConfirmationCode.Text);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (Steps.ActiveViewIndex == 0)
            {
                Exited?.Invoke(this, new EventArgs());
                return;
            }

            Steps.ActiveViewIndex--;

            ContinueButton.Visible = true;
            SaveButton.Visible = false;

            ConfirmationCode.Text = null;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Steps.ActiveViewIndex++;

            var isLastStep = Steps.GetActiveView() == Step3;

            if (isLastStep)
            {
                var (_, recovery) = MFAHelpers.GenerateMFATokens();
                RecoveryPhrasesTextBox.Text = string.Join("\n", recovery);
            }
            else
                SendEmail();

            ContinueButton.Visible = !isLastStep;
            SaveButton.Visible = isLastStep;
            ResendButton.Visible = Steps.GetActiveView() == Step2;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Saved?.Invoke(this, new EventArgs());
        }

        public void BindControls(bool allowCancel)
        {
            ConfirmationCode.Text = null;

            var user = UserSearch.Select(User.UserIdentifier);
            EmailAddress.Text = user.Email;
            EmailAddress2.Text = user.Email;

            Steps.SetActiveView(Step1);

            ContinueButton.Visible = true;
            SaveButton.Visible = false;
            ResendButton.Visible = false;

            CancelButton.Visible = allowCancel;
            CancelButton.NavigateUrl = "/ui/portal/profile";
        }

        private void SendEmail()
        {
            var u = CurrentSessionState.Identity?.User?.UserIdentifier;
            var t = CurrentSessionState.Identity?.Organization?.Identifier;

            if (u == null || t == null)
                HttpResponseHelper.Redirect("/");

            GenerateOTPCode();

            ServiceLocator.AlertMailer.Send(t.Value, u.Value, new AlertOTPActivationCode
            {
                ConfirmationCode = ConfirmationCodeStored,
                Organization = Organization.LegalName
            });
        }

        private void GenerateOTPCode()
        {
            var number = new RandomNumberGenerator().Next(0, 1000000);

            ConfirmationCodeStored = number.ToString().PadLeft(6, '0');
        }
    }
}