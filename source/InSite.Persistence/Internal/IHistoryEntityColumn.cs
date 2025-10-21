using System;

namespace InSite.Persistence
{
    public interface IHistoryEntityColumn
    {
        int Index { get; }
        string Name { get; }
        Type Type { get; }
    }
}
