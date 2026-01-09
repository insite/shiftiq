using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Persistence
{
    public class QMembershipStore : IMembershipStore
    {
        private InternalDbContext CreateContext()
            => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        public void Delete(Guid membership)
        {
            using (var db = CreateContext())
            {
                var entity = db.QMemberships.FirstOrDefault(x => x.MembershipIdentifier == membership);
                if (entity == null)
                    return;

                var reasons = db.QMembershipReasons.Where(x => x.MembershipIdentifier == membership).ToArray();
                db.QMembershipReasons.RemoveRange(reasons);

                db.QMemberships.Remove(entity);
                db.SaveChanges();
            }
        }

        public void Insert(MembershipStarted e)
        {
            using (var db = CreateContext())
            {
                var entity = new QMembership
                {
                    MembershipIdentifier = e.AggregateIdentifier,

                    GroupIdentifier = e.Group,
                    OrganizationIdentifier = e.OriginOrganization,
                    UserIdentifier = e.User,

                    MembershipEffective = e.Effective,
                    MembershipFunction = e.Function.MaxLength(100)
                };

                SetTimestamp(entity, e);

                db.QMemberships.Add(entity);
                db.SaveChanges();
            }
        }

        public void Insert(MembershipResumed e)
        {
            using (var db = CreateContext())
            {
                var entity = new QMembership
                {
                    MembershipIdentifier = e.AggregateIdentifier,

                    GroupIdentifier = e.Group,
                    OrganizationIdentifier = e.OriginOrganization,
                    UserIdentifier = e.User,

                    MembershipEffective = e.Effective,
                    MembershipFunction = e.Function.MaxLength(100)
                };

                SetTimestamp(entity, e);

                db.QMemberships.Add(entity);
                db.SaveChanges();
            }
        }

        public void Update(Change e, Action<QMembership> action)
        {
            using (var db = CreateContext())
            {
                var entity = db.QMemberships.FirstOrDefault(x => x.MembershipIdentifier == e.AggregateIdentifier);
                if (entity == null)
                    return;

                action(entity);

                SetTimestamp(entity, e);

                db.SaveChanges();
            }
        }

        private static void SetTimestamp(QMembership entity, IChange change)
        {
            entity.Modified = change.ChangeTime;
            entity.ModifiedBy = change.OriginUser;
        }
    }
}