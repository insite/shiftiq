using System;

namespace InSite.Domain.Banks
{
    [Serializable]
    public class FormAddendumItem
    {
        public int Asset { get; set; }
        public int Version { get; set; }

        public FormAddendumItem Clone() => new FormAddendumItem
        {
            Asset = Asset,
            Version = Version
        };

        public bool IsEqual(FormAddendumItem other)
        {
            return this.Asset == other.Asset
                && this.Version == other.Version;
        }


        public override bool Equals(object obj)
        {
            return obj is FormAddendumItem item &&
                   Asset == item.Asset &&
                   Version == item.Version;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Asset.GetHashCode();
            hash = hash * 23 + Version.GetHashCode();
            return hash;
        }
    }
}
