using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBank : Query<bool>
    {
        public Guid BankIdentifier { get; set; }
    }
}