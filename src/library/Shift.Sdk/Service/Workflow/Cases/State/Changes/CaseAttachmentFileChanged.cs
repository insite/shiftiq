using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseAttachmentFileChanged : Change
    {
        public string FileName { get; set; }
        public Guid FileIdentifier { get; set; }

        public CaseAttachmentFileChanged(string fileName, Guid fileIdentifier)
        {
            FileName = fileName;
            FileIdentifier = fileIdentifier;
        }
    }
}
