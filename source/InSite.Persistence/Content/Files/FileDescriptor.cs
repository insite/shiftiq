using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public sealed class FileDescriptor
    {
        #region Properties

        public Guid UploadId { get; private set; }
        public string Path { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Fingerprint { get; private set; }
        public bool IsNull { get; private set; }
        public string MimeType { get; private set; }

        public Guid OrganizationIdentifier { get; private set; }
        public FileAccessType Access { get; private set; }

        public DateTimeOffset Posted
        {
            get => _posted;
            set => _posted = Clock.Trim(value);
        }

        public bool IsExpired => (DateTimeOffset.UtcNow - _created).TotalSeconds > 10;

        #endregion

        #region Fields

        private DateTimeOffset _created = DateTimeOffset.UtcNow;
        private DateTimeOffset _posted = DateTimeOffset.MinValue;

        #endregion

        #region Construction

        private FileDescriptor()
        {

        }

        public FileDescriptor(string path, FileEntity entity)
        {
            Path = path;
            IsNull = entity == null;

            if (!IsNull)
            {
                UploadId = entity.UploadId;
                OrganizationIdentifier = entity.OrganizationIdentifier;
                Path = entity.NavigateUrl;
                Name = entity.Name;
                Type = entity.ContentType;
                Posted = entity.Posted;
                Fingerprint = entity.ContentFingerprint;
                MimeType = MimeMapping.GetContentType(entity.Name);
                Access = entity.AccessControlType.ToEnum(FileAccessType.Tenant);
            }
        }

        public FileDescriptor(Guid organizationId, string path, FileEntity entity)
            : this(path, entity)
        {
            if (IsNull)
                OrganizationIdentifier = organizationId;
        }

        #endregion

        #region Helper methods

        public void Expire() => _created = DateTimeOffset.MinValue;

        public static FileDescriptor GetNull() => new FileDescriptor { IsNull = true };

        #endregion
    }
}