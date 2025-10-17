using System;

namespace Shift.Contract
{
    public class ModifyGroupTag
    {
        public Guid GroupIdentifier { get; set; }
        public Guid TagIdentifier { get; set; }

        public string GroupTag { get; set; }
    }
}