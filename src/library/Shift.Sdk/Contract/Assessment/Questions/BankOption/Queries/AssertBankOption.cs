using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankOption : Query<bool>
    {
        public Guid QuestionIdentifier { get; set; }

        public int OptionKey { get; set; }
    }
}