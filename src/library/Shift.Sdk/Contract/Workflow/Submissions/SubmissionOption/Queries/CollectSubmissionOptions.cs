using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectSubmissionOptions : Query<IEnumerable<SubmissionOptionModel>>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }

        public bool? ResponseOptionIsSelected { get; set; }

        public int? OptionSequence { get; set; }
    }
}