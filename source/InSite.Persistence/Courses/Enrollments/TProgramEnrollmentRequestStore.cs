using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TProgramEnrollmentRequestStore
    {
        // Requests for the approver queue, joined to the learner name and program name. A Company
        // Administrator scopes by their organization; a partition-wide admin scopes by the
        // self-funded path. Every filter is optional, and each one is a separate conditional Where
        // rather than a disjunction, so an unfiltered call generates no "1=1 OR" query for the
        // optimizer to chew on. Passing no status returns every state, superseded rows included -
        // the queue passes "requested" for the pending ones. The learner name is left-joined so an
        // archived learner still shows (email is snapshotted). Oldest first, in every state: the
        // approver works the queue from the front, and a decided list stays in the order it was
        // worked.
        public static List<ProgramEnrollmentRequestListItem> Select(Guid? organizationId, string fundingPath, string status)
        {
            using (var db = new InternalDbContext())
            {
                IQueryable<TProgramEnrollmentRequest> query = db.TProgramEnrollmentRequests.AsNoTracking();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(r => r.RequestStatus == status);

                if (organizationId.HasValue)
                    query = query.Where(r => r.OrganizationIdentifier == organizationId.Value);

                if (!string.IsNullOrEmpty(fundingPath))
                    query = query.Where(r => r.RequestFundingPath == fundingPath);

                return query
                    .Join(db.TPrograms,
                        r => r.ProgramIdentifier,
                        p => p.ProgramIdentifier,
                        (r, p) => new { Request = r, p.ProgramName })
                    .GroupJoin(db.QUsers,
                        x => x.Request.LearnerUserIdentifier,
                        u => u.UserIdentifier,
                        (x, users) => new { x.Request, x.ProgramName, Users = users })
                    .SelectMany(
                        x => x.Users.DefaultIfEmpty(),
                        (x, u) => new ProgramEnrollmentRequestListItem
                        {
                            ProgramEnrollmentRequestIdentifier = x.Request.ProgramEnrollmentRequestIdentifier,
                            OrganizationIdentifier = x.Request.OrganizationIdentifier,
                            ProgramIdentifier = x.Request.ProgramIdentifier,
                            LearnerUserIdentifier = x.Request.LearnerUserIdentifier,
                            LearnerName = u != null ? u.FullName : null,
                            LearnerUserEmail = x.Request.LearnerUserEmail,
                            ProgramName = x.ProgramName,
                            RequestFundingPath = x.Request.RequestFundingPath,
                            RequestStatus = x.Request.RequestStatus,
                            RequestFee = x.Request.RequestFee,
                            RequestSubmittedAt = x.Request.RequestSubmittedAt,
                            RequestDecidedAt = x.Request.RequestDecidedAt
                        })
                    .OrderBy(x => x.RequestSubmittedAt)
                    .ToList();
            }
        }

        public static TProgramEnrollmentRequest Get(Guid requestId)
        {
            using (var db = new InternalDbContext())
                return db.TProgramEnrollmentRequests.AsNoTracking()
                    .FirstOrDefault(x => x.ProgramEnrollmentRequestIdentifier == requestId);
        }

        // The one open (requested) row for a learner+program, or null. The filtered unique index
        // guarantees at most one, so FirstOrDefault is exact.
        public static TProgramEnrollmentRequest GetOpen(Guid program, Guid learner)
        {
            using (var db = new InternalDbContext())
                return db.TProgramEnrollmentRequests.AsNoTracking()
                    .FirstOrDefault(x => x.ProgramIdentifier == program
                        && x.LearnerUserIdentifier == learner
                        && x.RequestStatus == ProgramEnrollmentRequestStatus.Requested);
        }

        public static void Insert(TProgramEnrollmentRequest request)
        {
            if (request.ProgramEnrollmentRequestIdentifier == Guid.Empty)
                request.ProgramEnrollmentRequestIdentifier = UniqueIdentifier.Create();

            if (request.RequestSubmittedAt == default(DateTimeOffset))
                request.RequestSubmittedAt = DateTimeOffset.UtcNow;

            using (var db = new InternalDbContext())
            {
                db.TProgramEnrollmentRequests.Add(request);
                db.SaveChanges();
            }
        }

        public static void Update(TProgramEnrollmentRequest request)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
