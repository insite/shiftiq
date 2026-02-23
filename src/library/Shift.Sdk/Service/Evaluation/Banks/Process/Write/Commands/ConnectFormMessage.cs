using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ConnectFormMessage : Command
    {
        public Guid Form { get; set; }

        public FormMessageType MessageType { get; set; }
        public Guid? MessageIdentifier { get; set; }

        public ConnectFormMessage(Guid bank, Guid form, FormMessageType messageType, Guid? messageIdentifier)
        {
            AggregateIdentifier = bank;
            Form = form;
            MessageType = messageType;
            MessageIdentifier = messageIdentifier;
        }
    }
}