using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IUserStore
    {
        void Insert(UserCreated e);
        void Delete(UserDeleted e);
        void Update(UserConnected e);
        void Update(UserDisconnected e);
        void Update(Change e, Action<QUser> action);
        void DeleteAll(Guid id);
        void DeleteAll();
    }
}
