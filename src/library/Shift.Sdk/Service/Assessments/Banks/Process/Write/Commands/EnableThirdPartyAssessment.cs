using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class EnableThirdPartyAssessment : Command
    {
        public Guid Form { get; set; }

        public EnableThirdPartyAssessment(Guid bank, Guid form)
        {
            AggregateIdentifier = bank;
            Form = form;
        }
    }
}