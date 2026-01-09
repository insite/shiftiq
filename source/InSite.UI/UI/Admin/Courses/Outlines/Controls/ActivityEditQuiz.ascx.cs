using System;

using InSite.Application.Courses.Read;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditQuiz : BaseActivityEdit
    {
        private Guid? OriginalQuizIdentifier
        {
            get => (Guid?)ViewState[nameof(OriginalQuizIdentifier)];
            set => ViewState[nameof(OriginalQuizIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuizIdentifier.AutoPostBack = true;
            QuizIdentifier.ValueChanged += QuizIdentifier_ValueChanged;

            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);
        }

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        protected override void BindModelToControls(QActivity activity)
        {
            OriginalQuizIdentifier = activity.QuizIdentifier;
            QuizIdentifier.ValueAsGuid = activity.QuizIdentifier;

            LoadQuiz(activity.QuizIdentifier);
        }

        private void LoadQuiz(Guid? quizId)
        {
            var quiz = quizId.HasValue ? ServiceLocator.QuizSearch.Select(quizId.Value) : null;
            var hasQuiz = quiz != null;

            QuizFields.Visible = hasQuiz;
            QuizLink.Visible = hasQuiz;

            if (!hasQuiz)
                return;

            var isSpeed = quiz.QuizType == QuizType.TypingSpeed;
            var isAccuracy = quiz.QuizType == QuizType.TypingAccuracy;

            QuizLink.NavigateUrl = InSite.UI.Admin.Assessments.Quizzes.Edit.GetNavigateUrl(quiz.QuizIdentifier);
            QuizName.Text = quiz.QuizName;

            PassingWpmField.Visible = isSpeed;
            PassingWpm.ValueAsInt = quiz.PassingWpm;

            PassingKphField.Visible = isAccuracy;
            PassingKph.ValueAsInt = quiz.PassingKph;

            PassingAccuracy.ValueAsDecimal = quiz.PassingAccuracy * 100m;
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            var quizId = QuizIdentifier.ValueAsGuid;

            if (quizId.HasValue && !SaveQuiz(quizId.Value))
                quizId = null;

            activity.QuizIdentifier = quizId;
        }

        private bool SaveQuiz(Guid quizId)
        {
            var quiz = ServiceLocator.QuizSearch.Select(quizId);
            if (quiz == null)
                return false;

            quiz.QuizName = QuizName.Text;

            if (PassingWpm.Visible)
                quiz.PassingWpm = PassingWpm.ValueAsInt ?? 0;

            if (PassingKph.Visible)
                quiz.PassingKph = PassingKph.ValueAsInt ?? 0;

            quiz.PassingAccuracy = (PassingAccuracy.ValueAsDecimal ?? 0) / 100m;

            ServiceLocator.QuizStore.Update(quiz);

            return true;
        }

        private void QuizIdentifier_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var quizId = QuizIdentifier.ValueAsGuid;

            if (quizId.HasValue && CourseSearch.ActivityExists(x => x.ActivityIdentifier != ActivityIdentifier && x.QuizIdentifier == quizId.Value))
            {
                var quiz = ServiceLocator.QuizSearch.Select(quizId.Value);

                ScreenStatus.AddMessage(AlertType.Error, $"<strong>{quiz?.QuizName}</strong> is already assigned to another course activity.");

                QuizIdentifier.ValueAsGuid = quizId = OriginalQuizIdentifier;
            }

            LoadQuiz(quizId);
        }
    }
}