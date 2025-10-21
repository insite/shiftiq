using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assessments
{
    public partial class Dashboard : AdminBasePage
    {
        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var bankCount = ServiceLocator.BankSearch.CountBanks(new QBankFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(BankCounter, BankCount, bankCount, BankLink, "/ui/admin/assessments/banks/search");

            var specsCount = ServiceLocator.BankSearch.Count(new QBankSpecificationFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(SpecCounter, SpecCount, specsCount, SpecLink, "/ui/admin/assessments/specifications/search");

            var formsCount = ServiceLocator.BankSearch.CountForms(new QBankFormFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(FormsCounter, FormsCount, formsCount, FormsLink, "/ui/admin/assessments/forms/search");

            var questionsCount = ServiceLocator.BankSearch.CountQuestions(new QBankQuestionFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(QuestionsCounter, QuestionsCount, questionsCount, QuestionsLink, "/ui/admin/assessments/questions/search");

            var bankCommentsCount = ServiceLocator.BankSearch.CountComments(new BankCommentaryFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(BankCommentsCounter, BankCommentsCount, bankCommentsCount, BankCommentsLink, "/ui/admin/assessments/bankscomments/search");

            QuizPanel.Visible = Identity.IsGranted(PermissionNames.Admin_Assessment_Quizzes);

            var quizTypingSpeedCount = ServiceLocator.QuizSearch.Count(new TQuizFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, QuizType = QuizType.TypingSpeed });
            LoadCounter(QuizTypingSpeedCounter, QuizTypingSpeedCount, quizTypingSpeedCount, QuizTypingSpeedLink, Quizzes.Search.GetNavigateUrl("typing-speed"));

            var quizTypingAccuracyCount = ServiceLocator.QuizSearch.Count(new TQuizFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, QuizType = QuizType.TypingAccuracy });
            LoadCounter(QuizTypingAccuracyCounter, QuizTypingAccuracyCount, quizTypingAccuracyCount, QuizTypingAccuracyLink, Quizzes.Search.GetNavigateUrl("typing-accuracy"));

            var quizAttemptCount = ServiceLocator.QuizAttemptSearch.Count(new TQuizAttemptFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(QuizAttemptCounter, QuizAttemptCount, quizAttemptCount, QuizAttemptLink, QuizAttempts.Search.NavigateUrl);

            var attemptsCount = ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter
            {
                FormOrganizationIdentifier = Organization.Identifier,
                CandidateOrganizationIdentifiers = new[] { Organization.Identifier },
                OrganizationPersonTypes = new[] { "Learner", "Administrator" }
            });
            LoadCounter(AttemptsCounter, AttemptsCount, attemptsCount, AttemptsLink, "/ui/admin/assessments/attempts/reports/search");

            var attemptCommentsCount = ServiceLocator.AttemptSearch.CountExaminationFeedback(new QAttemptCommentaryFilter { OrganizationIdentifier = Organization.OrganizationIdentifier });
            LoadCounter(AttemptCommentsCounter, AttemptCommentsCount, attemptCommentsCount, AttemptCommentsLink, "/ui/admin/assessments/attemptscomments/search");

            var publicationAssessmentsCount = ServiceLocator.PageSearch.Count(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.ContentControl == "Assessment");
            LoadCounter(
                PublicationAssessmentsCounter,
                PublicationAssessmentsCount,
                publicationAssessmentsCount,
                PublicationAssessmentsLink,
                "/ui/admin/assessments/publications/search");

            PublicationSection.Visible = PublicationAssessmentsCounter.Visible;

            AttemptSection.Visible = Identity.IsGranted(PermissionIdentifiers.Admin_Assessments_Attempts);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();

            RecentChanges.BindModelToControls(10);
            HistorySection.Visible = RecentChanges.ItemCount > 0;

            UploadAttemptsRow.Visible = Identity.IsGranted(PermissionIdentifiers.Admin_Assessments_Attempts, PermissionOperation.Write);

            PerformanceReportLink.Visible = Organization.Toolkits.Assessments?.PerformanceReport?.Enabled ?? false;
        }

        public void LoadCounter(HtmlGenericControl card, Literal counter, int count, HtmlAnchor link, string action)
        {
            card.Visible = Identity.IsActionAuthorized(action);
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}