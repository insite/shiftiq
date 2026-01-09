using System;
using System.Web;

using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Lobby.Controls;
using InSite.Web.SignIn;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInMFA : SignInBasePage
    {
        private const string ShortLivedCookieName = "Insite.Short.MFA.Cookie";
        private const int ShortCookieLifeTime = 72;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            MfaButton.Click += MfaButton_Click;
            ResendButton.Click += ResendButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                PageInitialization();
        }

        private void PageInitialization()
        {
            if (!SignInLogic.IsMultiFactorAuthenticationEnabled(MFA))
            {
                SignOut.Redirect(this, $"Multi-factor authentication is not enabled (OTP Mode = {MFA?.OtpMode})");
                return;
            }
            var user = (UserId != Guid.Empty)
                ? UserSearch.Select(UserId)
                : null;

            if (user is null)
            {
                SignOut.Redirect(this, "User is null");
                return;
            }

            if (CheckMFACookie())
            {
                UserId = Guid.Empty;
                RedirectToSignInSucceedPage(true, user.UserIdentifier);
            }

            CellNumber = user.PhoneMobile;
            switch (MFA.OtpMode)
            {
                case OtpModes.None:
                    break;
                case OtpModes.AuthenticatorApp:
                    MfaText.Text = "Insert your authentication code from the two-factor authenticator (TOTP) app on your mobile device.";
                    MfaPhone.Visible = false;
                    break;
                case OtpModes.Text:
                    ResendButton.Visible = true;
                    lastSent.Value = DateTime.UtcNow.ToString();
                    if (SendTextMessage()) break;
                    else return;
                case OtpModes.Email:
                    lastSent.Value = DateTime.UtcNow.ToString();
                    ResendButton.Visible = true;
                    if (SendEmail()) break;
                    else return;
                default:
                    break;
            }
        }

        private void MfaButton_Click(object sender, EventArgs e)
        {
            Persistence.User user = null;
            try
            {
                user = (UserId != Guid.Empty)
                   ? UserSearch.Select(UserId)
                   : null;
            }
            catch { }

            if (user is null)
            {
                SignOut.Redirect(this, "User is null");
                return;
            }
            if (MFA is null)
            {
                SignOut.Redirect(this, "MFA is null");
                return;
            }
            var code = MfaCode.Text;

            var otpmode = MFA.OtpMode;
            switch (otpmode)
            {
                case OtpModes.None:
                    RedirectToSignInErrorPage(SignInErrorCodes.General, true, "Please try again.");
                    return;
                case OtpModes.AuthenticatorApp:
                    if (MFA.OtpTokenSecret is null)
                    {
                        SignOut.Redirect(this, "MFA OTP Tenant Secret is null");
                        return;
                    }
                    var secret = MFA.OtpTokenSecret;
                    var result = MFAHelpers.VerifyCode(secret, code);
                    if (!result)
                    {
                        var recovery = MFA.OtpRecoveryPharases.ToString();
                        if (string.IsNullOrWhiteSpace(recovery))
                        {
                            SignOut.Redirect(this, "MFA OTP Recovery Phrases is null");
                            return;
                        }
                        if (!recovery.Contains(code))
                        {
                            SetUpPageErrorView("Authentication code is incorrect");
                            return;
                        }
                    }

                    break;
                case OtpModes.Text:
                    if (CheckConfirmationCode(code))
                        break;
                    else return;
                case OtpModes.Email:
                    if (CheckConfirmationCode(code))
                        break;
                    else return;
                default:
                    break;
            }
            SetUpMFACookie();
            UserId = Guid.Empty;
            RedirectToSignInSucceedPage(true, user.UserIdentifier);
        }

        private void SetUpMFACookie()
        {
            if (MFA.ShortLivedCookieToken == null
                || MFA.ShortCookieTokenStartTime == null
                || MFA.ShortCookieTokenStartTime.Value.AddHours(ShortCookieLifeTime) <= DateTimeOffset.UtcNow
                )
            {
                var mfa = TUserAuthenticationFactorSearch.GetMFARecordById(new TUserAuthenticationFactorFilter
                {
                    UserIdentifier = UserId
                });
                mfa.ShortLivedCookieToken = Guid.NewGuid();
                mfa.ShortCookieTokenStartTime = DateTimeOffset.UtcNow;
                TUserAuthenticationFactorStore.Update(mfa);
                MFA = new SignInUserMFA(mfa);
            }

            var cookie = new HttpCookie(ShortLivedCookieName)
            {
                Domain = CookieTokenModule.SecuritySettings.Domain,
                Expires = DateTime.Now.AddHours(ShortCookieLifeTime),
                Path = CookieTokenModule.CookieSettings.Path,
                Value = MFA.ShortLivedCookieToken.Value.ToString(),
                SameSite = SameSiteMode.Lax,
                HttpOnly = true
            };

            Response.Cookies.Set(cookie);
        }

        private bool CheckMFACookie()
        {
            var cookie = HttpContext.Current.Request.Cookies[ShortLivedCookieName];
            return cookie?.Value != null
                && MFA.ShortLivedCookieToken.HasValue
                && cookie.Value == MFA.ShortLivedCookieToken.Value.ToString()
                && MFA.ShortCookieTokenStartTime.Value.AddHours(ShortCookieLifeTime) >= DateTimeOffset.UtcNow;
        }

        private void ResendButton_Click(object sender, EventArgs e)
        {
            if (MFA == null)
            {
                RedirectToSignIn();
            }

            if (!DateTime.TryParse(lastSent.Value, out var time))
            {
                RedirectToSignIn();
            }

            if (time.AddMinutes(1) > DateTime.UtcNow)
            {
                resentmessageview.Visible = false;
                SetUpPageErrorView("Please wait one minute before requesting another code.");
                return;
            }

            switch (MFA.OtpMode)
            {
                case OtpModes.None:
                    break;
                case OtpModes.AuthenticatorApp:
                    break;
                case OtpModes.Text:
                    if (SendTextMessage())
                        break;
                    else
                        return;
                case OtpModes.Email:
                    if (SendEmail())
                        break;
                    else
                        return;
                default:
                    break;
            }
            errorView.Visible = false;
            resentmessageview.Visible = true;
            lastSent.Value = DateTime.UtcNow.ToString();

            void RedirectToSignIn()
            {
                RedirectToSignInErrorPage(SignInErrorCodes.General, false, "Something went wrong, please try again");
            }
        }

        private bool CheckConfirmationCode(string code)
        {
            if (string.IsNullOrWhiteSpace(ConfirmationCodeSession))
            {
                return false;
            }
            if (string.Equals(ConfirmationCodeSession, code))
            {
                return true;
            }
            else
            {
                var recovery = MFA.OtpRecoveryPharases.ToString();
                if (string.IsNullOrWhiteSpace(recovery))
                    return false;

                if (!recovery.Contains(code))
                {
                    SetUpPageErrorView("Authentication code is incorrect");
                    return false;
                }
                return true;
            }
        }

        #region Helpers

        private string GenerateOTPCode()
        {
            var number = SignInLogic._random.Next(0, 1000000);
            var numberText = number.ToString().PadLeft(6, '0');
            ConfirmationCodeSession = numberText;
            return numberText;
        }

        private bool SendTextMessage()
        {
            if (CellNumber.IsEmpty())
            {
                RedirectToSignInErrorPage(SignInErrorCodes.ErrorWhileSendingTextMessage, true, "The user account does not have a mobile phone number configured. Please contact your administrator to resolve this issue.");
                return false;
            }

            var numberText = GenerateOTPCode();
            var message = $"{Organization.CompanyName}: Your InSite verification code is {numberText}";
            try
            {
                var t = CurrentSessionState.Identity?.Organization?.Identifier;

                if (UserId != Guid.Empty && t.HasValue)
                {
                    var client = ServiceLocator.SwiftSmsGatewayClient;

                    var response = client.Send(CellNumber, message);

                    if (string.Equals(response, SwiftSmsGatewayClient.OkResponse, StringComparison.OrdinalIgnoreCase))
                    {
                        MfaText.Text = "Your authentication code was sent by text message to";
                        MfaPhone.InnerText = "+1 (***) *** - " + Phone.GetPlainNumber(CellNumber).Substring(6);
                        return true;
                    }
                }
                RedirectToSignInErrorPage(SignInErrorCodes.ErrorWhileSendingTextMessage, true, "An error occurd while sending the text message, please try again.");
                return false;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                RedirectToSignInErrorPage(SignInErrorCodes.ErrorWhileSendingTextMessage, true, "Unexpected error happened while sending SMS.");

                return false;
            }
        }

        private void SetUpPageErrorView(string errorMessage)
        {
            errorView.Visible = true;
            ErrorViewMessage.InnerText = errorMessage;
        }

        private bool SendEmail()
        {
            var numberText = GenerateOTPCode();
            var t = CurrentSessionState.Identity?.Organization?.Identifier;
            if (UserId != Guid.Empty && t.HasValue)
            {
                ServiceLocator.AlertMailer.Send(t.Value, UserId, new AlertOTPActivationCode
                {
                    ConfirmationCode = numberText,
                    Organization = Organization.LegalName
                });
                MfaText.Text = "Your authentication code was sent by email";
                MfaPhone.InnerText = "";
                return true;
            }
            ResendButton.Visible = true;
            RedirectToSignInErrorPage(SignInErrorCodes.ErrorWhileSendingEmail, true, "An error occurd while sending the email, please try again.");
            return false;
        }

        #endregion
    }
}