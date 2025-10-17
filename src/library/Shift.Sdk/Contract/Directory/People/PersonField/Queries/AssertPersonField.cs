using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPersonField : Query<bool>
    {
        public Guid FieldIdentifier { get; set; }
    }
}