using System;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Quizzes
{
    public partial class Edit : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes/edit";

        public static string GetNavigateUrl(Guid quizId, string status = null)
        {
            var url = NavigateUrl + "?quiz=" + quizId;

            if (status.IsNotEmpty())
                url += "&status=" + HttpUtility.UrlEncode(status);

            return url;
        }

        public static void Redirect(Guid quizId, string status = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(quizId, status));

        private Guid QuizIdentifier => Guid.TryParse(Request["quiz"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MaxSizeValidator.ServerValidate += MaxSizeValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            DeleteButton.NavigateUrl = Delete.GetNavigateUrl(QuizIdentifier);
            CancelButton.NavigateUrl = Search.GetNavigateUrl();
        }

        private void Open()
        {
            var entity = ServiceLocator.QuizSearch.Select(QuizIdentifier);
            if (entity == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(Page, null, entity.QuizName);

            QuizDetails.SetInputValues(entity);

            var isTypingSpeed = entity.QuizType == QuizType.TypingSpeed;
            var isTypingAccuracy = entity.QuizType == QuizType.TypingAccuracy;

            TypingSpeedSection.Visible = isTypingSpeed;
            TypingAccuracySection.Visible = isTypingAccuracy;

            if (isTypingSpeed)
            {
                var data = JsonConvert.DeserializeObject<string[]>(entity.QuizData);
                TypingSpeed.SetData(data);
            }
            else if (isTypingAccuracy)
            {
                var data = JsonConvert.DeserializeObject<TQuizTypingAccuracyQuestion[]>(entity.QuizData);
                TypingAccuracy.SetData(data);
            }
            else
                throw ApplicationError.Create("Unexpected quiz type: " + entity.QuizType);
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

            var entity = ServiceLocator.QuizSearch.Select(QuizIdentifier);

            GetInputValues(entity);

            ServiceLocator.QuizStore.Update(entity);

            Search.Redirect();
        }

        private void GetInputValues(TQuiz entity)
        {
            QuizDetails.GetInputValues(entity);

            entity.QuizData = GetQuizData(entity.QuizType);
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
    }
}