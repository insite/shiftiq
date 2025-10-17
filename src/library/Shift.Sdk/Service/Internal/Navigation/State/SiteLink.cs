using System;
using System.Runtime.Serialization;

using Shift.Common;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class SiteLink
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        public bool ShouldSerializeIcon()
        {
            return !string.IsNullOrEmpty(Icon);
        }

        public bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(Text);
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ApplicationError("Url is required property.");

            if (string.IsNullOrEmpty(Icon) && string.IsNullOrEmpty(Text))
                throw new ApplicationError("Content is required property.");
        }

        public SiteLink Clone()
        {
            var clone = new SiteLink();

            this.ShallowCopyTo(clone);

            return clone;
        }
    }
}