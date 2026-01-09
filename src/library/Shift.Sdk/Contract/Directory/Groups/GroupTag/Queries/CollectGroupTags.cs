using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectGroupTags : Query<IEnumerable<GroupTagModel>>, IGroupTagCriteria
    {
        public Guid? GroupIdentifier { get; set; }

        public string GroupTag { get; set; }
    }
}