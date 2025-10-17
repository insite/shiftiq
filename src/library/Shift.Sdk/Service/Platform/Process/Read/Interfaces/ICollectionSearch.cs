using System;
using System.Collections.Generic;

namespace InSite.Application.Utility.Read
{
    public interface ICollectionSearch
    {
        Dictionary<Guid, string> Select(HashSet<Guid> itemIdentifiers);
    }
}
