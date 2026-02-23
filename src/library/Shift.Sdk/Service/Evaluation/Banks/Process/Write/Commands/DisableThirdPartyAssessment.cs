using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DisableThirdPartyAssessment : Command
    {
        public Guid Form { get; set; }

        public DisableThirdPartyAssessment(Guid bank, Guid form)
        {
            AggregateIdentifier = bank;
            Form = form;
        }
    }
}