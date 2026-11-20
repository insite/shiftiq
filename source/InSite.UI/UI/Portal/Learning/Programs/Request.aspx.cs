using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning.Programs
{
    // The program pre-enrollment page - the unenrolled state of a paid program, reached from its
    // catalogue card at ui/portal/learning/programs/request?program={id}. It resolves the program by
    // identifier (any organization the learner can see, so a partition-owned program works from
    // every organization), sends an already-enrolled learner to their program-filtered training
    // plan, and otherwise previews the program (name, fee, summary, the achievements enrollment
    // will assign) and offers the two funding-path requests. While the learner has an open request,
    // the request buttons give way to a notice, and while the program's catalogue is hidden the
    // whole preview gives way to one.
    public partial class ProgramRequest : PortalBasePage
    {
        public const string NavigateUrl = "/ui/portal/learning/programs/request";

        private const string SubmittedStatus = "submitted";

        public static string GetNavigateUrl(Guid programId, string status = null)
        {
            var url = NavigateUrl + "?program=" + programId;

            if (status.IsNotEmpty())
                url += "&status=" + status;

            return url;
        }

        // The identifier arrives as ?program= on the action URL, never as a route segment. See
        // RoutingConfiguration.RegisterPrograms for why a route token would cost this page its
        // permission check.
        private Guid? ProgramId
        {
            get
            {
                var value = Request.QueryString["program"];

                return Guid.TryParse(value, out var id) ? id : (Guid?)null;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SelfFundedButton.Click += (s, a) => Submit(ProgramEnrollmentRequestFundingPath.SelfFunded);
            CompanySponsoredButton.Click += (s, a) => Submit(ProgramEnrollmentRequestFundingPath.CompanySponsored);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();
        }

        private void LoadData()
        {
            var program = GetProgramOrRedirect();
            if (program == null)
                return;

            PageHelper.AutoBindHeader(Page, null, program.ProgramName);

            ProgramName.Text = program.ProgramName;

            // A hidden catalogue is staged, not public: its cards are suppressed from the grid, so
            // a learner who arrives here by an old link or a bookmark is told the program is not
            // open rather than offered the request buttons. The enrolled redirect in
            // GetProgramOrRedirect still runs first, since an enrolled learner's access does not
            // depend on the catalogue.
            if (IsCatalogHidden(program))
            {
                StatusAlert.AddMessage(AlertType.Error, "This program is not open for enrollment right now. Its catalogue is hidden, so enrollment requests are not being accepted. Contact your administrator if you were expecting to enrol.");

                FeePanel.Visible = false;
                SummaryPanel.Visible = false;
                AchievementsPanel.Visible = false;
                OpenRequestPanel.Visible = false;
                RequestPanel.Visible = false;

                return;
            }

            var isPaid = program.ProgramFee.HasValue && program.ProgramFee.Value > 0;
            FeePanel.Visible = isPaid;
            if (isPaid)
                ProgramFee.Text = program.ProgramFee.Value.ToString("C");

            // The summary is the Content tab's Summary section, not the outline's Description,
            // which is reserved for internal notes.
            var content = ServiceLocator.ContentSearch.GetBlock(program.ProgramIdentifier);
            var summary = content?.Summary?.GetHtml(CurrentLanguage, true);
            SummaryPanel.Visible = summary.IsNotEmpty();
            Summary.Text = summary;

            // The achievement preview is the program definition - the set enrollment will assign.
            var achievements = TaskSearch.SelectByProgram(program.ProgramIdentifier)
                .OrderByDescending(x => x.IsRequired)
                .ThenBy(x => x.AchievementTitle)
                .ToList();
            AchievementsPanel.Visible = achievements.Count > 0;
            AchievementRepeater.DataSource = achievements;
            AchievementRepeater.DataBind();

            if (Request["status"] == SubmittedStatus)
                StatusAlert.AddMessage(AlertType.Success, "Your enrollment request was submitted.");

            // One open request per learner and program: while it is pending, show its date and
            // contact instead of the request buttons. A rejected or superseded request leaves the
            // buttons available.
            var open = TProgramEnrollmentRequestStore.GetOpen(program.ProgramIdentifier, User.UserIdentifier);

            OpenRequestPanel.Visible = open != null;
            RequestPanel.Visible = open == null;

            if (open != null)
            {
                OpenRequestSubmittedAt.Text = TimeZones.Format(open.RequestSubmittedAt, User.TimeZone);
                OpenRequestContact.Text = open.RequestFunderEmail.IfNullOrEmpty("your administrator");
                OpenRequestContactLink.HRef = open.RequestFunderEmail.IsNotEmpty() ? "mailto:" + open.RequestFunderEmail : null;
            }
        }

        // The confirm prompt on each button is the single yes/no terms agreement. On yes, Submit
        // writes the request row, snapshots the fee and contacts, and triggers the funder
        // notification; it is idempotent, so a repeated post cannot create a second row. Redirect
        // afterward so a refresh does not resubmit.
        private void Submit(string fundingPath)
        {
            var program = GetProgramOrRedirect();
            if (program == null)
                return;

            // A form loaded before the catalogue was hidden must not submit after it; reload the
            // page, which now shows the notice instead of the buttons.
            if (IsCatalogHidden(program))
            {
                HttpResponseHelper.Redirect(GetNavigateUrl(program.ProgramIdentifier));
                return;
            }

            ProgramEnrollmentRequestHelper.Submit(program.ProgramIdentifier, User.UserIdentifier, Organization.Identifier, fundingPath);

            HttpResponseHelper.Redirect(GetNavigateUrl(program.ProgramIdentifier, SubmittedStatus));
        }

        // A program is closed to requests while the catalogue it is published in is marked Hidden.
        // A program with no catalogue is not published anywhere, and is left alone here: reaching
        // it by direct route is an administrator's decision, not a learner's.
        private static bool IsCatalogHidden(TProgram program)
        {
            if (!program.CatalogIdentifier.HasValue)
                return false;

            var catalog = CourseSearch.GetCatalog(null, program.CatalogIdentifier.Value);

            return catalog != null && catalog.IsHidden;
        }

        // Resolves the program, or redirects: an unknown program goes to the catalogue, and a
        // learner who already holds the enrollment goes to their training plan filtered to this
        // program. The enrollment is the gate, so an enrolled learner never sees the request page.
        private TProgram GetProgramOrRedirect()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
            {
                HttpResponseHelper.Redirect("/ui/portal/learning/catalogue");
                return null;
            }

            var enrollmentId = ProgramStore.GetEnrollmentIdentifier(program.ProgramIdentifier, User.UserIdentifier);
            if (enrollmentId.HasValue)
            {
                HttpResponseHelper.Redirect($"/ui/portal/learning/plan?program={program.ProgramIdentifier}");
                return null;
            }

            return program;
        }
    }
}
