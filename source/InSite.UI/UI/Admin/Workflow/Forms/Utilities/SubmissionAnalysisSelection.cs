using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Utilities
{
    public partial class SubmissionAnalysis
    {
        [Serializable]
        public class SelectionItem
        {
            public Guid QuestionID { get; }
            public Guid? OptionID { get; }
            public bool AnswerCheck { get; }
            public int AnswerFrequency { get; set; }
            public double AnswerScore { get; }

            public SelectionItem(Guid questionId, Guid optionId, int answerFrequencey, double answerScore)
            {
                QuestionID = questionId;
                OptionID = optionId;
                AnswerFrequency = answerFrequencey;
                AnswerScore = answerScore;
            }

            public SelectionItem(ResponseAnalysisSelectionItem item)
            {
                QuestionID = item.QuestionIdentifier;
                OptionID = item.OptionIdentifier;
                AnswerFrequency = item.AnswerCount;
                AnswerScore = (double?)item.OptionPoints ?? 0d;
            }
        }

        [Serializable]
        public class SelectionList
        {
            #region Fields

            private readonly List<SelectionItem> _items = new List<SelectionItem>();

            #endregion

            #region Properties

            public int Frequency => _items.Sum(x => x.AnswerFrequency);

            public int Score => (int)_items.Sum(x => x.AnswerFrequency * x.AnswerScore);

            public int Count => _items.Count;

            #endregion

            #region Public methods

            public double CalculateValidMean(IEnumerable<Guid> options)
            {
                var sum = 0d;
                var count = 0;

                for (var i = 0; i < _items.Count; i++)
                {
                    var analysis = _items[i];
                    if (analysis.OptionID.HasValue && options.Any(x => x == analysis.OptionID.Value))
                    {
                        sum += analysis.AnswerScore * analysis.AnswerFrequency;
                        count += analysis.AnswerFrequency;
                    }
                }

                return count > 0 ? Math.Round(sum / count, 2) : 0;
            }

            public double CalculateStandardDeviation(Guid questionId, double validCount)
            {
                var scores = GetScores(questionId, validCount);

                return Calculator.CalculateStandardDeviation(scores.ToArray());
            }

            public int IndexOfQuestionAndAnswer(Guid questionId, Guid optionId)
            {
                for (int i = 0; i < Count; i++)
                {
                    var item = _items[i];
                    if (item.QuestionID == questionId && item.OptionID == optionId)
                        return i;
                }

                return -1;
            }

            public void Add(Guid questionId, Guid optionId, double answerScore)
            {
                var i = IndexOfQuestionAndAnswer(questionId, optionId);

                if (i > -1)
                    _items[i].AnswerFrequency++;
                else
                    _items.Add(new SelectionItem(questionId, optionId, 1, answerScore));
            }

            public void Add(SelectionItem item)
            {
                _items.Add(item);
            }

            public SelectionItem GetAnalysisForQuestionAndAnswer(Guid questionId, Guid optionId)
            {
                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item != null && questionId == item.QuestionID && item.OptionID == optionId)
                        return item;
                }

                return null;
            }

            public SelectionItem GetAnalysisForQuestionAndAnswer(Guid questionId, bool answerCheck)
            {
                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item != null && questionId == item.QuestionID && item.AnswerCheck == answerCheck)
                        return item;
                }

                return null;
            }

            #endregion

            #region Helpers

            private List<double> GetScores(Guid questionId, double validCount)
            {
                var result = new List<double>();

                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];

                    if (item.QuestionID == questionId && item.AnswerFrequency > 0 && validCount > 0)
                    {
                        for (var j = 0; j < item.AnswerFrequency; j++)
                            result.Add(Convert.ToDouble(item.AnswerScore));
                    }
                }

                return result;
            }

            #endregion
        }
    }
}