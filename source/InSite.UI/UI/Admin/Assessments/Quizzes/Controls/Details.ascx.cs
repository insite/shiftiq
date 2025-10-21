using System;

using InSite.Application.Quizzes.Read;
using InSite.Common.Web.UI;

using Shift.Common.Events;

namespace InSite.UI.Admin.Assessments.Quizzes.Controls
{
    public partial class Details : BaseUserControl
    {
        public event StringValueHandler TypeChanged;

        private void OnTypeChanged(string type) =>
            TypeChanged?.Invoke(this, new StringValueArgs(type));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuizType.AutoPostBack = true;
            QuizType.ValueChanged += (x, y) => OnTypeChanged(y.NewValue);
        }

        public void SetDefaultInputValues(string quizType)
        {
            QuizType.Enabled = true;
            QuizType.Value = quizType;
        }

        public void SetInputValues(TQuiz quiz)
        {
            var isTypingSpeed = quiz.QuizType == Shift.Constant.QuizType.TypingSpeed;
            var isTypingAccuracy = quiz.QuizType == Shift.Constant.QuizType.TypingAccuracy;

            QuizType.Value = quiz.QuizType;
            QuizType.Enabled = false;
            QuizName.Text = quiz.QuizName;
            TimeLimit.ValueAsInt = quiz.TimeLimit;
            AttemptLimit.ValueAsInt = quiz.AttemptLimit;

            MaximumPoints.ValueAsDecimal = quiz.MaximumPoints;
            PassingPoints.ValueAsDecimal = quiz.PassingPoints;
            PassingScore.ValueAsInt = (int?)(quiz.PassingScore * 100m);

            PassingWpmField.Visible = isTypingSpeed;
            PassingWpm.ValueAsInt = quiz.PassingWpm;

            PassingKphField.Visible = isTypingAccuracy;
            PassingKph.ValueAsInt = quiz.PassingKph;

            PassingAccuracy.ValueAsDecimal = quiz.PassingAccuracy * 100m;
        }

        public void GetInputValues(TQuiz quiz)
        {
            if (QuizType.Enabled || string.IsNullOrEmpty(quiz.QuizType))
                quiz.QuizType = QuizType.Value;

            quiz.QuizName = QuizName.Text;
            quiz.TimeLimit = TimeLimit.ValueAsInt ?? 0;
            quiz.AttemptLimit = AttemptLimit.ValueAsInt ?? 0;

            quiz.MaximumPoints = MaximumPoints.ValueAsDecimal;
            quiz.PassingPoints = PassingPoints.ValueAsDecimal;
            quiz.PassingScore = PassingScore.ValueAsDecimal / 100m;

            if (PassingWpm.Visible)
                quiz.PassingWpm = PassingWpm.ValueAsInt ?? 0;

            if (PassingKph.Visible)
                quiz.PassingKph = PassingKph.ValueAsInt ?? 0;

            quiz.PassingAccuracy = (PassingAccuracy.ValueAsDecimal ?? 0) / 100m;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            QuizTypeValidator.ValidationGroup = groupName;
            QuizNameValidator.ValidationGroup = groupName;
            TimeLimitValidator.ValidationGroup = groupName;
            AttemptLimitValidator.ValidationGroup = groupName;
        }
    }
}