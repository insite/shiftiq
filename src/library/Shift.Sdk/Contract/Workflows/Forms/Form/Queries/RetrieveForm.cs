using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveForm : Query<FormModel>
    {
        public Guid SurveyFormId { get; set; }
    }
}