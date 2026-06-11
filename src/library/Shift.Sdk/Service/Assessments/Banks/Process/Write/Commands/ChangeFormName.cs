using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormName : Command
    {
        public Guid Form { get; set; }
        public string Name { get; set; }

        public ChangeFormName(Guid bank, Guid form, string name)
        {
            AggregateIdentifier = bank;
            Form = form;
            Name = name.NullIfWhiteSpace();
        }
    }
}
