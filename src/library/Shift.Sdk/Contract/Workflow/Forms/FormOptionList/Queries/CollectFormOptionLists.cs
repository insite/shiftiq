using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormOptionLists : Query<IEnumerable<FormOptionListModel>>, IFormOptionListCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }

        public int? SurveyOptionListSequence { get; set; }
    }
}