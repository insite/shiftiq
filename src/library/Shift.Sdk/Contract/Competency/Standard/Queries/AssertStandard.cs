using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertStandard : Query<bool>
    {
        public Guid StandardIdentifier { get; set; }
    }
}