using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookReferenced : Change
    {
        public string Reference { get; set; }

        public GradebookReferenced(string reference)
        {
            Reference = reference;
        }
    }
}
