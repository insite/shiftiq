using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCaseUser : Query<bool>
    {
        public Guid JoinIdentifier { get; set; }
    }
}