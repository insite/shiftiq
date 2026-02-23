using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFormOptionItem : Query<FormOptionItemModel>
    {
        public Guid SurveyOptionItemId { get; set; }
    }
}