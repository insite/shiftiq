using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IMembershipStore
    {
        void Delete(Guid membership);

        void Insert(MembershipStarted e);
        void Insert(MembershipResumed e);

        void Update(Change e, Action<QMembership> action);
    }
}