using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAction : Query<bool>
    {
        public Guid ActionId { get; set; }
    }
}