using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormClassification : Command
    {
        public Guid Form { get; set; }
        public string Instrument { get; set; }
        public string Theme { get; set; }

        public ChangeFormClassification(Guid bank, Guid form, string instrument, string theme)
        {
            AggregateIdentifier = bank;
            Form = form;
            Instrument = instrument.NullIfWhiteSpace();
            Theme = theme.NullIfWhiteSpace();
        }
    }
}