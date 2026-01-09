using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class UploadSettings
    {
        public UploadSettingsImage Images { get; set; }
        public UploadSettingsDocument Documents { get; set; }

        public UploadSettings()
        {
            Images = new UploadSettingsImage();
            Documents = new UploadSettingsDocument();
        }

        public bool IsEqual(UploadSettings other)
        {
            return Images.IsEqual(other.Images)
                && Documents.IsEqual(other.Documents);
        }
    }
}
