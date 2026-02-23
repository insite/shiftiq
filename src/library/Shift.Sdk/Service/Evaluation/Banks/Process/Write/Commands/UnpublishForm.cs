using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UnpublishForm : Command
    {
        public Guid Form { get; set; }

        public UnpublishForm(Guid bank, Guid form)
        {
            AggregateIdentifier = bank;
            Form = form;
        }
    }
}
