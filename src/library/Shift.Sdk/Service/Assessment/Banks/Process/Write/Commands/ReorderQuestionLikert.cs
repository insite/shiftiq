using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestionLikert : Command
    {
        public Guid Question { get; set; }
        public Dictionary<Guid, int> Rows { get; set; }
        public Dictionary<Guid, int> Columns { get; set; }

        public ReorderQuestionLikert(Guid bank, Guid question, Dictionary<Guid, int> rows, Dictionary<Guid, int> columns)
        {
            AggregateIdentifier = bank;
            Question = question;
            Rows = rows;
            Columns = columns;
        }
    }
}
