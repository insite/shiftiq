using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class PublishForm : Command
    {
        public Guid Form { get; set; }
        public FormPublication Publication { get; set; }

        public PublishForm(Guid bank, Guid form, FormPublication publication)
        {
            AggregateIdentifier = bank;
            Form = form;
            Publication = publication;
        }
    }
}