using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class QGroupStore : IGroupStore
    {
        private IContactSearch _contactSearch;

        public QGroupStore(IContactSearch contactSearch)
        {
            _contactSearch = contactSearch;
        }

        public void InsertGroup(GroupCreated e)
        {
            using (var db = CreateContext())
            {
                var entity = new QGroup
                {
                    GroupIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.Tenant,
                    GroupName = e.Name.MaxLength(90),
                    GroupType = e.Type,
                    GroupCreated = e.ChangeTime
                };

                SetLastChange(entity, e);

                db.QGroups.Add(entity);
                db.SaveChanges();
            }

        }

        public void UpdateGroup(Change e, Action<QGroup> action)
        {
            using (var db = CreateContext())
            {
                UpdateGroup(e, action, db);

                db.SaveChanges();
            }
        }

        public void UpdateGroup(GroupSocialMediaUrlChanged e)
        {
            UpdateGroup(e, group =>
            {
                var socialMediaUrls = GetSocialMediaUrls(group);
                if (e.Url == null)
                {
                    if (socialMediaUrls.ContainsKey(e.Type))
                        socialMediaUrls.Remove(e.Type);
                }
                else
                    socialMediaUrls[e.Type] = e.Url;

                group.SocialMediaUrls = socialMediaUrls.Count > 0
                    ? JsonConvert.SerializeObject(socialMediaUrls)
                    : null;
            });
        }

        public static Dictionary<string, string> GetSocialMediaUrls(QGroup group)
        {
            return !string.IsNullOrEmpty(group.SocialMediaUrls)
                ? JsonConvert.DeserializeObject<Dictionary<string, string>>(group.SocialMediaUrls)
                : new Dictionary<string, string>();
        }

        public void DeleteGroup(GroupDeleted e)
        {
            const string query = "exec contacts.DeleteQGroup @GroupIdentifier";

            using (var db = CreateContext())
                db.Database.ExecuteSqlCommand(query, new SqlParameter("GroupIdentifier", e.AggregateIdentifier));
        }

        public void InsertGroupTag(GroupTagAdded e)
        {
            using (var db = CreateContext())
            {
                var tag = db.QGroupTags.FirstOrDefault(x => x.GroupIdentifier == e.AggregateIdentifier && x.GroupTag == e.Tag);
                if (tag != null)
                    return;

                tag = new QGroupTag { TagIdentifier = UniqueIdentifier.Create(), GroupIdentifier = e.AggregateIdentifier, GroupTag = e.Tag };
                db.QGroupTags.Add(tag);

                var group = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == e.AggregateIdentifier);
                SetLastChange(group, e);

                db.SaveChanges();
            }
        }

        public void DeleteGroupTag(GroupTagRemoved e)
        {
            using (var db = CreateContext())
            {
                var tag = db.QGroupTags.FirstOrDefault(x => x.GroupIdentifier == e.AggregateIdentifier && x.GroupTag == e.Tag);
                if (tag == null)
                    return;

                db.QGroupTags.Remove(tag);

                var group = db.QGroups.FirstOrDefault(x => x.GroupIdentifier == e.AggregateIdentifier);
                SetLastChange(group, e);

                db.SaveChanges();
            }
        }


        public void UpdateGroupAddress(GroupAddressChanged e)
        {
            var addressType = e.Type.ToString();

            using (var db = CreateContext())
            {
                var address = db.QGroupAddresses
                    .Where(x => x.GroupIdentifier == e.AggregateIdentifier && x.AddressType == addressType)
                    .FirstOrDefault();

                if (e.Address != null)
                {
                    if (address == null)
                    {
                        address = new QGroupAddress
                        {
                            AddressIdentifier = UniqueIdentifier.Create(),
                            GroupIdentifier = e.AggregateIdentifier,
                            AddressType = addressType
                        };

                        db.QGroupAddresses.Add(address);
                    }

                    address.City = e.Address.City;
                    address.Country = e.Address.Country;
                    address.Description = e.Address.Description;
                    address.PostalCode = e.Address.PostalCode;
                    address.Province = e.Address.Province;
                    address.Street1 = e.Address.Street1;
                    address.Street2 = e.Address.Street2;

                    if (e.Address.Latitude != null)
                        address.Latitude = Math.Round(e.Address.Latitude.Value, 3);
                    if (e.Address.Longitude != null)
                        address.Longitude = Math.Round(e.Address.Longitude.Value, 3);
                }
                else if (address != null)
                {
                    db.QGroupAddresses.Remove(address);
                }

                db.SaveChanges();
            }
        }

        public void InsertGroupContainer(GroupConnected e)
        {
            if (e.ConnectionType == ConnectionType.Child) // Save connection record only once for Parent connection
                return;

            if (e.ConnectionType != ConnectionType.Parent)
                throw new ArgumentException($"Unsupported connection type: {e.ConnectionType}");

            var child = e.AggregateIdentifier;
            var parent = e.Group;

            using (var db = CreateContext())
            {
                var exists = db.QGroupConnections
                    .Where(x => x.ChildGroupIdentifier == child && x.ParentGroupIdentifier == parent)
                    .Any();

                if (exists)
                    return;

                var entity = new QGroupConnection
                {
                    ChildGroupIdentifier = child,
                    ParentGroupIdentifier = parent
                };

                db.QGroupConnections.Add(entity);
                db.SaveChanges();
            }
        }

        public void DeleteGroupContainer(GroupDisconnected e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QGroupConnections
                    .Where(x => x.ChildGroupIdentifier == e.AggregateIdentifier && x.ParentGroupIdentifier == e.Group)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QGroupConnections.Remove(entity);
                db.SaveChanges();
            }
        }

        private void UpdateGroup(IChange e, Action<QGroup> action, InternalDbContext db)
        {
            var entity = db.QGroups
                .FirstOrDefault(x => x.GroupIdentifier == e.AggregateIdentifier);

            if (entity == null)
                return;

            action?.Invoke(entity);

            SetLastChange(entity, e);
        }

        private void SetLastChange(QGroup entity, IChange e)
        {
            var user = _contactSearch.GetUser(e.OriginUser);

            entity.LastChangeTime = e.ChangeTime;
            entity.LastChangeType = e.GetType().Name;
            entity.LastChangeUser = user != null ? user.UserFullName : UserNames.Someone;
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };
    }
}
