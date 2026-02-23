using System;
using System.Collections;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QResponseCorrelationAnalysisFilter : Filter
    {
        #region Classes

        [Serializable]
        public sealed class AxisItem
        {
            #region Properties

            public Guid QuestionIdentifier { get; private set; }
            public string Title { get; private set; }

            #endregion

            #region Constructor

            private AxisItem()
            {

            }

            public AxisItem(Guid questionId, string title)
            {
                QuestionIdentifier = questionId;
                Title = title;
            }

            #endregion

            #region Public methods

            public AxisItem CreateCopy() => new AxisItem(QuestionIdentifier, Title);

            #endregion

            #region Overriden methods

            public override bool Equals(object obj)
            {
                if (!(obj is AxisItem))
                    return false;

                var other = (AxisItem)obj;

                return other != null && QuestionIdentifier == other.QuestionIdentifier;
            }

            public override int GetHashCode()
            {
                int result;

                unchecked
                {
                    result = QuestionIdentifier.GetHashCode();
                }

                return result;
            }

            #endregion
        }

        [Serializable]
        public sealed class AxisItemCollection : IEnumerable<AxisItem>
        {
            #region Fields

            private List<AxisItem> _list;

            #endregion

            #region Properties

            public int Count => _list.Count;

            #endregion

            #region Constructor

            public AxisItemCollection()
            {
                _list = new List<AxisItem>();
            }

            #endregion

            #region Public methods

            public void Add(AxisItem item)
            {
                if (item == null)
                    return;

                var itemIndex = _list.IndexOf(item);

                if (itemIndex >= 0)
                    _list[itemIndex] = item;
                else
                    _list.Add(item);
            }

            public void Add(Guid questionId, string title) => Add(new AxisItem(questionId, title));

            public void Remove(Guid questionId) => _list.Remove(new AxisItem(questionId, null));

            public void Clear() => _list.Clear();

            public void CopyFrom(IEnumerable<AxisItem> array)
            {
                _list.Clear();

                foreach (var item in array)
                    _list.Add(item.CreateCopy());
            }

            #endregion

            #region IEnumerable

            public IEnumerator<AxisItem> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Properties

        public Guid? SurveyFormIdentifier { get; set; }
        public bool ShowFrequencies { get; set; }
        public bool ShowRowPercentages { get; set; }
        public bool ShowColumnPercentages { get; set; }

        public AxisItemCollection XAxis { get; }
        public AxisItemCollection YAxis { get; }

        #endregion

        #region Construction

        public QResponseCorrelationAnalysisFilter()
        {
            XAxis = new AxisItemCollection();
            YAxis = new AxisItemCollection();
        }

        #endregion
    }
}
