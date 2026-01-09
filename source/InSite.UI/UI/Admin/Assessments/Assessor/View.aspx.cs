using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.UI.Admin.Assessments.Attempts.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Assessor
{
    public partial class View : AdminBasePage
    {
        #region Properties

        private Guid? AttemptIdentifier => Guid.TryParse(Request["attempt"], out Guid value) ? value : (Guid?)null;

        private string DefaultPanel => Request.QueryString["panel"];

        private string DefaultStatus => Request.QueryString["status"];

        protected DateTimeOffset? AttemptGraded
        {
            get => (DateTimeOffset?)ViewState[nameof(AttemptGraded)];
            set => ViewState[nameof(AttemptGraded)] = value;
        }

        protected bool IsAttemptImported
        {
            get => (bool)(ViewState[nameof(IsAttemptImported)] ?? false);
            set => ViewState[nameof(IsAttemptImported)] = value;
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open(true);

            if (string.Equals(DefaultStatus, "graded", StringComparison.OrdinalIgnoreCase))
                ViewerStatus.AddMessage(AlertType.Success, "The attempt was successfully graded");
        }

        #endregion

        #region Data binding

        private void Open(bool bindRepeaters)
        {
            if (!AttemptIdentifier.HasValue)
                RedirectToSearch();

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(AttemptIdentifier.Value, x => x.Form, x => x.LearnerPerson);
            if (attempt == null)
                RedirectToSearch();

            var bank = ServiceLocator.BankSearch.GetBankState(attempt.Form.BankIdentifier);
            if (bank == null)
                RedirectToSearch();

            var form = bank.FindForm(attempt.FormIdentifier);
            if (form == null)
                RedirectToSearch();

            var headerQualifier = Organization.Toolkits.Assessments.ShowPersonNameToGradingAssessor
                ? $"{attempt.LearnerPerson.UserFullName} <span class='form-text'>{attempt.LearnerPerson.PersonCode}</span>"
                : attempt.LearnerPerson.PersonCode;

            PageHelper.AutoBindHeader(this, null, headerQualifier);

            AttemptGraded = attempt.AttemptGraded;
            IsAttemptImported = attempt.AttemptImported.HasValue;

            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(AttemptIdentifier.Value);

            if (bindRepeaters)
                BindRepeaters(attempt, questions);
        }

        private static void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/assessments/assessor/search", true);

        private void BindRepeaters(QAttempt attempt, List<QAttemptQuestion> questions) => BindRubrics(attempt, questions);

        private void BindRubrics(QAttempt attempt, List<QAttemptQuestion> questions)
        {
            var hasOldRubrics = Grade.HasOldRubrics(attempt.AttemptStarted);

            var composedName1 = QuestionItemType.ComposedEssay.GetName();
            var composedName2 = QuestionItemType.ComposedVoice.GetName();
            var composedQuestions = questions.Where(x => x.QuestionType == composedName1 || x.QuestionType == composedName2).ToArray();

            Rubrics.LoadData(composedQuestions);

            var gradeUrl = $"/ui/admin/assessments/assessor/grade?attempt={AttemptIdentifier}";

            GradeButton.Visible = attempt.AttemptSubmitted.HasValue && attempt.AttemptGraded == null;
            GradeButton.NavigateUrl = new ReturnUrl($"/ui/admin/assessments/assessor/view?attempt={AttemptIdentifier}")
                .GetRedirectUrl(gradeUrl);

            RegradeButton.Visible = attempt.AttemptSubmitted.HasValue
                && attempt.AttemptGraded.HasValue
                && Identity.IsGranted(ActionName.Admin_Assessments_Attempts_Grade_Regrade);

            RegradeButton.NavigateUrl = gradeUrl;

            if (composedQuestions.Length > 0 && hasOldRubrics && attempt.AttemptGraded == null)
                ViewerStatus.AddMessage(AlertType.Warning, "This attempt was created before new rubrics were introduced and therefore it cannot be graded");
        }

        #endregion
    }
}