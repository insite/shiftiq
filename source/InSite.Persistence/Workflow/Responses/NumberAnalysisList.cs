using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    [Serializable]
    public class NumberAnalysisList
    {
        #region Fields

        private readonly List<NumberAnalysisItem> _items = new List<NumberAnalysisItem>();

        #endregion

        #region Properties

        public int Count => _items.Count;

        #endregion

        #region Public methods

        public void Add(NumberAnalysisItem item)
        {
            _items.Add(item);
        }
        
        public NumberAnalysisItem GetData(int fieldKey)
        {
            NumberAnalysisItem data = null;

            for (int i = 0; i < _items.Count && data == null; i++)
            {
                NumberAnalysisItem item = _items[i];
                if (item != null && fieldKey == item.QuestionId)
                    data = item;
            }

            return data;
        }

        #endregion
    }
}
