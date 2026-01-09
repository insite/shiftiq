using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable]
    public class UploadFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string ContainerType { get; set; }
        public string[] UploadType { get; set; }
        public string Keyword { get; set; }
        public DateTimeOffset? PostedSince { get; set; }
        public DateTimeOffset? PostedBefore { get; set; }

        public UploadFilter()
        {
            ContainerType = UploadContainerType.Oganization;
        }
    }
}
