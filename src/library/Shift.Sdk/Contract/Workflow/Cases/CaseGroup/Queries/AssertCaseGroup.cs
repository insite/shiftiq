using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCaseGroup : Query<bool>
    {
        public Guid JoinId { get; set; }
    }
}