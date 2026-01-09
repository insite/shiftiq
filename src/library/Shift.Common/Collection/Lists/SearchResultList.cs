using System.Collections;
using System.ComponentModel;

namespace Shift.Common
{
    public class SearchResultList : IListSource
    {
        private readonly IList _list;

        public SearchResultList(IList list)
        {
            _list = list;
        }

        public bool ContainsListCollection => false;

        public IList GetList() => _list;
    }
}
