using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TPersonFieldFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public string FieldName { get; set; }

        public TPersonFieldFilter Clone()
            => (TPersonFieldFilter)MemberwiseClone();
    }
}
