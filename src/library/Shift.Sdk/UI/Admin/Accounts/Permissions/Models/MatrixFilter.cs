using System;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class MatrixFilter : Filter
    {
        public Guid? ActionIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        
        public string GroupType { get; set; }
    }
}