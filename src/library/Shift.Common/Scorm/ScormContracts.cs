using System;

namespace Shift.Common.Scorm
{
    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int? Version { get; set; }
        public int? RegistrationCount { get; set; }
        public string CourseLearningStandard { get; set; }
    }

    public class CourseImport
    {
        public string CourseId { get; set; }
        public string Message { get; set; }

        public bool IsComplete { get; set; }
        public bool IsError { get; set; }
        public bool IsRunning { get; set; }
    }

    public class Registration
    {
        public string Id { get; set; }
        public int? Instance { get; set; }
        public DateTime? Updated { get; set; }
        public string RegistrationCompletion { get; set; }
        public string RegistrationSuccess { get; set; }
        public double? TotalSecondsTracked { get; set; }
        public DateTime? FirstAccessDate { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int? CourseVersion { get; set; }
        public string LearnerId { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
    }

    public class RegistrationProgress
    {
        public DateTime? CompletedDate { get; set; }
        public DateTime? FirstAccessDate { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public string RegistrationCompletion { get; set; }
        public string RegistrationSuccess { get; set; }
        public decimal? ScoreScaled { get; set; }
        public int? TotalSecondsTracked { get; set; }
    }

    public class RegistrationRequest
    {
        public RegistrationRequest() { }

        public RegistrationRequest(Guid registrationId, string courseSlug, Guid learnerId, string learnerEmail, string learnerFirstName, string learnerLastName)
        {
            RegistrationId = registrationId;
            CourseSlug = courseSlug;
            LearnerId = learnerId;
            LearnerEmail = learnerEmail;
            LearnerFirstName = learnerFirstName;
            LearnerLastName = learnerLastName;
        }

        public Guid RegistrationId { get; set; }
        public string CourseSlug { get; set; }
        public Guid LearnerId { get; set; }
        public string LearnerEmail { get; set; }
        public string LearnerFirstName { get; set; }
        public string LearnerLastName { get; set; }
    }
}