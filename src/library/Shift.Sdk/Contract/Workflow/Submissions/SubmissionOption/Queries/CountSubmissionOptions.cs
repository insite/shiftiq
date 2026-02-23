using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissionOptions : Query<int>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? SurveyQuestionId { get; set; }

        public bool? ResponseOptionIsSelected { get; set; }

        public int? OptionSequence { get; set; }
    }
}