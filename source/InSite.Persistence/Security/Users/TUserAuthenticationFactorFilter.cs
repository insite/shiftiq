using System;

namespace InSite.Persistence
{
    public class TUserAuthenticationFactorFilter
    {
        public Guid? MFAIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
    }
}
