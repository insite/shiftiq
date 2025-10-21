using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionMatchingDetails : BaseUserControl
    {
        #region Loading

        protected override void SetupValidationGroup(string groupName)
        {
            Answers.ValidationGroup = ValidationGroup;
        }

        #endregion

        #region Setting and getting input values

        public void InitData()
        {
            var matchingList = new MatchingList { Pairs = new List<MatchingPair>() };

            matchingList.Pairs.Add(new MatchingPair());

            Answers.LoadData(matchingList);
        }

        public void SetInputValues(Question question)
        {
            FeedbackForEveryone.Text = question.Content.Rationale;
            FeedbackForCorrectAnswers.Text = question.Content.RationaleOnCorrectAnswer;
            FeedbackForWrongAnswers.Text = question.Content.RationaleOnIncorrectAnswer;

            Answers.LoadData(question.Matches ?? new MatchingList());

            SetDistractors(question.Matches.Distractors);
        }

        private void SetDistractors(List<ContentTitle> distractors)
        {
            var distractorsText = new MultilingualString();
            foreach (var distractor in distractors)
            {
                foreach (var language in distractor.Title.Languages)
                {
                    var newLine = distractor.Title[language];
                    if (string.IsNullOrEmpty(newLine))
                        continue;

                    var text = distractorsText[language];
                    if (string.IsNullOrEmpty(text))
                        text = newLine;
                    else
                        text += "\r\n" + newLine;

                    distractorsText[language] = text;
                }
            }

            Distractors.Text = distractorsText;
        }

        public MatchingList GetMatches()
        {
            var result = new MatchingList();

            Answers.Update(result);

            GetDistractors(result.Distractors);

            return result;
        }

        private void GetDistractors(List<ContentTitle> distractors)
        {
            foreach (var language in Distractors.Text.Languages)
            {
                var distractor = Distractors.Text[language];
                if (string.IsNullOrEmpty(distractor))
                    continue;

                var distractorParts = distractor.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var distractorPart in distractorParts)
                {
                    var trimmed = distractorPart.Trim();
                    if (string.IsNullOrEmpty(trimmed))
                        continue;

                    var distractorContent = new ContentTitle();
                    distractorContent.Title[language] = trimmed;

                    distractors.Add(distractorContent);
                }
            }
        }

        public MultilingualString GetRationale()
        {
            return TrimText(FeedbackForEveryone.Text);
        }

        public MultilingualString GetRationaleOnCorrectAnswer()
        {
            return TrimText(FeedbackForCorrectAnswers.Text);
        }

        public MultilingualString GetRationaleOnIncorrectAnswer()
        {
            return TrimText(FeedbackForWrongAnswers.Text);
        }

        private static MultilingualString TrimText(MultilingualString multiText)
        {
            var result = new MultilingualString();

            foreach (var language in multiText.Languages)
            {
                var text = multiText[language]?.Trim();
                if (!string.IsNullOrEmpty(text))
                    result[language] = text;
            }

            return result;
        }

        #endregion
    }
}