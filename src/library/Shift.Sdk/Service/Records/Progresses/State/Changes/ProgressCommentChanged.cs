using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressCommentChanged : Change
    {
        public ProgressCommentChanged(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; set; }
    }
}
