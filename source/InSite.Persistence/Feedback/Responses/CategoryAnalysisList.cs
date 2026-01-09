using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    [Serializable]
    public class CategoryAnalysisList
    {
        #region Fields

        private readonly List<CategoryAnalysisItem> _items = new List<CategoryAnalysisItem>();

        #endregion

        #region Properties

        public int Frequency
        {
            get
            {
                var result = 0;

                for (var i = 0; i < _items.Count; i++)
                {
                    var info = _items[i];

                    result += info.AnswerFrequency;
                }

                return result;
            }
        }

        public Int32 Count
        {
            get { return _items.Count; }
        }

        #endregion

        #region Public methods

        public int IndexOfQuestionAndAnswer(int questionId, string category)
        {
            for (Int32 i = 0; i < Count; i++)
            {
                var data = _items[i];
                if (data.QuestionId == questionId && string.Equals(data.Category, category, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public void Add(int questionId, string category)
        {
            int i = IndexOfQuestionAndAnswer(questionId, category);

            if (i > -1)
            {
                var data = _items[i];

                data.AnswerFrequency++;
            }
            else
            {
                var data = new CategoryAnalysisItem(questionId, category, 1);

                _items.Add(data);
            }
        }

        public void Add(CategoryAnalysisItem item)
        {
            _items.Add(item);
        }

        public CategoryAnalysisItem GetAnalysisForQuestionAndAnswer(int questionId, string category)
        {
            CategoryAnalysisItem data = null;

            for (int i = 0; i < _items.Count && data == null; i++)
            {
                var temp = _items[i];

                if (temp != null && questionId == temp.QuestionId && string.Equals(temp.Category, category, StringComparison.OrdinalIgnoreCase))
                    data = temp;
            }

            return data;
        }

        #endregion
    }
}
