using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public interface IHistoryEntity
    {
        IReadOnlyList<IHistoryEntityColumn> Columns { get; }
        IEnumerable<IHistoryEntityState> States { get; }

        IHistoryEntityState GetState(DateTimeOffset timestamp);
        IHistoryEntityColumn GetColumn(string name);
    }
}
