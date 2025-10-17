using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class PlatformUrl
    {
        public string Logo { get; set; }
        public string Wallpaper { get; set; }

        public string Support { get; set; }
        public string Contact { get; set; }

        public bool ShouldSerializeLogo() => !string.IsNullOrEmpty(Logo);
        public bool ShouldSerializeWallpaper() => !string.IsNullOrEmpty(Wallpaper);

        public bool ShouldSerializeSupport() => !string.IsNullOrEmpty(Support);
        public bool ShouldSerializeContact() => !string.IsNullOrEmpty(Contact);

        public bool IsEqual(PlatformUrl other)
        {
            return Logo.NullIfEmpty() == other.Logo.NullIfEmpty()
                && Wallpaper.NullIfEmpty() == other.Wallpaper.NullIfEmpty()
                && Support.NullIfEmpty() == other.Support.NullIfEmpty()
                && Contact.NullIfEmpty() == other.Contact.NullIfEmpty();
        }
    }
}