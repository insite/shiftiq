using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroup : Query<bool>
    {
        public Guid GroupIdentifier { get; set; }
    }
}