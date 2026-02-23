using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AnalyzeForm : Command
    {
        public Guid FormIdentifier { get; set; }

        public AnalyzeForm(Guid bank, Guid form)
        {
            AggregateIdentifier = bank;
            FormIdentifier = form;
        }
    }
}
