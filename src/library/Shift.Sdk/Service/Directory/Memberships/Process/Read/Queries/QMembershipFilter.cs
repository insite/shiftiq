using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QMembershipFilter : Filter
    {
        public Guid[] GroupIdentifiers { get; set; }
        public Guid? GroupIdentifier
        {
            get => GroupIdentifiers != null && GroupIdentifiers.Length == 1 ? GroupIdentifiers[0] : (Guid?)null;
            set => GroupIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }

        public Guid[] UserIdentifiers { get; set; }
        public Guid? UserIdentifier
        {
            get => UserIdentifiers != null && UserIdentifiers.Length == 1 ? UserIdentifiers[0] : (Guid?)null;
            set => UserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
    }
}
