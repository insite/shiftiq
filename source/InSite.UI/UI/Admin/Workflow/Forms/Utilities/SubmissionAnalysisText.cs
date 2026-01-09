using System;
using System.Collections;
using System.Collections.Generic;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public partial class SubmissionAnalysis
    {
        [Serializable]
        public class TextItem
        {
            public Guid ResponseSessionID { get; set; }
            public Guid QuestionID { get; set; }
            public string AnswerText { get; set; }
        }

        [Serializable]
        public class TextList : IEnumerable<TextItem>
        {
            private readonly List<TextItem> _items;

            public int Count => _items.Count;

            public TextItem this[int index] => _items[index];

            public TextList()
            {
                _items = new List<TextItem>();
            }

            public void Add(TextItem item)
            {
                _items.Add(item);
            }

            public IEnumerator<TextItem> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}