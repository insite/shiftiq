using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormGradebook : Command
    {
        public Guid Form { get; set; }
        public Guid? Gradebook { get; set; }

        public ChangeFormGradebook(Guid bank, Guid form, Guid? gradebook)
        {
            AggregateIdentifier = bank;
            Form = form;
            Gradebook = gradebook;
        }
    }
}
