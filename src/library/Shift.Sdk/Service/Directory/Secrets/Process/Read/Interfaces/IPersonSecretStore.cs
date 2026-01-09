using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IPersonSecretStore
    {
        void Insert(PersonSecretAdded e);
        void Delete(PersonSecretRemoved e);
    }
}
