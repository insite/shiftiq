using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddField : Command
    {
        public Guid Field { get; set; }
        public Guid Section { get; set; }
        public Guid Question { get; set; }
        public int Index { get; set; }

        public AddField(Guid bank, Guid field, Guid section, Guid question, int index)
        {
            AggregateIdentifier = bank;
            Field = field;
            Section = section;
            Question = question;
            Index = index;
        }
    }
}
