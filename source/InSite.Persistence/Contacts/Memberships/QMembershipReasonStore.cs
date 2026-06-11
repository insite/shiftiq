using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;

namespace InSite.Persistence
{
    public class QMembershipReasonStore : IMembershipReasonStore
    {
        private InternalDbContext CreateContext() => new InternalDbContext(true);

        public void Insert(MembershipReasonAdded change)
        {
            using (var db = CreateContext())
            {
                var entity = new QMembershipReason
                {
                    MembershipIdentifier = change.AggregateIdentifier,
                    ReasonIdentifier = change.ReasonIdentifier,
                    ReasonType = change.Type,
                    ReasonSubtype = change.Subtype,
                    ReasonEffective = change.Effective,
                    ReasonExpiry = change.Expiry,
                    PersonOccupation = change.PersonOccupation
                };

                SetTimestamps(change, entity);

                db.QMembershipReasons.Add(entity);
                db.SaveChanges();
            }
        }

        public void Update(MembershipReasonModified change)
        {
            using (var db = CreateContext())
            {
                var entity = db.QMembershipReasons
                    .FirstOrDefault(x => x.MembershipIdentifier == change.AggregateIdentifier
                                      && x.ReasonIdentifier == change.ReasonIdentifier);

                entity.ReasonType = change.Type;
                entity.ReasonSubtype = change.Subtype;
                entity.ReasonEffective = change.Effective;
                entity.ReasonExpiry = change.Expiry;
                entity.PersonOccupation = change.PersonOccupation;

                SetTimestamps(change, entity);

                db.SaveChanges();
            }
        }

        public void Delete(MembershipReasonRemoved change)
        {
            using (var db = CreateContext())
            {
                var entity = db.QMembershipReasons
                    .FirstOrDefault(x => x.MembershipIdentifier == change.AggregateIdentifier
                                      && x.ReasonIdentifier == change.ReasonIdentifier);

                db.QMembershipReasons.Remove(entity);
                db.SaveChanges();
            }
        }

        private void SetTimestamps(Change change, QMembershipReason entity)
        {
            if (entity.Created == default)
            {
                entity.Created = change.ChangeTime;
                entity.CreatedBy = change.OriginUser;
            }

            entity.Modified = change.ChangeTime;
            entity.ModifiedBy = change.OriginUser;

            entity.LastChangeType = change.GetType().Name;
        }
    }
}
