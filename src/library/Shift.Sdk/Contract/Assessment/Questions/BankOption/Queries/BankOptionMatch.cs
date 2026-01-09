using System;

namespace Shift.Contract
{
    public partial class BankOptionMatch
    {
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}