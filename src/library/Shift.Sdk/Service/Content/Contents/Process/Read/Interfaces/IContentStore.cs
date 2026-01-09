using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;

using Shift.Common;
namespace InSite.Application.Contents.Read
{
    public interface IContentStore
    {
        void InsertPrivacyGroups(IEnumerable<TGroupPermission> groups);
        Guid InsertPrivacyGroup(Guid containerIdentifier, string containerType, Guid groupIdentifier);
        void DeletePrivacyGroup(Guid privacyIdentifier);
        void DuplicatePrivacyGroup(Guid source, Guid destination, string containerType);

        void Delete(Guid content);

        void DeleteContainer(Guid container);
        void DeleteContainer(Guid container, string label);
        void DeleteContainer(Guid container, string label, string language);
        void DeleteContainerByType(Guid container, string label, string containerType);

        bool Save(TContent content);
        void Save(IEnumerable<TContent> contents);
        bool Save(string type, Guid container, string label, string value, string language, Guid organization);

        void SaveContainer(Guid organization, string type, Guid container, ContentContainer data);
        bool SaveContainer(Guid organization, string type, Guid container, string label, string value, string language = Language.Default);

        void DuplicateContainer(Guid source, Guid destination);
        void DuplicateContainer(Guid source, Guid destination, Guid organization);
        void DuplicateContainer(Guid source, Guid destination, Guid organization, string Title);
    }
}
