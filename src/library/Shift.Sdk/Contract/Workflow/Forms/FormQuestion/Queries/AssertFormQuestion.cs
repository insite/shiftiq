using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFormQuestion : Query<bool>
    {
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}