using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseAttachmentDeleted : Change
    {
        public string FileName { get; set; }

        public CaseAttachmentDeleted(string filename)
        {
            FileName = filename;
        }
    }
}
