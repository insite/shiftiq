using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseAttachmentFileRenamed : Change
    {
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }

        public CaseAttachmentFileRenamed(string oldFileName, string newFileName)
        {
            OldFileName = oldFileName;
            NewFileName = newFileName;
        }
    }
}
