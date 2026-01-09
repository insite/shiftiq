using System;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    public interface IGroupStore
    {
        void InsertGroup(GroupCreated e);
        void UpdateGroup(Change e, Action<QGroup> action);
        void UpdateGroup(GroupSocialMediaUrlChanged e);
        void DeleteGroup(GroupDeleted e);

        void InsertGroupTag(GroupTagAdded e);
        void DeleteGroupTag(GroupTagRemoved e);

        void UpdateGroupAddress(GroupAddressChanged e);
        void InsertGroupContainer(GroupConnected e);
        void DeleteGroupContainer(GroupDisconnected e);
    }
}
