using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IPersonStore
    {
        void Insert(PersonCreated e);
        void Delete(PersonDeleted e);
        void Update(PersonAddressModified e);
        void Update(PersonCommentModified e);
        void Update(Change e, Action<QPerson> action);
    }
}