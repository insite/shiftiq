using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankForm : Query<bool>
    {
        public Guid FormIdentifier { get; set; }
    }
}