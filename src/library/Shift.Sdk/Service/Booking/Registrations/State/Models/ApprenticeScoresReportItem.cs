using System;

namespace InSite.Domain.Registrations
{
    public class ApprenticeScoresReportItem
    {
        public Guid? GradeItemIdentifier { get; set; }
        public int? GradeItemSequence { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }
        public string AchievementDescription { get; set; }
        public string EventTitle { get; set; }
        public DateTimeOffset EventScheduledStart { get; set; }
        public Guid CandidateIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string EmployerGroupName { get; set; }
        public string Region { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string UserPhone { get; set; }
        public Guid? ScoreAchievementIdentifier { get; set; }
        public decimal? ScorePercent { get; set; }
        public string ScoreText { get; set; }
        public DateTimeOffset? GradebookEventStartDate { get; set; }
    }
}
