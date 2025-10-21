using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class QuestionOption
    {
        public string Text { get; }
        public bool IsCorrectAnswer { get; }

        public QuestionOption(string text, bool isCorrect)
        {
            Text = text;
            IsCorrectAnswer = isCorrect;
        }

        public Option ToBankOption()
        {
            var option = new Option();

            option.Content.Title = new MultilingualString { Default = Text };
            option.Points = IsCorrectAnswer ? 1 : 0;

            return option;
        }
    }
}