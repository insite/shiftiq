using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IMembershipReasonStore
    {
        void Insert(MembershipReasonAdded e);
        void Update(MembershipReasonModified e);
        void Delete(MembershipReasonRemoved e);
    }
}
