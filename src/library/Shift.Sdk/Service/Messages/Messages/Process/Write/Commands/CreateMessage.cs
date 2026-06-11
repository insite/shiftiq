using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Messages.Write
{
    public class CreateMessage : Command
    {
        public CreateMessage(
            Guid aggregate,
            Guid tenant,
            Guid senderIdentifier,
            string type,
            string name,
            MultilingualString title,
            MultilingualString contentText,
            Guid? surveyFormIdentifier)
        {
            AggregateIdentifier = aggregate;
            Tenant = tenant;
            SenderIdentifier = senderIdentifier;
            SurveyFormIdentifier = surveyFormIdentifier;
            Type = type;
            Name = name;
            Title = title;
            ContentText = contentText;
        }

        public Guid Tenant { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public MultilingualString Title { get; set; }
        public MultilingualString ContentText { get; set; }
    }
}