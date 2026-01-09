using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveBankOption : Query<BankOptionModel>
    {
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}