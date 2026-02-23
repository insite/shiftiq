using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class SelectionAnalysisList
    {
        #region Fields

        private readonly List<SelectionAnalysisItem> _items = new List<SelectionAnalysisItem>();

        #endregion

        #region Properties

        public int Frequency => _items.Sum(x => x.AnswerFrequency);

        public int Score => _items.Sum(x => x.AnswerFrequency * x.AnswerScore);

        public int Count => _items.Count;

        #endregion

        #region Public methods

        public double CalculateValidMean(int questionId, double validCount)
        {
            var result = 0d;

            if (validCount > 0)
            {
                for (var i = 0; i < _items.Count; i++)
                {
                    var analysis = _items[i];
                    if (analysis.QuestionId != questionId || analysis.AnswerFrequency <= 0)
                        continue;

                    var weightedScore = analysis.AnswerScore / validCount;
                    result += weightedScore * analysis.AnswerFrequency;
                }
            }

            return Math.Round(result, 2);
        }

        public double CalculateStandardDeviation(int questionId, double validCount)
        {
            var scores = GetScores(questionId, validCount);

            return Calculator.CalculateStandardDeviation(scores.ToArray());
        }

        public int IndexOfQuestionAndAnswer(int questionId, int optionId)
        {
            for (int i = 0; i < Count; i++)
            {
                var item = _items[i];
                if (item.QuestionId == questionId && item.OptionId == optionId)
                    return i;
            }

            return -1;
        }

        public void Add(int questionId, int optionId, int answerScore)
        {
            var i = IndexOfQuestionAndAnswer(questionId, optionId);

            if (i > -1)
                _items[i].AnswerFrequency++;
            else
                _items.Add(new SelectionAnalysisItem(questionId, optionId, 1, answerScore));
        }

        public void Add(int questionId, bool answerCheck, int answerFrequencey, int answerScore)
        {
            _items.Add(new SelectionAnalysisItem(questionId, answerCheck, answerFrequencey, answerScore));
        }

        public void Add(SelectionAnalysisItem item)
        {
            _items.Add(item);
        }

        public SelectionAnalysisItem GetAnalysisForQuestionAndAnswer(int questionId, int optionId)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item != null && questionId == item.QuestionId && item.OptionId == optionId)
                    return item;
            }

            return null;
        }

        public SelectionAnalysisItem GetAnalysisForQuestionAndAnswer(int questionId, bool answerCheck)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item != null && questionId == item.QuestionId && item.AnswerCheck == answerCheck)
                    return item;
            }

            return null;
        }

        #endregion

        #region Helpers

        private List<double> GetScores(int questionId, double validCount)
        {
            var result = new List<double>();

            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if (item.QuestionId == questionId && item.AnswerFrequency > 0 && validCount > 0)
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
