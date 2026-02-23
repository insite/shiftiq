using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectSubmissionOptions : Query<IEnumerable<SubmissionOptionModel>>, ISubmissionOptionCriteria
    {
        public Guid? OrganizationId { get; set; }
        public Guid? SurveyQuestionId { get; set; }

        public bool? ResponseOptionIsSelected { get; set; }

        public int? OptionSequence { get; set; }
    }
}