using System;

using Shift.Common;

namespace InSite.Persistence
{
    public class TUserAuthenticationFactor
    {
        public Guid UserAuthenticationFactorIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string OtpRecoveryPhrases { get; set; }

        public string OtpTokenSecret { get; set; }

        public DateTimeOffset OtpTokenRefreshed { get; set; }

        public OtpModes OtpMode { get; set; }

        public virtual User User { get; set; }
        public Guid? ShortLivedCookieToken { get; set; }
        public DateTimeOffset? ShortCookieTokenStartTime { get; set; }

    }
}
