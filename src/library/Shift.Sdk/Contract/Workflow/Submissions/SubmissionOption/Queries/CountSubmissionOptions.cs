using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissionOptions : Query<int>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }

        public bool? ResponseOptionIsSelected { get; set; }

        public int? OptionSequence { get; set; }
    }
}