using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankSpecification : Query<bool>
    {
        public Guid SpecIdentifier { get; set; }
    }
}