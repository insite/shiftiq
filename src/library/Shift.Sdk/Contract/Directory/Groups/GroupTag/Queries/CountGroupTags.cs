using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGroupTags : Query<int>, IGroupTagCriteria
    {
        public Guid? GroupIdentifier { get; set; }

        public string GroupTag { get; set; }
    }
}