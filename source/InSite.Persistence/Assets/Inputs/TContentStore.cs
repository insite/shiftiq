using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;

using Shift.Common;
namespace InSite.Persistence
{
    public class TContentStore : IContentStore
    {
        public void InsertPrivacyGroups(IEnumerable<TGroupPermission> groups)
        {
            using (var db = new InternalDbContext())
            {
                db.TGroupPermissions.AddRange(groups);
                db.SaveChanges();
            }
        }

        public Guid InsertPrivacyGroup(Guid containerIdentifier, string containerType, Guid groupIdentifier)
        {
            Guid result;

            using (var db = new InternalDbContext())
            {
                var existing = db.TGroupPermissions.FirstOrDefault(x => x.GroupIdentifier == groupIdentifier && x.ObjectIdentifier == containerIdentifier);

                if (existing == null)
                {
                    var entity = new TGroupPermission
                    {
                        ObjectIdentifier = containerIdentifier,
                        PermissionIdentifier = UniqueIdentifier.Create(),
                        GroupIdentifier = groupIdentifier,
                        ObjectType = containerType
                    };

                    db.TGroupPermissions.Add(entity);
                    db.SaveChanges();

                    result = entity.PermissionIdentifier;
                }
                else
                {
                    result = existing.PermissionIdentifier;
                }
            }

            return result;
        }

        public void DuplicatePrivacyGroup(Guid source, Guid destination, string containerType)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TGroupPermissions
                    .AsNoTracking()
                    .Where(d => d.ObjectIdentifier == source && d.ObjectType == containerType)
                    .ToList();
                foreach (var item in list)
                {
                    var content = new TGroupPermission();

                    content.PermissionIdentifier = UniqueIdentifier.Create();
                    content.ObjectIdentifier = destination;
                    content.GroupIdentifier = item.GroupIdentifier;
                    content.ObjectType = containerType;
                    db.TGroupPermissions.Add(content);
                }

