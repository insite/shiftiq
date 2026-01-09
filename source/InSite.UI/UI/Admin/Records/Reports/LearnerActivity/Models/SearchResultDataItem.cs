using System;
using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    [Serializable]
    internal class SearchResultDataItem : VLearnerActivitySummary
    {
        public string EngagementStatus
        {
            get
            {
                if (SessionCount == 0)
                    return "4. Never Authenticated";

                var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
                var oneMonthAgo = DateTimeOffset.Now.AddMonths(-1);

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneWeekAgo)
                    return "2. Last Authenticated Over 1 Week Ago";

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneMonthAgo)
                    return "3. Last Authenticated Over 1 Month Ago";

                return "1. Authenticated Within the Past Week";
            }
        }

        public string EngagementFlag
        {
            get
            {
                if (SessionCount == 0)
                    return "<i class='fas fa-flag text-danger'></i>";

                var oneWeekAgo = DateTimeOffset.Now.AddDays(-7);
                var oneMonthAgo = DateTimeOffset.Now.AddMonths(-1);

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneWeekAgo)
                    return "<i class='fas fa-flag text-primary'></i>";

                if (SessionStartedLast.HasValue && SessionStartedLast.Value < oneMonthAgo)
                    return "<i class='fas fa-flag text-warning'></i>";

                return "<i class='fas fa-flag text-success'></i>";
            }
        }

        public SearchResultProgramInfo[] Programs { get; set; }

        public string ProgramName => Programs.IsEmpty() ? null : string.Join(", ", Programs.Select(x => x.Name));

        public SearchResultDataItem(VLearnerActivitySummary item)
        {
            LearnerIdentifier = item.LearnerIdentifier;

            EnrollmentStatus = item.EnrollmentStatus;
            GradebookName = item.GradebookName;
            LearnerCitizenship = item.LearnerCitizenship;
            LearnerEmail = item.LearnerEmail;
            LearnerGender = item.LearnerGender;
            LearnerName = item.LearnerName;
            LearnerNameFirst = item.LearnerNameFirst;
            LearnerNameLast = item.LearnerNameLast;
            LearnerOccupation = item.LearnerOccupation;
            LearnerPhone = item.LearnerPhone;
            PersonCode = item.PersonCode;
            EmployerName = item.EmployerName;
            MembershipStatus = item.MembershipStatus;

            SessionCount = item.SessionCount;
            SessionMinutes = item.SessionMinutes;

            EnrollmentStarted = item.EnrollmentStarted;
            LearnerBirthdate = item.LearnerBirthdate;
            LearnerCreated = item.LearnerCreated;
            SessionStartedFirst = item.SessionStartedFirst;
            SessionStartedLast = item.SessionStartedLast;

            AchievementGranted = item.AchievementGranted;
        }

        public SearchResultDataItem(VLearnerActivity item)
        {
            LearnerIdentifier = item.LearnerIdentifier;

            EnrollmentStatus = item.EnrollmentStatus;
            GradebookName = item.GradebookName;
            LearnerCitizenship = item.LearnerCitizenship;
            LearnerEmail = item.LearnerEmail;
            LearnerGender = item.LearnerGender;
            LearnerName = item.LearnerName;
            LearnerNameFirst = item.LearnerNameFirst;
            LearnerNameLast = item.LearnerNameLast;
            LearnerOccupation = item.LearnerOccupation;
            LearnerPhone = item.LearnerPhone;
            PersonCode = item.PersonCode;
            EmployerName = item.EmployerGroupName;
            MembershipStatus = item.MembershipStatusItemName;

            SessionCount = item.SessionCount;
            SessionMinutes = item.SessionMinutes;

            EnrollmentStarted = item.EnrollmentStarted;
            LearnerBirthdate = item.LearnerBirthdate;
            LearnerCreated = item.LearnerCreated;
            SessionStartedFirst = item.SessionStartedFirst;
            SessionStartedLast = item.SessionStartedLast;

            AchievementGranted = item.CertificateGranted;
        }
    }
}