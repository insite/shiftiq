using System;

namespace Shift.Contract
{
    public class DeleteBankOption
    {
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}