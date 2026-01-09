using System;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Rubrics.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Delete : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/records/rubrics/delete";

        public static string GetNavigateUrl(Guid rubricId)
        {
            return NavigateUrl + "?rubric=" + rubricId;
        }

        public static void Redirect(Guid rubricId) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(rubricId));

        private Guid RubricId => Guid.TryParse(Request["rubric"], out var value) ? value : Guid.Empty;

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

            PageHelper.AutoBindHeader(this);

            LoadData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!ServiceLocator.RubricSearch.RubricHasAttempts(RubricId))
                ServiceLocator.SendCommand(new DeleteRubric(RubricId));

            Search.Redirect();
        }

        private void LoadData()
        {
            var rubric = ServiceLocator.RubricSearch.GetRubric(RubricId);
            if (rubric == null || rubric.OrganizationIdentifier != Organization.Identifier)
                Search.Redirect();

            RubricTitle.Text = rubric.RubricTitle;
            RubricDescription.Text = rubric.RubricDescription;
            RubricPoints.Text = $"{rubric.RubricPoints:n2}";

            var attemptsCount = ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter
            {
                RubricIdentifier = rubric.RubricIdentifier
            });
            var questionCount = ServiceLocator.BankSearch.CountQuestions(new QBankQuestionFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                RubricIdentifier = rubric.RubricIdentifier
            });
            var criteriaCount = ServiceLocator.RubricSearch.CountCriteria(rubric.RubricIdentifier);

            CriteriaCount.Text = criteriaCount.ToString("n0");
            QuestionsCount.Text = questionCount.ToString("n0");
            AttemptsCount.Text = attemptsCount.ToString("n0");

            var isLocked = attemptsCount > 0;
            if (isLocked)
                AlertStatus.AddMessage(AlertType.Warning, "The rubric cannot be deleted because is used in one or more attempts.");

            DeleteButton.Visible = !isLocked;
            ConfirmAlert.Visible = !isLocked;

            CancelButton.NavigateUrl = Search.NavigateUrl;
        }
    }
}