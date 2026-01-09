using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFormOptionList : Query<bool>
    {
        public Guid SurveyOptionListIdentifier { get; set; }
    }
}