using System.Collections.Generic;

using InSite.Application.Registrations.Read;

using Shift.Common;

namespace InSite.Admin.Events.Registrations.Reports
{
    public static class RegistrationCriteriaHelper
    {
        public class CriteriaItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static List<CriteriaItem> GetCriteriaItems(QRegistrationFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var result = new List<CriteriaItem>();

            if (filter.EventScheduledSince.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Since", Value = filter.EventScheduledSince.Value.Format(timeZone) });

            if (filter.EventScheduledBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Before", Value = filter.EventScheduledBefore.Value.Format(timeZone) });

            if (filter.RegistrationCompletedSince.HasValue)
                result.Add(new CriteriaItem { Name = "Registration Completed Since", Value = filter.RegistrationCompletedSince.Value.Format(timeZone) });

            if (filter.RegistrationCompletedBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Registration Completed Before", Value = filter.RegistrationCompletedBefore.Value.Format(timeZone) });

            if (filter.EventTitle.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Class Title", Value = filter.EventTitle });

            if (filter.VenueName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Class Venue", Value = filter.VenueName });

            if (filter.CandidateName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Student Name", Value = filter.CandidateName });

            if (filter.ApprovalStatus.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Approval status", Value = filter.ApprovalStatus });

            if (filter.AttendanceStatus.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Attendance status", Value = filter.AttendanceStatus });

            if (filter.RegistrationEmployerName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Registration Employer Name", Value = filter.RegistrationEmployerName });

            if (filter.RegistrationCustomerName.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Registration Customer Name", Value = filter.RegistrationCustomerName });

            if (filter.CandidateEmployerGroupIdentifier.HasValue)
            {
                var candidateEmployer = ServiceLocator.GroupSearch.GetGroup(filter.CandidateEmployerGroupIdentifier.Value);

                if (candidateEmployer != null)
                    result.Add(new CriteriaItem { Name = "Participant Employer Name", Value = candidateEmployer.GroupName });
            }

            if (filter.SeatAvailable.HasValue)
                result.Add(new CriteriaItem { Name = "Seat Availability", Value = filter.SeatAvailable.Value ? "Available for purchase" : "Hidden" });

            if (filter.RegistrationComment.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Comment", Value = filter.RegistrationComment });

            return result;
        }
    }
}