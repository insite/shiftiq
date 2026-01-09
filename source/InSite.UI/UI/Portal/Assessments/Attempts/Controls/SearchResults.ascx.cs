using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Portal.Controls;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QAttemptFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Print")
            {
                var attemptId = Grid.GetDataKey<Guid>(e);

                var attempt = ServiceLocator.AttemptSearch.GetAttempt(attemptId);
                if (attempt == null)
                    return;

                var bankForm = ServiceLocator.BankSearch.GetFormData(attempt.FormIdentifier);
                var data = new ReportCompetency.DataSource
                {
                    Attempt = attempt,
                    Questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(attempt.AttemptIdentifier),
                    Bank = bankForm.Specification.Bank,
                    BankForm = bankForm,
                    Learner = ServiceLocator.ContactSearch.GetPerson(attempt.LearnerUserIdentifier, Organization.Identifier),

                    OrganizationIdentifier = Organization.Identifier,
                    LogoImageUrl = PortalHeader.GetLogoImageUrl(CurrentSessionState.Identity, Page.Server),
                    CurrentUrl = Request.Url.Scheme + "://" + Request.Url.Host + Request.RawUrl,
                    Language = CurrentSessionState.Identity.Language,
                    TimeZone = User.TimeZone
                };

                var pdfBytes = ReportCompetency.RenderPdf(data);
                var filename = StringHelper.Sanitize(data.BankForm?.Content.Title.Default ?? "report", '-');

                Response.SendFile(filename, "pdf", pdfBytes);
            }
        }

        protected override int SelectCount(QAttemptFilter filter)
            => ServiceLocator.AttemptSearch.CountAttempts(filter);

        protected override IListSource SelectData(QAttemptFilter filter)
        {
            return ServiceLocator.AttemptSearch
                .GetAttempts(filter, y => y.Form.BankSpecification)
                .ToSearchResult();
        }

        protected string FormatScore()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var calcDisclosure = attempt.Form.BankSpecification.CalcDisclosure.ToEnum<DisclosureType>();

            if (calcDisclosure != DisclosureType.Full && calcDisclosure != DisclosureType.Score || !attempt.AttemptGraded.HasValue)
                return string.Empty;

            var statusHtml = attempt.AttemptIsPassing
                ? "<span class='badge bg-success'>Pass</span>"
                : "<span class='badge bg-danger'>Fail</span>";

            return $"<div>{attempt.AttemptScore:p0}</div>"
                + $"<div class='form-text'>{attempt.AttemptPoints} / {attempt.FormPoints}</div>"
                + statusHtml;
        }

        protected bool IsDisclosureVisible()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            var calcDisclosure = attempt.Form.BankSpecification.CalcDisclosure.ToEnum<DisclosureType>();

            return calcDisclosure != DisclosureType.None;
        }

        protected string FormatTime()
        {
            var html = new StringBuilder();

            var attempt = (QAttempt)Page.GetDataItem();
            if (attempt.AttemptStarted.HasValue)
                html.Append("<div>Started " + attempt.AttemptStarted.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptGraded.HasValue)
                html.Append("<div>Completed " + attempt.AttemptGraded.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptImported.HasValue)
                html.Append("<div class='form-text'>Imported " + attempt.AttemptImported.Value.Format(User.TimeZone, true) + "</div>");

            if (attempt.AttemptDuration.HasValue)
                html.Append("<div class='form-text'>Time Taken = " + ((double)attempt.AttemptDuration).Seconds().Humanize(2, minUnit: TimeUnit.Second) + "</div>");

            return html.ToString();
        }

        protected string GetEncodedGuid()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var form = attempt.Form;

            if (form == null || attempt == null)
                return string.Empty;

            return AttemptUrlForm.Create(form.FormIdentifier, attempt.AttemptIdentifier)
                .GetAttemptHash();
        }
    }
}