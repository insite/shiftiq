using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertBankQuestion : Query<bool>
    {
        public Guid QuestionIdentifier { get; set; }
    }
}