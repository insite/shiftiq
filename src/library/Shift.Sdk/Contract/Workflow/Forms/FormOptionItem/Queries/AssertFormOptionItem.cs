using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFormOptionItem : Query<bool>
    {
        public Guid SurveyOptionItemIdentifier { get; set; }
    }
}