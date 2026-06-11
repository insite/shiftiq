using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Messages
{
    public class MessageCreated : Change
    {
        public MessageCreated(
            Guid tenant,
            Guid sender,
            string type,
            string name,
            MultilingualString title,
            MultilingualString contentText,
            List<LinkItem> links,
            Guid? surveyFormIdentifier)
        {
            Tenant = tenant;
            Sender = sender;
            SurveyFormIdentifier = surveyFormIdentifier;
            Type = type;
            Name = name;
            Title = title;
            ContentText = contentText;
            Links = links;
        }

        public Guid Tenant { get; set; }
        public Guid Sender { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public MultilingualString Title { get; set; }
        public MultilingualString ContentText { get; set; }
        public List<LinkItem> Links { get; set; }
    }
}