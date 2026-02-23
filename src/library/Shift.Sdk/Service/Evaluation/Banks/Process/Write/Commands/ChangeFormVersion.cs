using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormVersion : Command
    {
        public Guid Form { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }

        public ChangeFormVersion(Guid bank, Guid form, string major, string minor)
        {
            AggregateIdentifier = bank;
            Form = form;
            Major = major.NullIfWhiteSpace();
            Minor = minor.NullIfWhiteSpace();
        }
    }
}
