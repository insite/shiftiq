using System;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Layout.Lobby.Controls
{
    public partial class SignInBasePage
    {
        public enum SignInErrorCodes
        {
            General,
            WrongUserNamePassword,
            UnApproved,
            Unauthorized,
            UnAuthorizedForOrganization,
            BruteForce,
            UnknownException,
            MFAFaied,
            ErrorWhileSendingTextMessage,
            ErrorWhileSendingEmail,
            WrongMFACode,
            UserNotFoundOrEmptyUserId
        }

        [Serializable]
        public class SignInUserMFA
        {
            public Guid MFAIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }

            public string OtpRecoveryPharases { get; set; }

            public byte[] OtpTokenSecret { get; set; }
            public OtpModes OtpMode { get; set; }
            public Guid? ShortLivedCookieToken { get; set; }
            public DateTimeOffset? ShortCookieTokenStartTime { get; set; }
            public SignInUserMFA(TUserAuthenticationFactor mfa)
            {
                MFAIdentifier = mfa.UserAuthenticationFactorIdentifier;
                UserIdentifier = mfa.UserAuthenticationFactorIdentifier;
                OtpRecoveryPharases = mfa.OtpRecoveryPhrases;
                OtpTokenSecret = StringHelper.HexToByteArray(mfa.OtpTokenSecret);
                OtpMode = mfa.OtpMode;
                ShortLivedCookieToken = mfa.ShortLivedCookieToken;
                ShortCookieTokenStartTime = mfa.ShortCookieTokenStartTime;
            }

            public SignInUserMFA()
            {

            }
        }
    }
}