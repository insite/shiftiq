using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertForm : Query<bool>
    {
        public Guid SurveyFormIdentifier { get; set; }
    }
}