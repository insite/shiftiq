using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFormOptionList : Query<FormOptionListModel>
    {
        public Guid SurveyOptionListIdentifier { get; set; }
    }
}