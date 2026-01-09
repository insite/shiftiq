using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class OrganizationUrl
    {
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Instagram { get; set; }
        public string YouTube { get; set; }
        public string Other { get; set; }
        public string WebSite { get; set; }

        public bool IsEmpty => !(
               ShouldSerializeFacebook()
            || ShouldSerializeTwitter()
            || ShouldSerializeLinkedIn()
            || ShouldSerializeInstagram()
            || ShouldSerializeYouTube()
            || ShouldSerializeOther()
            || ShouldSerializeWebSite()
            );

        public bool ShouldSerializeFacebook() => !string.IsNullOrEmpty(Facebook);
        public bool ShouldSerializeTwitter() => !string.IsNullOrEmpty(Twitter);
        public bool ShouldSerializeLinkedIn() => !string.IsNullOrEmpty(LinkedIn);
        public bool ShouldSerializeInstagram() => !string.IsNullOrEmpty(Instagram);
        public bool ShouldSerializeYouTube() => !string.IsNullOrEmpty(YouTube);
        public bool ShouldSerializeOther() => !string.IsNullOrEmpty(Other);
        public bool ShouldSerializeWebSite() => !string.IsNullOrEmpty(WebSite);

        public bool IsEqual(OrganizationUrl other)
        {
            return Facebook.NullIfEmpty() == other.Facebook.NullIfEmpty()
                && Twitter.NullIfEmpty() == other.Twitter.NullIfEmpty()
                && LinkedIn.NullIfEmpty() == other.LinkedIn.NullIfEmpty()
                && Instagram.NullIfEmpty() == other.Instagram.NullIfEmpty()
                && YouTube.NullIfEmpty() == other.YouTube.NullIfEmpty()
                && Other.NullIfEmpty() == other.Other.NullIfEmpty()
                && WebSite.NullIfEmpty() == other.WebSite.NullIfEmpty();
        }
    }
}