using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class UploadSettingsImage
    {
        public int MaximumFileSize { get; set; }
        public int MaximumHeight { get; set; }
        public int MaximumWidth { get; set; }

        public UploadSettingsImage()
        {
            MaximumFileSize = 1048576;
            MaximumHeight = 800;
            MaximumWidth = 800;
        }

        public bool IsEqual(UploadSettingsImage other)
        {
            return MaximumFileSize == other.MaximumFileSize
                && MaximumHeight == other.MaximumHeight
                && MaximumWidth == other.MaximumWidth;
        }

        public UploadSettingsImage Clone()
        {
            return new UploadSettingsImage
            {
                MaximumFileSize = MaximumFileSize,
                MaximumHeight = MaximumHeight,
                MaximumWidth = MaximumWidth
            };
        }
    }
}
