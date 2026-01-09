using System.ComponentModel;

namespace Shift.Common
{
    public enum OtpModes
    {
        [Description("None")]
        None,

        [Description("Authenticator App")]
        AuthenticatorApp,

        [Description("Text")]
        Text,

        [Description("Email")]
        Email
    }
}