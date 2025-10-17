using System;

namespace InSite.Application.Resources.Read
{
    public class VUpload
    {
        public Guid UploadIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ContainerIdentifier { get; set; }
        public Guid UploaderIdentifier { get; set; }
        public string UploadType { get; set; }
        public string AccessType { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NavigateUrl { get; set; }
        public string ContainerType { get; set; }
        public string ContentFingerprint { get; set; }
        public int ContentSize { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset Uploaded { get; set; }
        public int UploaderKey { get; set; }
        public string UploaderName { get; set; }
    }
}
