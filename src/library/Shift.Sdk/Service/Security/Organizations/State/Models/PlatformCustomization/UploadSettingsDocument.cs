using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class UploadSettingsDocument
    {
        public int MaximumFileSize { get; set; }

        public UploadSettingsDocument()
        {
            MaximumFileSize = 1048576;
        }

        public bool IsEqual(UploadSettingsDocument other)
        {
            return MaximumFileSize == other.MaximumFileSize;
        }

        internal UploadSettingsDocument Clone()
        {
            return new UploadSettingsDocument
            {
                MaximumFileSize = MaximumFileSize
            };
        }
    }
}
