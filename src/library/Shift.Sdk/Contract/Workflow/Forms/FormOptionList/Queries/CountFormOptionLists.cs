using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormOptionLists : Query<int>, IFormOptionListCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }

        public int? SurveyOptionListSequence { get; set; }
    }
}