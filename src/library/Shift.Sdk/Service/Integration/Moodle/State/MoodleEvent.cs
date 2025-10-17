namespace InSite.Domain.Integrations.Scorm
{
    public class MoodleEvent
    {
        public string EventName { get; set; }
        public string Component { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public string ObjectTable { get; set; }
        public string ObjectId { get; set; }
        public string Crud { get; set; }
        public int EduLevel { get; set; }
        public int ContextId { get; set; }
        public int ContextLevel { get; set; }
        public string ContextInstanceId { get; set; }
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string RelatedUserId { get; set; }
        public int Anonymous { get; set; }
        public long TimeCreated { get; set; }
        public string ShortName { get; set; }
        public string IdNumber { get; set; }
        public string UserGuid { get; set; }
        public string CallbackUrl { get; set; }
        public string Token { get; set; }

        public MoodleEventOther Other { get; set; }
    }

    public class MoodleEventOther
    {
        // sco_launched
        public string InstanceId { get; set; }
        public string LoadedContent { get; set; }

        // status_submitted
        public string AttemptId { get; set; }
        public string CmiElement { get; set; }
        public string CmiValue { get; set; }

        // user_graded
        public int? FinalGrade { get; set; }
        public string ItemId { get; set; }
        public bool? Overridden { get; set; }
    }
}
