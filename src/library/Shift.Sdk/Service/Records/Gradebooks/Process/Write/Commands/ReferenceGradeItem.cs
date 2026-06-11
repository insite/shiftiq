using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ReferenceGradeItem : Command
    {
        public Guid Item { get; set; }
        public string Reference { get; set; }

        public ReferenceGradeItem(Guid record, Guid item, string reference)
        {
            AggregateIdentifier = record;
            Item = item;
            Reference = reference;
        }
    }
}
