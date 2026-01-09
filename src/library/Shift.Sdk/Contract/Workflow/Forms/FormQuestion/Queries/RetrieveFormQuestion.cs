using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFormQuestion : Query<FormQuestionModel>
    {
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}