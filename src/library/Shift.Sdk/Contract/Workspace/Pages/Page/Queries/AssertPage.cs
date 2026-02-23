using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPage : Query<bool>
    {
        public Guid PageId { get; set; }
    }
}