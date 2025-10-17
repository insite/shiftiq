using System;

using Shift.Constant;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class Group
    {
        public Guid Identifier { get; set; }
        public GroupType Type { get; set; }
        public string Name { get; set; }
    }
}
