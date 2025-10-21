using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TUserSettingFilter : Filter
    {
        public Guid? UserIdentifier { get; set; }
    }
}
