using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class DeleteGradeItem : Command
    {
        public DeleteGradeItem(Guid record, Guid item)
        {
            AggregateIdentifier = record;
            Item = item;
        }

        public Guid Item { get; set; }
    }
}