using System;

namespace Shift.Contract
{
    public partial class SubmissionOptionModel
    {
        public Guid? OrganizationId { get; set; }
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyOptionId { get; set; }
        public Guid SurveyQuestionId { get; set; }

        public bool ResponseOptionIsSelected { get; set; }

        public int OptionSequence { get; set; }
    }
}