using System;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Attempts.Reports.Forms
{
    public partial class DeleteAttempt : AdminBasePage
    {
        private Guid AttemptID => Guid.TryParse(Request["attempt"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = "/ui/admin/assessments/attempts/reports/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var attempt = ServiceLocator.AttemptSearch.GetAttempt(AttemptID, x => x.Form, x => x.LearnerPerson);
            if (attempt == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);
                return;
            }

            PageHelper.AutoBindHeader(this, null, $"{attempt.LearnerPerson.UserFullName} <span class='form-text'>{attempt.Form.FormTitle}</span>");

            AnswerCount.Text = ServiceLocator.AttemptSearch.CountAttemptQuestions(AttemptID).ToString("n0");

            var student = ServiceLocator.PersonSearch.GetPerson(attempt.LearnerPerson.UserIdentifier, attempt.OrganizationIdentifier, x => x.User);
            FullName.Text = $"<a href=\"/ui/admin/contacts/people/edit?contact={student.User.UserIdentifier}\">{student.User.FullName}</a>";

            var bank = ServiceLocator.BankSearch.GetBankState(attempt.Form.BankIdentifier);
            if (bank == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            BankName.Text = $"<a href=\"/ui/admin/assessments/banks/outline?bank={bank.Identifier}\">{bank.Name}</a>";

            var form = bank.FindForm(attempt.FormIdentifier);
            if (form == null)
                HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search", true);

            AttemptStarted.Text = attempt.AttemptStarted.Format(User.TimeZone, nullValue: "N/A");
            AttemptGraded.Text = attempt.AttemptGraded.Format(User.TimeZone, nullValue: "N/A");
            AttemptTimeTaken.Text = attempt.AttemptDuration.HasValue
                ? ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second)
                : "N/A";

            AttemptPoints.Text = attempt.AttemptGraded.HasValue ? $"{attempt.AttemptPoints:n2} / {attempt.FormPoints:n2}" : "N/A";
            AttemptScore.Text = attempt.AttemptGraded.HasValue ? $"{attempt.AttemptScore:p0}" : "N/A";

            if (attempt.AttemptGraded.HasValue)
                AttemptIsPassing.Text = attempt.AttemptIsPassing
                    ? "<span class='badge bg-success'>Pass</span>"
                    : "<span class='badge bg-danger'>Fail</span>";
            else
                AttemptIsPassing.Text = "N/A";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var attempt = ServiceLocator.AttemptSearch.GetAttempt(AttemptID);
            if (attempt != null)
            {
                ServiceLocator.SendCommand(new VoidAttempt(AttemptID, "Attempt Deleted"));

                var form = ServiceLocator.BankSearch.GetForm(attempt.FormIdentifier);
                if (form != null)
                    ServiceLocator.SendCommand(new AnalyzeForm(form.BankIdentifier, form.FormIdentifier));
            }

            HttpResponseHelper.Redirect("/ui/admin/assessments/attempts/reports/search");
        }
    }
}