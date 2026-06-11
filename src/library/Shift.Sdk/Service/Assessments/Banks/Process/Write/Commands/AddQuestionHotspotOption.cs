using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class AddQuestionHotspotOption : Command
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }
        public HotspotShape Shape { get; set; }
        public ContentTitle Content { get; set; }
        public decimal Points { get; set; }

        public AddQuestionHotspotOption(Guid bank, Guid question, Guid option, HotspotShape shape, ContentTitle content, decimal points)
        {
            AggregateIdentifier = bank;
            Question = question;
            Option = option;
            Shape = shape;
            Content = content;
            Points = points;
        }
    }
}
