using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Quizzes
{
    public partial class Create : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes/create";

        public static string GetNavigateUrl(string type = null)
        {
            var url = NavigateUrl;

            if (type.IsNotEmpty())
                url += "?type=" + HttpUtility.UrlEncode(type);

            return url;
        }

        public static void Redirect(string type = null) => HttpResponseHelper.Redirect(GetNavigateUrl(type));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuizDetails.TypeChanged += (x, y) => OnQuizTypeChanged(y.Value);

            MaxSizeValidator.ServerValidate += MaxSizeValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                Search.Redirect();

            PageHelper.AutoBindHeader(this);

            var defaultType = QuizHelper.TypeFromQueryValue(Request.QueryString["type"]);

            QuizDetails.SetDefaultInputValues(defaultType);
            OnQuizTypeChanged(defaultType);

            CancelButton.NavigateUrl = Search.GetNavigateUrl();
        }

        private void OnQuizTypeChanged(string value)
        {
            var isTypingSpeed = value == QuizType.TypingSpeed;
            var isTypingAccuracy = value == QuizType.TypingAccuracy;

            TypingSpeedSection.Visible = isTypingSpeed;
            TypingAccuracySection.Visible = isTypingAccuracy;

            if (isTypingSpeed)
                TypingSpeed.SetData(new string[0]);
            else if (isTypingAccuracy)
                TypingAccuracy.SetData(new TQuizTypingAccuracyQuestion[0]);
        }

        private void MaxSizeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var quiz = new TQuiz();
            QuizDetails.GetInputValues(quiz);

            var quizData = GetQuizData(quiz.QuizType);

            args.IsValid = string.IsNullOrEmpty(quizData) || quizData.Length <= TQuiz.MaxQuizDataLength;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var quiz = CreateQuiz();

            ServiceLocator.QuizStore.Insert(quiz);

            quiz.GradebookIdentifier = CreateGradebook(quiz);

            ServiceLocator.QuizStore.Update(quiz);

            Edit.Redirect(quiz.QuizIdentifier, "saved");
        }

        private TQuiz CreateQuiz()
        {
            var quiz = new TQuiz
            {
                QuizIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };

            QuizDetails.GetInputValues(quiz);

            quiz.QuizData = GetQuizData(quiz.QuizType);

            return quiz;
        }

        private string GetQuizData(string quizType)
        {
            object data;

            if (quizType == QuizType.TypingSpeed)
                data = TypingSpeed.GetData();
            else if (quizType == QuizType.TypingAccuracy)
                data = TypingAccuracy.GetData();
            else
                throw ApplicationError.Create("Unexpected quiz type: " + quizType);

            return JsonConvert.SerializeObject(data);
        }

        private Guid CreateGradebook(TQuiz quiz)
        {
            var gradebookId = UniqueIdentifier.Create();
            var commands = new List<Command>();

            var titlePrefix = $"{quiz.QuizType} Quiz: ";
            var titlePostfix = $" ({quiz.QuizIdentifier})";
            var maxNameLength = 100 - titlePostfix.Length - titlePrefix.Length;

            commands.Add(new CreateGradebook(
                gradebookId,
                Organization.OrganizationIdentifier,
                titlePrefix + quiz.QuizName.MaxLength(maxNameLength, true) + titlePostfix,
                GradebookType.Scores,
                null,
                null,
                null));

            if (quiz.QuizType == QuizType.TypingSpeed || quiz.QuizType == QuizType.TypingAccuracy)
            {
                AddGradebookScoreGradeItem(QuizGradeItem.Mistakes);
                AddGradebookScoreGradeItem(QuizGradeItem.WordsPerMin);
                AddGradebookScoreGradeItem(QuizGradeItem.CharsPerMin);
                AddGradebookScoreGradeItem(QuizGradeItem.Accuracy);
                AddGradebookScoreGradeItem(QuizGradeItem.Speed);
            }

            if (quiz.QuizType == QuizType.TypingAccuracy)
            {
                AddGradebookScoreGradeItem(QuizGradeItem.KeystrokesPerHour);
            }

            ServiceLocator.SendCommands(commands);

            return gradebookId;

            void AddGradebookScoreGradeItem(QuizGradeItem item)
            {
                commands.Add(new AddGradeItem(
                    gradebookId,
                    UniqueIdentifier.Create(),
                    item.Code,
                    item.FullName,
                    item.ShortName,
                    true,
                    item.Format,
                    GradeItemType.Score,
                    GradeItemWeighting.None,
                    null,
                    null
                ));
            }
        }
    }
}