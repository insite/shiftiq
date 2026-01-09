using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddSection : Command
    {
        public Guid Form { get; set; }
        public Guid Section { get; set; }
        public Guid Criterion { get; set; }

        public AddSection(Guid bank, Guid form, Guid section, Guid criterion)
        {
            AggregateIdentifier = bank;
            Form = form;
            Section = section;
            Criterion = criterion;
        }
    }
}
