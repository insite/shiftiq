using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class GradePublished : Change
    {
        public string Reference { get; set; }
        public string Errors { get; set; }

        public GradePublished(string reference, string errors)
        {
            Reference = reference;
            Errors = errors;
        }
    }
}
