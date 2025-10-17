using System;

namespace Shift.Contract
{
    public partial class GroupTagModel
    {
        public Guid GroupIdentifier { get; set; }
        public Guid TagIdentifier { get; set; }

        public string GroupTag { get; set; }
    }
}