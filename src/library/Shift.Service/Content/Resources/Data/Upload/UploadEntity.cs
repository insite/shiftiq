namespace Shift.Service.Content;

public class UploadEntity
{
        public Guid ContainerIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid Uploader { get; set; }
        public Guid UploadIdentifier { get; set; }

        public string UploadPrivacyScope { get; set; } = null!;
        public string ContainerType { get; set; } = null!;
        public string? ContentFingerprint { get; set; }
        public string? ContentType { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; } = null!;
        public string? NavigateUrl { get; set; }
        public string Title { get; set; } = null!;
        public string UploadType { get; set; } = null!;

        public int? ContentSize { get; set; }

        public DateTimeOffset Uploaded { get; set; }
}
