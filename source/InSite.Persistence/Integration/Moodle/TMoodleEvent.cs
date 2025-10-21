using System;

namespace InSite.Persistence.Integration.Moodle
{
    public class TMoodleEvent
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserGuid { get; set; }

        public string Action { get; set; }
        public string CallbackUrl { get; set; }
        public string Component { get; set; }
        public string ContextInstanceId { get; set; }
        public string CourseId { get; set; }
        public string Crud { get; set; }
        public string EventData { get; set; }
        public string EventName { get; set; }
        public string IdNumber { get; set; }
        public string ObjectId { get; set; }
        public string ObjectTable { get; set; }
        public string OtherAttemptId { get; set; }
        public string OtherCmiElement { get; set; }
        public string OtherCmiValue { get; set; }
        public string OtherInstanceId { get; set; }
        public string OtherItemId { get; set; }
        public string OtherLoadedContent { get; set; }
        public string RelatedUserId { get; set; }
        public string ShortName { get; set; }
        public string Target { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }

        public bool Anonymous { get; set; }
        public bool? OtherOverridden { get; set; }

        public int ContextId { get; set; }
        public int ContextLevel { get; set; }
        public int EduLevel { get; set; }
        public int? OtherFinalGrade { get; set; }

        public DateTimeOffset EventWhen { get; set; }

        public long TimeCreated { get; set; }
    }
}