                db.SaveChanges();
            }
        }

        public void DeletePrivacyGroup(Guid privacyIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TGroupPermissions.FirstOrDefault(x => x.PermissionIdentifier == privacyIdentifier);

                if (existing != null)
                {
                    db.TGroupPermissions.Remove(existing);
                    db.SaveChanges();
                }
            }
        }

        public bool Save(string type, Guid container, string label, string value, string language, Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TContents
                    .Where(x => x.ContainerIdentifier == container && x.ContentLabel == label && x.ContentLanguage == language && x.OrganizationIdentifier == organization)
                    .FirstOrDefault();

                if (entity == null)
                    entity = new TContent
                    {
                        ContentIdentifier = UniqueIdentifier.Create(),
                        ContainerType = type,
                        ContainerIdentifier = container,
                        ContentLabel = label,
                        ContentLanguage = language,
                        OrganizationIdentifier = organization
                    };

                entity.ContentText = value;

                return Save(entity);
            }
        }

        public bool SaveContainer(Guid organization, string type, Guid container, string label, string value, string language = Language.Default)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TContents
                    .Where(x => x.ContainerIdentifier == container && x.ContentLabel == label && x.ContentLanguage == language)
                    .FirstOrDefault();

                if (entity == null)
                {
                    entity = new TContent
                    {
                        ContentIdentifier = UniqueIdentifier.Create(),
                        ContainerType = type,
                        ContainerIdentifier = container,
                        ContentLabel = label,
                        ContentLanguage = language,
                        OrganizationIdentifier = organization,
                    };
                }

                entity.ContentText = value;

                return Save(entity);
            }
        }

        public bool Save(TContent content)
        {
            var hasChanges = false;

            var contentSnip = ContentContainerItem.GetSnip(content.ContentText, content.ContentHtml);
            var label = StringHelper.Snip(content.ContentLabel, 100);

            using (var db = new InternalDbContext())
            {
                var entity = db.TContents
                    .Where(x => x.ContentIdentifier == content.ContentIdentifier)
                    .FirstOrDefault();

                if (entity != null)
                {
                    hasChanges = !string.Equals(entity.ContentLabel, label)
                        || !string.Equals(entity.ContentLanguage, content.ContentLanguage)
                        || !string.Equals(entity.ContentSnip, contentSnip)
                        || !string.Equals(entity.ContentText, content.ContentText)
                        || !string.Equals(entity.ContentHtml, content.ContentHtml)
                        || !string.Equals(entity.ContainerType, content.ContainerType);

                    entity.ContentLabel = label;
                    entity.ContentLanguage = content.ContentLanguage;
                    entity.ContentSnip = contentSnip;
                    entity.ContentText = content.ContentText;
                    entity.ContentHtml = content.ContentHtml;
                    entity.ContainerType = content.ContainerType;
                    entity.OrganizationIdentifier = content.OrganizationIdentifier;
                }
                else
                {
                    content.ContentLabel = label;
                    content.ContentSnip = contentSnip;

                    db.TContents.Add(content);

                    hasChanges = true;
                }

                if (hasChanges)
                    db.SaveChanges();
            }

            return hasChanges;
        }

        public void Save(IEnumerable<TContent> contents)
        {
            foreach (var content in contents)
                Save(content);
        }

        public void SaveContainer(Guid organization, string type, Guid container, ContentContainer data)
        {
            var innerData = data.Clone();

            innerData.CreateSnips();

            using (var db = new InternalDbContext())
            {
                UpdateDeleteContainer(type, container, innerData, db);

                InsertContainer(organization, type, container, innerData, db);

                db.SaveChanges();
            }
        }

        private static void UpdateDeleteContainer(string type, Guid container, ContentContainer innerData, InternalDbContext db)
        {
            TContent[] entities;

            if (!innerData.IsLoaded) // This option should prevent removing of items that is not loaded to ContentBlock. It can be used when the ContentBlock was partialy loaded.
            {
                var filter = innerData.GetLabels().ToArray();

                entities = db.TContents.Where(x => x.ContainerIdentifier == container && filter.Contains(x.ContentLabel)).ToArray();
            }
            else
            {
                entities = db.TContents.Where(x => x.ContainerIdentifier == container).ToArray();
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var dataItem = innerData[entity.ContentLabel];

                if (dataItem == null)
                {
                    if (innerData.IsLoaded)
                        db.TContents.Remove(entity);

                    continue;
                }

                if (!innerData.IsLoaded && !dataItem.Html.Exists(entity.ContentLanguage) && !dataItem.Text.Exists(entity.ContentLanguage))
                    continue;

                entity.ContainerType = type;
                entity.ContentText = dataItem.Text[entity.ContentLanguage];
                entity.ContentHtml = dataItem.Html[entity.ContentLanguage];
                entity.ContentSnip = dataItem.Snip[entity.ContentLanguage];

                if (entity.ContentText.IsNotEmpty() || entity.ContentHtml.IsNotEmpty())
                {
                    // Mark item as empty to prevent insertion in Insert method
                    dataItem.Text[entity.ContentLanguage] = null;
                    dataItem.Html[entity.ContentLanguage] = null;
                }
                else
                {
                    db.TContents.Remove(entity);
                }
            }
        }

        private static void InsertContainer(Guid organization, string type, Guid container, ContentContainer innerData, InternalDbContext db)
        {
            foreach (var name in innerData.GetLabels())
            {
                var dataItem = innerData[name];
                if (dataItem.Text.IsEmpty && dataItem.Html.IsEmpty)
                    continue;

                foreach (var lang in dataItem.Text.Languages.Concat(dataItem.Html.Languages).Distinct())
                {
                    var entity = new TContent
                    {
                        ContentIdentifier = UniqueIdentifier.Create(),

                        OrganizationIdentifier = organization,
                        ContainerType = type,
                        ContainerIdentifier = container,

                        ContentLabel = name,
                        ContentLanguage = lang,
                        ContentText = dataItem.Text[lang],
                        ContentHtml = dataItem.Html[lang],
                        ContentSnip = dataItem.Snip[lang]
                    };

                    if (string.IsNullOrEmpty(entity.ContentText) && string.IsNullOrEmpty(entity.ContentHtml))
                        continue;

                    db.TContents.Add(entity);
                }
            }
        }

        public void Delete(Guid content)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TContents.FirstOrDefault(x => x.ContentIdentifier == content);
                if (entity != null)
                {
                    db.TContents.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public void DeleteContainer(Guid container)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TContents
                    .Where(x => x.ContainerIdentifier == container);

                db.TContents.RemoveRange(existing);
                db.SaveChanges();
            }
        }

        public void DeleteContainer(Guid container, string label)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TContents
                    .Where(x => x.ContainerIdentifier == container && x.ContentLabel == label);

                db.TContents.RemoveRange(existing);
                db.SaveChanges();
            }
        }

        public void DeleteContainer(Guid container, string label, string language)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TContents
                    .Where(x => x.ContainerIdentifier == container && x.ContentLabel == label && x.ContentLanguage == language);

                db.TContents.RemoveRange(existing);
                db.SaveChanges();
            }
        }

        public void DeleteContainerByType(Guid container, string label, string containerType)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TContents
                    .Where(x => x.ContainerIdentifier == container && x.ContentLabel == label && x.ContainerType == containerType);

                db.TContents.RemoveRange(existing);
                db.SaveChanges();
            }
        }

        public void DuplicateContainer(Guid source, Guid destination)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TContents
                    .AsNoTracking()
                    .Where(x => x.ContainerIdentifier == source)
                    .ToList();

                foreach (var item in list)
                {
                    var content = item.Clone();
                    content.ContentIdentifier = UniqueIdentifier.Create();
                    content.ContainerIdentifier = destination;
                    db.TContents.Add(content);
                }

                db.SaveChanges();
            }
        }

        public void DuplicateContainer(Guid source, Guid destination, Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TContents
                    .AsNoTracking()
                    .Where(x => x.ContainerIdentifier == source)
                    .ToList();

                foreach (var item in list)
                {
                    var content = item.Clone();
                    content.ContentIdentifier = UniqueIdentifier.Create();
                    content.ContainerIdentifier = destination;
                    content.OrganizationIdentifier = organization;
                    db.TContents.Add(content);
                }

                db.SaveChanges();
            }
        }

        public void DuplicateContainer(Guid source, Guid destination, Guid organization, string Title)
        {
            using (var db = new InternalDbContext())
            {
                var list = db.TContents
                    .AsNoTracking()
                    .Where(x => x.ContainerIdentifier == source)
                    .ToList();

                foreach (var item in list)
                {
                    if (item.ContentLabel == "Title" && item.ContentLanguage == Language.Default)
                        item.ContentSnip = item.ContentText = Title;
                    var content = item.Clone();
                    content.ContentIdentifier = UniqueIdentifier.Create();
                    content.ContainerIdentifier = destination;
                    content.OrganizationIdentifier = organization;
                    db.TContents.Add(content);
                }

                db.SaveChanges();
            }
        }
    }
}
