using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroupAddress : Query<bool>
    {
        public Guid AddressId { get; set; }
    }
}