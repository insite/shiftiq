using System;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class VLearnerActivityHelper
    {
        internal static IQueryable<VLearnerActivity> Filter(this IQueryable<VLearnerActivity> query, VLearnerActivityFilter filter, InternalDbContext db)
        {
            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.CertificateStatus))
                query = query.Where(x => x.CertificateStatus.StartsWith(filter.CertificateStatus));

            if (filter.AchievementGrantedSince.HasValue)
                query = query.Where(x => x.CertificateGranted >= filter.AchievementGrantedSince);

            if (filter.AchievementGrantedBefore.HasValue)
                query = query.Where(x => x.CertificateGranted < filter.AchievementGrantedBefore);

            if (filter.EnrollmentStartedSince.HasValue)
                query = query.Where(x => x.EnrollmentStarted >= filter.EnrollmentStartedSince);

            if (filter.EnrollmentStartedBefore.HasValue)
                query = query.Where(x => x.EnrollmentStarted < filter.EnrollmentStartedBefore);

            if (!string.IsNullOrEmpty(filter.EnrollmentStatus))
                query = query.Where(x => x.EnrollmentStatus.StartsWith(filter.EnrollmentStatus));

            if (!string.IsNullOrEmpty(filter.LearnerEmail))
                query = query.Where(x => x.LearnerEmail.StartsWith(filter.LearnerEmail));

            if (!string.IsNullOrEmpty(filter.LearnerNameFirst))
                query = query.Where(x => x.LearnerNameFirst.StartsWith(filter.LearnerNameFirst));

            if (filter.PersonCode.HasValue())
                query = query.Where(x => x.PersonCode.Contains(filter.PersonCode));

            if (!string.IsNullOrEmpty(filter.LearnerNameLast))
                query = query.Where(x => x.LearnerNameLast.StartsWith(filter.LearnerNameLast));

            if (!string.IsNullOrEmpty(filter.LearnerRole))
                query = query.Where(x => x.LearnerRole == filter.LearnerRole);

            if (filter.LearnerGenders.IsNotEmpty())
                query = query.Where(x => filter.LearnerGenders.Any(y => y == x.LearnerGender));

            if (filter.LearnerCitizenships.IsNotEmpty())
                query = query.Where(x => filter.LearnerCitizenships.Any(y => y == x.LearnerCitizenship));

            if (filter.ImmigrationStatuses.IsNotEmpty())
                query = query.Where(x => filter.ImmigrationStatuses.Any(y => y == x.ImmigrationStatus));

            if (filter.ReferrerNames.IsNotEmpty())
                query = query.Where(x => filter.ReferrerNames.Any(y => y == x.ReferrerName));

            if (filter.ProgramNames.IsNotEmpty())
                query = query.Where(x => filter.ProgramNames.Any(y => y == x.ProgramName));

            if (filter.GradebookNames.IsNotEmpty())
                query = query.Where(x => filter.GradebookNames.Any(y => y == x.GradebookName));

            var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
            var oneMonthAgo = DateTimeOffset.Now.AddMonths(-1);

            if (filter.EngagementStatus != EngagementStatusType.None)
            {
                if (filter.EngagementStatus == EngagementStatusType.NoActivity)
                    query = query.Where(x => x.SessionCount == 0);

                if (filter.EngagementStatus == EngagementStatusType.LastActivityOverOneWeekAgo)
                    query = query.Where(x => x.SessionStartedLast < oneWeekAgo);

                if (filter.EngagementStatus == EngagementStatusType.LastActivityOverOneMonthAgo)
                    query = query.Where(x => x.SessionStartedLast < oneMonthAgo);
            }

            if (filter.EngagementPrompt != EngagementPromptType.None)
            {
                // TODO: Exclude rows for learners who have already been prompted in the last 7 days!

                if (filter.EngagementPrompt == EngagementPromptType.NoPromptNeeded)
                    query = query.Where(x => x.SessionStartedLast >= oneWeekAgo || x.CertificateStatus == "Valid");

                if (filter.EngagementPrompt == EngagementPromptType.PromptNeeded)
                    query = query.Where(x => (x.SessionCount == 0 || x.SessionStartedLast < oneWeekAgo) && (x.CertificateStatus == null || x.CertificateStatus != "Valid"));
            }

            if (filter.MembershipStatusItemIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.MembershipStatusItemIdentifiers.Contains(x.MembershipStatusItemIdentifier.Value));

            if (filter.EmployerGroupIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.EmployerGroupIdentifiers.Contains(x.EmployerGroupIdentifier.Value));

            if (filter.ProgramIdentifiers.IsNotEmpty())
            {
                query = query.Where(x => db.TProgramEnrollments.Any(
                    y => filter.ProgramIdentifiers.Contains(y.ProgramIdentifier)
                      && y.LearnerUserIdentifier == x.LearnerIdentifier));
            }

            return query;
        }
    }
}
