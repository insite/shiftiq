using System;
using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Invoices.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Payments.Read;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Records.Read;
using InSite.Application.Surveys.Read;

namespace InSite.Application.Contacts.Read
{
    public class VUser
    {
        public Guid UserIdentifier { get; set; }

        public string UserEmail { get; set; }
        public string UserEmailAlternate { get; set; }
        public string UserFirstName { get; set; }
        public string UserFullName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhoneMobile { get; set; }
        public string UserTimeZone { get; set; }

        public bool IsArchived { get; set; }

        public DateTimeOffset? UtcArchived { get; set; }

        public virtual ICollection<QAttempt> AssessorAttempts { get; set; }
        public virtual ICollection<QAttempt> LearnerAttempts { get; set; }
        public virtual ICollection<QProgress> Progresses { get; set; }
        public virtual ICollection<QGradebookCompetencyValidation> Validations { get; set; }
        public virtual ICollection<QEnrollment> Enrollments { get; set; }
        public virtual ICollection<VPerson> Persons { get; set; }
        public virtual ICollection<QResponseSession> ResponseSessions { get; set; }
        public virtual ICollection<TQuizAttempt> LearnerQuizAttempts { get; set; }

        public virtual ICollection<VIssue> VAdministratorIssues { get; set; }
        public virtual ICollection<VIssue> VLawyerIssues { get; set; }
        public virtual ICollection<VIssue> VOwnerIssues { get; set; }
        public virtual ICollection<VIssue> VTopicIssues { get; set; }

        public virtual ICollection<QJournal> Journals { get; set; }
        public virtual ICollection<QExperience> ValidatorExperiences { get; set; }
        public virtual ICollection<QJournalSetupUser> JournalSetupUsers { get; set; }
        public virtual ICollection<QPayment> Payments { get; set; }
        public virtual ICollection<VMembership> Memberships { get; set; }

        public virtual ICollection<TOrder> CustomerOrders { get; set; }
        public virtual ICollection<TOrder> ManagerOrders { get; set; }
        public virtual ICollection<TCourseDistribution> CourseDistributions { get; set; } = new HashSet<TCourseDistribution>();

        public VUser()
        {
            AssessorAttempts = new HashSet<QAttempt>();
            LearnerAttempts = new HashSet<QAttempt>();
            Progresses = new HashSet<QProgress>();
            Validations = new HashSet<QGradebookCompetencyValidation>();
            Enrollments = new HashSet<QEnrollment>();
            Persons = new HashSet<VPerson>();
            ResponseSessions = new HashSet<QResponseSession>();
            LearnerQuizAttempts = new HashSet<TQuizAttempt>();

            VAdministratorIssues = new HashSet<VIssue>();
            VLawyerIssues = new HashSet<VIssue>();
            VOwnerIssues = new HashSet<VIssue>();
            VTopicIssues = new HashSet<VIssue>();

            Journals = new HashSet<QJournal>();
            ValidatorExperiences = new HashSet<QExperience>();
            JournalSetupUsers = new HashSet<QJournalSetupUser>();
            Payments = new HashSet<QPayment>();
            Memberships = new HashSet<VMembership>();

            CustomerOrders = new HashSet<TOrder>();
            ManagerOrders = new HashSet<TOrder>();
        }
    }
}
