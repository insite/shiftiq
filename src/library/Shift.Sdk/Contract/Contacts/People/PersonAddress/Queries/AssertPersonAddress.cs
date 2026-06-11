using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPersonAddress : Query<bool>
    {
        public Guid AddressId { get; set; }
    }
}