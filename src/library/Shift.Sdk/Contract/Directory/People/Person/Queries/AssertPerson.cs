using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPerson : Query<bool>
    {
        public Guid PersonId { get; set; }
    }
}