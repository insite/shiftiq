using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGroupTags : Query<IEnumerable<GroupTagMatch>>, IGroupTagCriteria
    {
        public Guid? GroupIdentifier { get; set; }

        public string GroupTag { get; set; }
    }
}