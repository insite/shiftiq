using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFileClaim : Query<bool>
    {
        public Guid ClaimIdentifier { get; set; }
    }
}