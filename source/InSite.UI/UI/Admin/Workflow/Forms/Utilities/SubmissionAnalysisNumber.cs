using System;
using System.Collections.Generic;

using InSite.Domain.Surveys.Forms;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public partial class SubmissionAnalysis
    {
        [Serializable]
        public class NumberItem
        {
            public Guid QuestionID { get; }
            public double Minimum { get; }
            public double Maximum { get; }
            public double Sum { get; }
            public double Average { get; }
            public double? StandardDeviation { get; }
            public double? Variance { get; }
            public double Count { get; }

            public NumberItem(ResponseAnalysisIntegerItem item)
            {
                QuestionID = item.QuestionIdentifier;
                Minimum = item.Minimum;
                Maximum = item.Maximum;
                Sum = item.Sum;
                Average = item.Average;
                StandardDeviation = item.StandardDeviation;
                Variance = item.Variance;
                Count = item.Count;
            }
        }

        [Serializable]
        public class NumberList
        {
            #region Fields

            private readonly List<NumberItem> _items = new List<NumberItem>();

            #endregion

            #region Properties

            public int Count => _items.Count;

            #endregion

            #region Public methods

            public void Add(NumberItem item)
            {
                _items.Add(item);
            }

            public NumberItem GetData(Guid questionId)
            {
                NumberItem data = null;

                for (int i = 0; i < _items.Count && data == null; i++)
                {
                    NumberItem item = _items[i];
                    if (item != null && questionId == item.QuestionID)
                        data = item;
                }

                return data;
            }

            #endregion
        }
    }
}