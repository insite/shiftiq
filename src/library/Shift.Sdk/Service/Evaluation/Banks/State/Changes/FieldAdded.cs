using System;
using System.ComponentModel;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FieldAdded : Change
    {
        public Guid Identifier { get; set; }
        public Guid Section { get; set; }
        public Guid Question { get; set; }

        [DefaultValue(-1)]
        public int Index { get; set; } = -1;

        public FieldAdded(Guid identifier, Guid section, Guid question, int? index)
        {
            Identifier = identifier;
            Section = section;
            Question = question;

            // We need to have "index" nullable otherwise
            // all changes that do not have "Index" property will have "index" = 0
            // and the resulted sort order will be reversed
            //
            // We should use the same approach for any properties tha have DefaultValue attribute
            // and non-default value by default

            Index = index ?? -1;
        }
    }
}
