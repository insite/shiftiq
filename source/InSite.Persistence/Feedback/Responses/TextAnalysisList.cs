using System;
using System.Collections;
using System.Collections.Generic;

namespace InSite.Persistence
{
    [Serializable]
    public class TextAnalysisList : IEnumerable<TextAnalysisItem>
    {
        private readonly List<TextAnalysisItem> _items;

        public int Count => _items.Count;

        public TextAnalysisItem this[int index] => _items[index];

        public TextAnalysisList()
        {
            _items = new List<TextAnalysisItem>();
        }

        public TextAnalysisList(List<TextAnalysisItem> items)
        {
            _items = items;
        }

        public void Add(TextAnalysisItem item)
        {
            _items.Add(item);
        }

        public IEnumerator<TextAnalysisItem> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
