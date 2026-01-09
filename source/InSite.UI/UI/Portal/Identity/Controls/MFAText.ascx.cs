using System;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Portal.Accounts.Users.Controls
{
    public partial class MFAText : BaseUserControl
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

            var isLastStep = Steps.GetActiveView() == Step2;

            if (isLastStep)
            {
                var (_, recovery) = MFAHelpers.GenerateMFATokens();
                RecoveryPhrasesTextBox.Text = string.Join("\n", recovery);
            }
            else if (!SavePhoneAndSendMessage())
                return;

            Steps.ActiveViewIndex++;

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
            CellNumber.Text = user.PhoneMobile;

            if (string.IsNullOrEmpty(user.PhoneMobile))
                MFAStatus.AddMessage(AlertType.Error, "In order to proceed you need to enter your cell number.");

            Steps.SetActiveView(Step1);

            ContinueButton.Visible = true;
            SaveButton.Visible = false;
            ResendButton.Visible = false;

            CancelButton.Visible = allowCancel;
            CancelButton.NavigateUrl = "/ui/portal/profile";
        }

        private bool SavePhoneAndSendMessage()
        {
            var user = ServiceLocator.UserSearch.GetUser(User.UserIdentifier);
            if (!string.Equals(user.PhoneMobile, CellNumber.Text))
            {
                user.PhoneMobile = Phone.Format(CellNumber.Text);
                UserStore.Update(user, null);
            }

            if (!SendMessage())
                return false;

            CellNumber2.Text = CellNumber.Text;

            return true;
        }

        private bool SendMessage()
        {
            var u = CurrentSessionState.Identity?.User?.UserIdentifier;
            var t = CurrentSessionState.Identity?.Organization?.Identifier;

            if (u == null || t == null)
                HttpResponseHelper.Redirect("/");

            GenerateOTPCode();

            var message = $"{Organization.CompanyName}: Your InSite verification code is {ConfirmationCodeStored}";

            try
            {
                var response = ServiceLocator.SwiftSmsGatewayClient.Send(CellNumber.Text, message);

                if (string.Equals(response, SwiftSmsGatewayClient.OkResponse, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }

            MFAStatus.AddMessage(AlertType.Error, "Unexpected error occured while sending text message.");

            return false;
        }

        private void GenerateOTPCode()
        {
            var number = new RandomNumberGenerator().Next(0, 1000000);

            ConfirmationCodeStored = number.ToString().PadLeft(6, '0');
        }
    }
}