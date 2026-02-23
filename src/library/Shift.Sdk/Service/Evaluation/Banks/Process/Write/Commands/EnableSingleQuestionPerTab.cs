using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class EnableSingleQuestionPerTab : Command
    {
        public Guid Specification { get; set; }

        public EnableSingleQuestionPerTab(Guid bank, Guid spec)
        {
            AggregateIdentifier = bank;

            Specification = spec;
        }
    }
}
