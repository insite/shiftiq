using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCase : Query<bool>
    {
        public Guid CaseIdentifier { get; set; }
    }
}