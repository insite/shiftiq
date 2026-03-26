using System;
using System.IO;

using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby
{
    public partial class VerifyEmail : Layout.Lobby.LobbyBasePage
    {
        public class Token
        {
            public DateTimeOffset Issued { get; private set; }
            public Guid UserId { get; private set; }
            public string Email { get; private set; }

            public const int LifetimeHours = 24;
            public const int ResendIntervalMinutes = 5;
            public bool Expired => (DateTimeOffset.UtcNow - Issued).TotalHours >= LifetimeHours;

            private Token()
            {

            }

            public Token(Guid userId, string email)
            {
                Issued = Clock.Trim(DateTimeOffset.UtcNow);
                UserId = userId;
                Email = email;
            }

            public string Serialize()
            {
                return StringHelper.EncodeBase64Url(EncryptionKey.Default, EncryptionHelper.GenerateSalt(8), stream =>
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(Issued);
                        writer.Write(UserId);
                        writer.Write(Email);
                    }
                });
            }

            public static Token Deserialize(string data)
            {
                if (data.IsEmpty())
                    return null;

                try
                {
                    return (Token)StringHelper.DecodeBase64Url(data, EncryptionKey.Default, 8, stream =>
                    {
                        var token = new Token();

                        using (var reader = new BinaryReader(stream))
                        {
                            token.Issued = reader.ReadDateTimeOffset();
                            token.UserId = reader.ReadGuid();
                            token.Email = reader.ReadString();
                        }

                        return token;
                    });
                }
                catch
                {
                    return null;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SendButton.Click += (s, a) => SendVerificationEmail();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);
            CloseButton.NavigateUrl = Common.Web.HttpRequestHelper.CurrentRootUrl;
            SendButton.Text = Translate("Verify Email");

            var token = Request["token"];
            if (token.IsNotEmpty())
                HandleVerification(token);
            else if (!HandleVerificationRequired())
                Failure(5);
        }

        private void HandleVerification(string data)
        {
            var token = Token.Deserialize(data);
            if (token == null)
            {
                Failure(1);
                return;
            }

            var now = DateTimeOffset.UtcNow;
            if (token.Issued > now)
                throw ApplicationError.Create("The token issue date ({0:yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK}) is greater than DateTimeOffset.UtcNow ({1:yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK})", token.Issued, now);

            if (token.Expired)
            {
                Failure(2);
                return;
            }

            var user = ServiceLocator.UserSearch.GetUser(token.UserId);
            if (user?.EmailVerificationTokenIssued == null || token.Issued != user.EmailVerificationTokenIssued.Value)
            {
                Failure(3);
                return;
            }

            if (!string.Equals(token.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                Failure(4);
                return;
            }

            if (!string.Equals(user.EmailVerified, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.EmailVerified = user.Email;
                UserStore.Update(user, null);
            }

            Success();
        }

        private QUser GetUnverifiedUser()
        {
            if (ServiceLocator.Partition.IsE03())
                return null;

            var identity = CurrentSessionState.Identity;
            if (identity == null || !identity.IsAuthenticated || identity.IsImpersonating)
                return null;

            if (identity.User == null
             || identity.Organization == null
             || !identity.Organization.PlatformCustomization.RequireEmailVerification
             || identity.User.EmailVerified == identity.User.Email)
                return null;

            var user = ServiceLocator.UserSearch.GetUser(identity.User.UserIdentifier);
            if (user == null)
                return null;

            if (string.Equals(user.EmailVerified, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                SignOut.Redirect(this, "User data is out of date");
                return null;
            }

            return user;
        }

        private bool HandleVerificationRequired()
        {
            var user = GetUnverifiedUser();
            if (user == null)
                return false;

            var tokenIssued = user.EmailVerificationTokenIssued.HasValue;
            var issuedSpan = tokenIssued ? (DateTimeOffset.UtcNow - user.EmailVerificationTokenIssued.Value) : TimeSpan.MaxValue;
            var alertMessage = Translate("Your email address has not been verified.");

            if (tokenIssued && issuedSpan.TotalHours < Token.LifetimeHours)
                alertMessage += " " + Translate("Please check your inbox and click the verification link.");

            ScreenStatus.AddMessage(AlertType.Warning, alertMessage);

            if (tokenIssued && issuedSpan.TotalMinutes < Token.ResendIntervalMinutes)
            {
                SendMessage.InnerText = Translate("A verification email was recently sent. Please wait a few minutes before requesting another.");
                SendMessage.Visible = true;
            }
            else
            {
                if (tokenIssued)
                {
                    SendMessage.InnerText = Translate("If you did not receive the email, you can request a new one.");
                    SendMessage.Visible = true;
                }

                SendButton.Visible = true;
                CloseButton.Visible = false;
            }

            return true;
        }

        private void SendVerificationEmail()
        {
            var user = GetUnverifiedUser();
            if (user == null)
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    Translate("Unable to send verification email. Please refresh the page and try again."));

                SendMessage.Visible = false;
                SendButton.Visible = false;
                CloseButton.Visible = true;
                return;
            }

            if (!user.EmailVerificationTokenIssued.HasValue || (DateTimeOffset.UtcNow - user.EmailVerificationTokenIssued.Value).TotalMinutes >= Token.ResendIntervalMinutes)
            {
                var token = new VerifyEmail.Token(user.UserIdentifier, user.Email);

                ServiceLocator.SendCommand(new ModifyUserFieldDateOffset(token.UserId, UserField.EmailVerificationTokenIssued, token.Issued));

                ServiceLocator.AlertMailer.Send(Organization.OrganizationIdentifier, token.UserId, new AlertUserEmailVerificationRequested
                {
                    AppUrl = HttpRequestHelper.CurrentRootUrl,
                    Organization = Organization.LegalName,
                    UserEmail = token.Email,
                    UserIdentifier = token.UserId,
                    TokenValue = token.Serialize()
                });
            }

            ScreenStatus.AddMessage(
                AlertType.Success,
                Translate("A new verification email has been sent to your email address."));

            SendButton.Visible = false;
            CloseButton.Visible = true;
        }

        private void Success()
        {
            ScreenStatus.AddMessage(
                AlertType.Success,
                Translate("Thank you for verifying your email address!"));
        }

        private void Failure(int number)
        {
            var message = Translate("Your email address verification link is expired or invalid. Please try again, and if you are still unable to verify your email address then contact your account administrator.");

            ScreenStatus.AddMessage(AlertType.Error, message + $" ({number})");
        }
    }
}