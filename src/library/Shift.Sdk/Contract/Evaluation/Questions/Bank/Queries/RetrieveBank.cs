using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBank : Query<BankModel>
    {
        public Guid BankId { get; set; }
    }
}