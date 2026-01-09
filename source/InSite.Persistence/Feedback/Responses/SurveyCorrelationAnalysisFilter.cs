using System;
using System.Collections;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class CorrelationAnalysisFilter : Filter
    {
        #region Classes

        [Serializable]
        public sealed class AxisItem
        {
            #region Properties

            public int QuestionId { get; private set; }
            public int FieldId { get; private set; }
            public string Title { get; private set; }

            #endregion

            #region Constructor

            private AxisItem()
            {

            }

            public AxisItem(int questionId, int fieldId, string title)
            {
                QuestionId = questionId;
                FieldId = fieldId;
                Title = title;
            }

            #endregion

            #region Public methods

            public AxisItem CreateCopy()
            {
                return new AxisItem(QuestionId, FieldId, Title);
            }

            #endregion

            #region Overriden methods

            public override bool Equals(object obj)
            {
                if (!(obj is AxisItem))
                    return false;

                var other = (AxisItem)obj;

                return other != null && QuestionId == other.QuestionId && FieldId == other.FieldId;
            }

            public override int GetHashCode()
            {
                int result;

                unchecked
                {
                    result = QuestionId;
                    result = 31 * result + FieldId;
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

            public int Count
            {
                get { return _list.Count; }
            }

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

            public void Add(int questionId, int fieldId, string title)
            {
                var item = new AxisItem(questionId, fieldId, title);

                Add(item);
            }

            public void Remove(int questionId, int fieldId)
            {
                var item = new AxisItem(questionId, fieldId, null);

                _list.Remove(item);
            }

            public void Clear()
            {
                _list.Clear();
            }

            public int[] GetFieldIdArray()
            {
                var result = new List<int>();

                foreach (var item in _list)
                    result.Add(item.FieldId);

                return result.ToArray();
            }

            public void CopyFrom(IEnumerable<AxisItem> array)
            {
                _list.Clear();

                foreach (var item in array)
                    _list.Add(item.CreateCopy());
            }

            #endregion

            #region IEnumerable

            public IEnumerator<AxisItem> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region Properties

        public int? SurveyId { get; set; }
        public bool ShowFrequencies { get; set; }
        public bool ShowRowPercentages { get; set; }
        public bool ShowColumnPercentages { get; set; }

        public AxisItemCollection XAxis { get; private set; }
        public AxisItemCollection YAxis { get; private set; }

        #endregion

        #region Construction

        public CorrelationAnalysisFilter()
        {
            XAxis = new AxisItemCollection();
            YAxis = new AxisItemCollection();
        }

        #endregion
    }
}
