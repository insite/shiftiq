using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBank : Query<BankModel>
    {
        public Guid BankIdentifier { get; set; }
    }
}