using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class MultifactorAuthentication : PortalBasePage
    {
        private bool IsForced => string.Equals(Request.QueryString["force"], "true", StringComparison.OrdinalIgnoreCase);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MFASelectMode.Selected += MFASelectMode_Selected;

            MFAAuthenticatorApp.Exited += MFA_Exited;
            MFAAuthenticatorApp.Saved += MFA_Saved;

            MFAText.Exited += MFA_Exited;
            MFAText.Saved += MFA_Saved;

            MFAEmail.Exited += MFA_Exited;
            MFAEmail.Saved += MFA_Saved;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void MFASelectMode_Selected(object sender, EventArgs e)
        {
            if (MFASelectMode.Mode == OtpModes.None)
                Save();
            else
                ShowMode(MFASelectMode.Mode);
        }

        private void MFA_Exited(object sender, EventArgs e)
        {
            ShowMode(OtpModes.None);
        }

        private void MFA_Saved(object sender, EventArgs e)
        {
            Save();
        }

        private void Open()
        {
            PageHelper.AutoBindHeader(this);

            if (IsForced)
            {
                EditorStatus.AddMessage(
                    AlertType.Warning,
                    "fas fa-exclamation-triangle fs-xl",
                    Translate("Based on your administrator configurations you need to set up your multi-factor authentication before you can continue.")
                );
            }

            ShowMode(OtpModes.None);

            var mfaUser = GetMfaUserOrCreate();

            MFASelectMode.BindControls(mfaUser.OtpMode, !IsForced);
        }

        private void ShowMode(OtpModes mode)
        {
            MFASelectMode.Visible = mode == OtpModes.None;
            MFAAuthenticatorApp.Visible = mode == OtpModes.AuthenticatorApp;
            MFAText.Visible = mode == OtpModes.Text;
            MFAEmail.Visible = mode == OtpModes.Email;

            switch (mode)
            {
                case OtpModes.AuthenticatorApp:
                    MFAAuthenticatorApp.BindControls(!IsForced);
                    break;
                case OtpModes.Text:
                    MFAText.BindControls(!IsForced);
                    break;
                case OtpModes.Email:
                    MFAEmail.BindControls(!IsForced);
                    break;
            }
        }

        private void Save()
        {
            var mode = MFASelectMode.Mode;
            var mfaUser = GetMfaUserOrCreate();

            switch (mode)
            {
                case OtpModes.AuthenticatorApp:
                    mfaUser.OtpRecoveryPhrases = MFAAuthenticatorApp.RecoveryPhrases;
                    mfaUser.OtpTokenRefreshed = DateTime.Now;
                    mfaUser.OtpTokenSecret = StringHelper.ByteArrayToHex(MFAAuthenticatorApp.TokenSecret);
                    break;
                case OtpModes.Text:
                    mfaUser.OtpRecoveryPhrases = MFAText.RecoveryPhrases;
                    break;
                case OtpModes.Email:
                    mfaUser.OtpRecoveryPhrases = MFAEmail.RecoveryPhrases;
                    break;
            }

            mfaUser.OtpMode = mode;

            TUserAuthenticationFactorStore.Update(mfaUser);

            CurrentSessionState.Identity.User.ActiveOtpMode = mode;

            HttpResponseHelper.Redirect("/ui/portal/home");
        }

        private TUserAuthenticationFactor GetMfaUserOrCreate()
        {
            var usermfa = TUserAuthenticationFactorSearch.GetMFARecordById(new TUserAuthenticationFactorFilter
            {
                UserIdentifier = User.UserIdentifier
            });

            if (usermfa != null)
                return usermfa;

            var (token, recovery) = MFAHelpers.GenerateMFATokens();
            usermfa = new TUserAuthenticationFactor
            {
                UserAuthenticationFactorIdentifier = Guid.NewGuid(),
                UserIdentifier = User.UserIdentifier,
                OtpMode = OtpModes.None,
                OtpTokenRefreshed = DateTimeOffset.UtcNow,
                OtpTokenSecret = StringHelper.ByteArrayToHex(token),
                OtpRecoveryPhrases = string.Join("\n", recovery)
            };

            TUserAuthenticationFactorStore.Insert(usermfa);

            return usermfa;
        }
    }
}