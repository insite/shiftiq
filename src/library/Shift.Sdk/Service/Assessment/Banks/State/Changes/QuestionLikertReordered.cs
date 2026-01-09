using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Banks
{
    public class QuestionLikertReordered : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Dictionary<Guid, int> RowsOrder { get; set; }
        public Dictionary<Guid, int> ColumnsOrder { get; set; }

        public QuestionLikertReordered(Guid questionIdentifier, Dictionary<Guid, int> rowsOrder, Dictionary<Guid, int> columnsOrder)
        {
            QuestionIdentifier = questionIdentifier;
            RowsOrder = rowsOrder.IsEmpty() ? null : rowsOrder;
            ColumnsOrder = columnsOrder.IsEmpty() ? null : columnsOrder;
        }
    }
}
