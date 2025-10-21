using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TGroupPermissionStore
    {
        public static void Copy(Guid fromGroup, Guid toGroup)
        {
            using (var db = new InternalDbContext())
            {
                var from = db.TGroupPermissions.Where(x => x.GroupIdentifier == fromGroup).ToList();
                var to = db.TGroupPermissions.Where(x => x.GroupIdentifier == toGroup).ToList();

                foreach (var item in from)
                {
                    if (0 == to.Count(x => x.ObjectIdentifier == item.ObjectIdentifier))
                    {
                        var permission = new TGroupPermission
                        {
                            GroupIdentifier = toGroup,
                            OrganizationIdentifier = item.OrganizationIdentifier,
                            PermissionIdentifier = UniqueIdentifier.Create(),

                            ObjectIdentifier = item.ObjectIdentifier,
                            ObjectType = item.ObjectType,

                            AllowRead = item.AllowRead,
                            AllowWrite = item.AllowWrite,
                            AllowDelete = item.AllowDelete,
                            AllowCreate = item.AllowCreate,
                            AllowAdministrate = item.AllowAdministrate,
                            AllowConfigure = item.AllowConfigure,

                            PermissionGranted = DateTimeOffset.UtcNow
                        };
                        db.TGroupPermissions.Add(permission);
                    }
                }

                db.SaveChanges();
            }
        }

        public static void Insert(TGroupPermission entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TGroupPermissions.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Delete(Guid permission)
        {
            using (var context = new InternalDbContext())
            {
                var privacy = context.TGroupPermissions.FirstOrDefault(x => x.PermissionIdentifier == permission);
                if (privacy != null)
                {
                    context.Entry(privacy).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        }

        public static void Delete(Guid group, Guid action)
        {
            var permission = TGroupPermissionSearch.Select(group, action);
            if (permission != null)
                Delete(permission.PermissionIdentifier);
        }

        public static void Save(TGroupPermission permission)
        {
            var p = TGroupPermissionSearch.Select(permission.GroupIdentifier, permission.ObjectIdentifier);
            if (p != null)
                return;

            Update(permission.GroupIdentifier, permission.ObjectIdentifier, permission.ObjectType,
                permission.AllowExecute,
                permission.AllowRead,
                permission.AllowWrite,
                permission.AllowCreate,
                permission.AllowDelete,
                permission.AllowAdministrate,
                permission.AllowConfigure,
                permission.AllowTrialAccess);
        }

        public static void Save(string objectType, Guid objectId, Guid group)
        {
            var p = TGroupPermissionSearch.Select(group, objectId);
            if (p != null)
                return;

            Update(group, objectId, objectType, false, true, false, false, false, false, false, false);
        }

        public static void Update(TGroupPermission entity)
        {
            TGroupPermission first = null;
            using (var db = new InternalDbContext())
                first = db.TGroupPermissions.FirstOrDefault(x => x.GroupIdentifier == entity.GroupIdentifier && x.ObjectIdentifier == entity.ObjectIdentifier);

            if (first != null)
                Update(entity.GroupIdentifier, entity.ObjectIdentifier, entity.ObjectType, entity.AllowExecute, entity.AllowRead, entity.AllowWrite, entity.AllowCreate, entity.AllowDelete, entity.AllowAdministrate, entity.AllowConfigure, entity.AllowTrialAccess);
            else
                Insert(entity);
        }

        public static Guid Update(Guid group, Guid @objectId, string objectType, bool execute, bool read, bool write, bool create, bool delete, bool administrate, bool configure) =>
            Update(group, @objectId, objectType, execute, read, write, create, delete, administrate, configure, null);

        public static Guid Update(Guid group, Guid @objectId, string objectType, bool execute, bool read, bool write, bool create, bool delete, bool administrate, bool configure, bool trial) =>
            Update(group, @objectId, objectType, execute, read, write, create, delete, administrate, configure, (bool?)trial);

        private static Guid Update(Guid group, Guid @objectId, string objectType, bool execute, bool read, bool write, bool create, bool delete, bool administrate, bool configure, bool? trial)
        {
            using (var db = new InternalDbContext())
            {
                var permission = db.TGroupPermissions.FirstOrDefault(x => x.GroupIdentifier == group && x.ObjectIdentifier == @objectId);

                if (permission == null)
                {
                    db.TGroupPermissions.Add(permission = new TGroupPermission
                    {
                        PermissionIdentifier = UniqueIdentifier.Create(),
                        GroupIdentifier = group,
                        ObjectIdentifier = @objectId,
                        ObjectType = objectType,
                        AllowTrialAccess = false
                    });
                }

                permission.AllowExecute = execute;

                permission.AllowRead = read || write || create || delete || administrate || configure;
                permission.AllowWrite = write || create || delete || administrate || configure;

                permission.AllowCreate = create || administrate || configure;
                permission.AllowDelete = delete || administrate || configure;

                permission.AllowAdministrate = administrate || configure;
                permission.AllowConfigure = configure;

                if (trial.HasValue)
                    permission.AllowTrialAccess = trial.Value;

                db.SaveChanges();

                return permission.PermissionIdentifier;
            }
        }

        public static void Update(DateTimeOffset granted, Guid user, Guid container, string containerType, List<Guid> grants, List<Guid> revokes)
        {
            using (var db = new InternalDbContext())
            {
                if (revokes != null)
                { 
                    var deletes = db.TGroupPermissions.Where(x => x.ObjectIdentifier == container && revokes.Any(y => y == x.GroupIdentifier));
                    db.TGroupPermissions.RemoveRange(deletes);
                }

                foreach (var grant in grants)
                {
                    if (!db.TGroupPermissions.Any(x => x.ObjectIdentifier == container && x.GroupIdentifier == grant))
                    {
                        var organization = db.QGroups.Where(x => x.GroupIdentifier == grant).Select(x => x.OrganizationIdentifier).FirstOrDefault();
                        if (organization == Guid.Empty)
                            continue;

                        var wpg = new TGroupPermission
                        {
                            PermissionGranted = granted,
                            PermissionGrantedBy = user,
                            ObjectIdentifier = container,
                            ObjectType = containerType,
                            GroupIdentifier = grant,
                            PermissionIdentifier = UniqueIdentifier.Create(),
                            OrganizationIdentifier = organization
                        };
                        db.TGroupPermissions.Add(wpg);
                    }
                }

                db.SaveChanges();
            }
        }
    }
}