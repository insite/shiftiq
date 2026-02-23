using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionHotspotImage : Command
    {
        public Guid Question { get; set; }
        public HotspotImage Image { get; set; }

        public ChangeQuestionHotspotImage(Guid bank, Guid question, HotspotImage image)
        {
            AggregateIdentifier = bank;
            Question = question;
            Image = image;
        }
    }
}
