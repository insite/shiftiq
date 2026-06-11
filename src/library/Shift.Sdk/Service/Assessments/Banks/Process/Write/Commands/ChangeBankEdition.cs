using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeBankEdition : Command
    {
        public string Major { get; set; }
        public string Minor { get; set; }

        public ChangeBankEdition(Guid bank, string major, string minor)
        {
            AggregateIdentifier = bank;
            Major = major;
            Minor = minor;
        }
    }
}