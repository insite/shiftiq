using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class FormMessageConnected : Change
    {
        public Guid Form { get; set; }
        public FormMessageType MessageType { get; set; }
        public Guid? MessageIdentifier { get; set; }

        public FormMessageConnected(Guid form, FormMessageType messageType, Guid? messageIdentifier)
        {
            Form = form;
            MessageType = messageType;
            MessageIdentifier = messageIdentifier;
        }
    }
}
