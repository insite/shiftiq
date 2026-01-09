using System;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Accounts.Users.Controls
{
    public partial class MFASelectMode : BaseUserControl
    {
        public event EventHandler Selected;

        private bool AllowSaveNone
        {
            get => (bool?)ViewState[nameof(AllowSaveNone)] ?? false;
            set => ViewState[nameof(AllowSaveNone)] = value;
        }

        public OtpModes Mode
        {
            get
            {
                if (ModeNone.Checked)
                    return OtpModes.None;

                if (ModeAuthenticatorApp.Checked)
                    return OtpModes.AuthenticatorApp;

                if (ModeText.Checked)
                    return OtpModes.Text;

                if (ModeEmail.Checked)
                    return OtpModes.Email;

                throw new ArgumentException("Unsupported mode");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ModeNone.AutoPostBack = true;
            ModeNone.CheckedChanged += Mode_CheckedChanged;

            ModeAuthenticatorApp.AutoPostBack = true;
            ModeAuthenticatorApp.CheckedChanged += Mode_CheckedChanged;

            ModeText.AutoPostBack = true;
            ModeText.CheckedChanged += Mode_CheckedChanged;

            ModeEmail.AutoPostBack = true;
            ModeEmail.CheckedChanged += Mode_CheckedChanged;

            ContinueButton.Click += ContinueButton_Click;
            SaveButton.Click += ContinueButton_Click;
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            Selected?.Invoke(this, new EventArgs());
        }

        private void Mode_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonVisibility();

            if (!ModeNone.Checked)
                Selected?.Invoke(this, new EventArgs());
        }

        public void BindControls(OtpModes mode, bool allowSaveNone)
        {
            AllowSaveNone = allowSaveNone;

            ModeNone.Checked = mode == OtpModes.None;
            ModeAuthenticatorApp.Checked = mode == OtpModes.AuthenticatorApp;
            ModeText.Checked = mode == OtpModes.Text;
            ModeEmail.Checked = mode == OtpModes.Email;

            SetButtonVisibility();

            CancelButton.Visible = allowSaveNone;
            CancelButton.NavigateUrl = "/ui/portal/profile";
        }

        private void SetButtonVisibility()
        {
            ContinueButton.Visible = !ModeNone.Checked;
            SaveButton.Visible = ModeNone.Checked && AllowSaveNone;
        }
    }
}