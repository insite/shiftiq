using System;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Toolbox;

namespace InSite.UI.Portal.Accounts.Users.Controls
{
    public partial class MFAAuthenticatorApp : BaseUserControl
    {
        public event EventHandler Exited;
        public event EventHandler Saved;

        public byte[] TokenSecret
        {
            get => (byte[])ViewState[nameof(TokenSecret)];
            private set => ViewState[nameof(TokenSecret)] = value;
        }

        public string RecoveryPhrases => RecoveryPhrasesTextBox.Text;

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
                || MFAHelpers.VerifyCode(TokenSecret, ConfirmationCode.Text);
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

            ContinueButton.Visible = !isLastStep;
            SaveButton.Visible = isLastStep;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Saved?.Invoke(this, new EventArgs());
        }

        public void BindControls(bool allowCancel)
        {
            var domain = ServiceLocator.AppSettings.Security.Domain;

            TokenSecret = MFAHelpers.GenerateMFAToken();
            QRCodeText.Value = $"otpauth://totp/{domain}:{User.Email}?secret={TokenSecret.ToBase32()}&issuer=InSite";
            ConfirmationCode.Text = null;

            Steps.SetActiveView(Step1);

            ContinueButton.Visible = true;
            SaveButton.Visible = false;

            CancelButton.Visible = allowCancel;
            CancelButton.NavigateUrl = "/ui/portal/profile";
        }
    }
}