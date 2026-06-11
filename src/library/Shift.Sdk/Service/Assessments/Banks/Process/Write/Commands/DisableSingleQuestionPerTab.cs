using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DisableSingleQuestionPerTab : Command
    {
        public Guid Specification { get; set; }

        public DisableSingleQuestionPerTab(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
