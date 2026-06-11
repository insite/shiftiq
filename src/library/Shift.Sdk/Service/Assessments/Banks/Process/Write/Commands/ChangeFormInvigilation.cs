using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormInvigilation : Command
    {
        public Guid Form { get; set; }
        public FormInvigilation Invigilation { get; set; }

        public ChangeFormInvigilation(Guid bank, Guid form, FormInvigilation invigilation)
        {
            AggregateIdentifier = bank;
            Form = form;
            Invigilation = invigilation;
        }
    }
}
