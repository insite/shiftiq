using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public partial class SubmissionAnalysis
    {
        [Serializable]
        public class CategoryItem
        {
            public Guid QuestionID { get; }
            public string Category { get; }
            public int AnswerFrequency { get; set; }

            public CategoryItem(Guid questionId, string category, int frequency)
            {
                QuestionID = questionId;
                Category = category;
                AnswerFrequency = frequency;
            }

            public CategoryItem(ResponseAnalysisCategoryItem item)
            {
                QuestionID = item.QuestionIdentifier;
                Category = item.OptionCategory;
                AnswerFrequency = item.AnswerCount;
            }
        }

        [Serializable]
        public class CategoryList
        {
            #region Fields

            private readonly List<CategoryItem> _items = new List<CategoryItem>();

            #endregion

            #region Properties

            public int Frequency => _items.Sum(x => x.AnswerFrequency);

            public int Count => _items.Count;

            #endregion

            #region Public methods

            public int IndexOfQuestionAndAnswer(Guid questionId, string category)
            {
                for (var i = 0; i < Count; i++)
                {
                    var data = _items[i];
                    if (data.QuestionID == questionId && string.Equals(data.Category, category, StringComparison.OrdinalIgnoreCase))
                        return i;
                }

                return -1;
            }

            public void Add(Guid questionId, string category)
            {
                int i = IndexOfQuestionAndAnswer(questionId, category);

                if (i > -1)
                {
                    _items[i].AnswerFrequency++;
                }
                else
                {
                    _items.Add(new CategoryItem(questionId, category, 1));
                }
            }

            public void Add(CategoryItem item)
            {
                _items.Add(item);
            }

            public CategoryItem GetAnalysisForQuestionAndAnswer(Guid questionId, string category)
            {
                CategoryItem data = null;

                for (int i = 0; i < _items.Count && data == null; i++)
                {
                    var temp = _items[i];

                    if (temp != null && questionId == temp.QuestionID && string.Equals(temp.Category, category, StringComparison.OrdinalIgnoreCase))
                        data = temp;
                }

                return data;
            }

            #endregion
        }
    }
}