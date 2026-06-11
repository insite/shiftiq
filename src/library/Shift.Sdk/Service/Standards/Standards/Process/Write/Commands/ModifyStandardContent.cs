using System;

using Shift.Common.Timeline.Commands;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardContent : Command
    {
        public ContentContainer Content { get; private set; }

        [JsonConstructor]
        public ModifyStandardContent()
        {

        }

        public ModifyStandardContent(Guid standardId)
        {
            AggregateIdentifier = standardId;
            Content = new ContentContainer();
        }

        public ModifyStandardContent(Guid standardId, ContentContainer content)
        {
            AggregateIdentifier = standardId;
            Content = content;
        }
    }
}
