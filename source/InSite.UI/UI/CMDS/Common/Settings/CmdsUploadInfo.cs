using System;

using Shift.Common;

namespace InSite.Cmds.Infrastructure
{
    public class CmdsUploadInfo
    {
        #region Properties

        public Guid ContainerIdentifier { get; private set; }
        public string Name { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }

        public string MimeType { get; private set; }
        public bool IsNull { get; private set; }

        public bool IsExpired => (DateTime.UtcNow - _created).TotalSeconds > 10;

        #endregion

        #region Fields

        private DateTime _created = DateTime.UtcNow;

        #endregion

        #region Construction

        public CmdsUploadInfo()
        {
            ContainerIdentifier = Guid.Empty;
            Timestamp = DateTimeOffset.MinValue;
            IsNull = true;
        }

        public CmdsUploadInfo(Guid guid, string name, DateTimeOffset timestamp)
        {
            if (guid == Guid.Empty)
                throw new ArgumentNullException(nameof(guid));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (timestamp == DateTimeOffset.MinValue)
                throw new ArgumentNullException(nameof(timestamp));

            ContainerIdentifier = guid;
            Name = name;
            Timestamp = Clock.Trim(timestamp);
            MimeType = MimeMapping.GetContentType(name);
        }

        #endregion

        #region Helper methods

        public void Expire() => _created = DateTime.MinValue;

        #endregion
    }
}