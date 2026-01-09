using System;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Quizzes
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes/delete";

        public static string GetNavigateUrl(Guid quizId) => NavigateUrl + "?quiz=" + quizId;

        public static void Redirect(Guid quizId) => HttpResponseHelper.Redirect(GetNavigateUrl(quizId));

        private Guid? QuizIdentifier => Guid.TryParse(Request["quiz"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanDelete)
                Search.Redirect();

            var quiz = GetEntity();
            if (quiz == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, quiz.QuizName);

            QuizType.InnerText = quiz.QuizType;
            QuizName.InnerText = quiz.QuizName;

            var attemptCount = ServiceLocator.QuizAttemptSearch.Count(new TQuizAttemptFilter
            {
                QuizIdentifier = QuizIdentifier.Value
            });

            AttemptCount.InnerText = "TO DO"; //$"{attemptCount:n0}";

            CancelButton.NavigateUrl = Edit.GetNavigateUrl(QuizIdentifier.Value);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ConfirmCheckBox.Checked)
            {
                var quiz = GetEntity();
                if (quiz != null)
                {
                    ServiceLocator.QuizAttemptStore.DeleteByQuizId(quiz.QuizIdentifier);
                    ServiceLocator.QuizStore.Delete(quiz.QuizIdentifier);
                }
            }

            Search.Redirect();
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"quiz={QuizIdentifier}"
                : null;
        }

        private TQuiz GetEntity()
        {
            var id = QuizIdentifier;
            var entity = id.HasValue ? ServiceLocator.QuizSearch.Select(id.Value) : null;
            return entity?.OrganizationIdentifier == Organization.Identifier ? entity : null;
        }
    }
}