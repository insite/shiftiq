using System;
using System.ComponentModel;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class PlatformCustomizationSignIn
    {
        [DefaultValue(true)]
        public bool AllowGoogleSignIn { get; set; } = true;

        [DefaultValue(true)]
        public bool AllowMicrosoftSignIn { get; set; } = true;

        public bool IsEqual(PlatformCustomizationSignIn other)
        {
            return AllowGoogleSignIn == other.AllowGoogleSignIn
                && AllowMicrosoftSignIn == other.AllowMicrosoftSignIn;
        }
    }
}
