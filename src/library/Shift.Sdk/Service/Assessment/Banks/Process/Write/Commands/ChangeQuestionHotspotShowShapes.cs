using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionHotspotShowShapes : Command
    {
        public Guid Question { get; set; }
        public bool ShowShapes { get; set; }

        public ChangeQuestionHotspotShowShapes(Guid bank, Guid question, bool showShapes)
        {
            AggregateIdentifier = bank;
            Question = question;
            ShowShapes = showShapes;
        }
    }
}
