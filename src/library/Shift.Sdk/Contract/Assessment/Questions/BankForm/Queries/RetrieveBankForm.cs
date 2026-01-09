using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankForm : Query<BankFormModel>
    {
        public Guid FormIdentifier { get; set; }
    }
}