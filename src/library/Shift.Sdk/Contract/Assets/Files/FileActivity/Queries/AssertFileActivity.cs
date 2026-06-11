using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFileActivity : Query<bool>
    {
        public Guid ActivityIdentifier { get; set; }
    }
}