using System;

using Shift.Common;

namespace InSite.Application.Standards.Read
{
    [Serializable]
    public class QStandardFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid[] StandardIdentifiers { get; set; }
        public int? AssetNumber
        {
            get => AssetNumbers?.Length == 1 ? AssetNumbers[0] : (int?)null;
            set => AssetNumbers = value.HasValue ? new[] { value.Value } : null;
        }
        public int[] AssetNumbers { get; set; }
    }
}
