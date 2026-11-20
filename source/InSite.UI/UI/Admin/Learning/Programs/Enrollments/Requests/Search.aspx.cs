using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Programs
{
    // The Program Enrollment Request Search page. Lists the requests the signed-in approver may act
    // on - a Company Administrator sees company-sponsored requests for learners in their
    // organization; the partition sees every request, because self-funded requests and the
    // no-Company-Administrator fallback both route to it. Approve and Reject invoke the request
    // engine; the learner name opens the detail page. Access is gated automatically by the "Program
    // Enrollment Request Search" permission tied to this page's route.
    //
    // The Status criterion defaults to Submitted, so landing on the page still gives the approver
    // their work queue and nothing else. Clearing it shows every status, which is the only place a
    // decided request can be found by navigating - once approved or rejected, a request is otherwise
    // reachable only by its identifier, through the deep link in the funder notification.
    public partial class EnrollmentRequests : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/enrollments/requests/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
            Status.ValueChanged += (s, a) => LoadData();

            BindStatuses();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            LoadData();
        }

        // The options come from the status domain rather than from markup, because those four
        // strings also back a CHECK constraint on the table and the two have to move together. The
        // blank option is the repo's "no filter" idiom, and it is what shows every status here.
        //
        // This runs from OnInit, on every request rather than only the first. ComboBox.Items is a
        // plain field, not view state, and ComboBox.Value reads the selected option rather than the
        // posted string - so an unpopulated list makes Value return empty, and the criterion would
        // silently reset to every status on each post. OnInit is early enough that LoadPostData
        // finds the options and selects the one that was posted, which is what overrides the default
        // set below.
        private void BindStatuses()
        {
            Status.Items.Add(new ComboBoxOption());

            foreach (var status in AllStatuses)
                Status.Items.Add(new ComboBoxOption(GetStatusText(status), status));

            Status.Value = ProgramEnrollmentRequestStatus.Requested;
        }

        private static readonly string[] AllStatuses =
        {
            ProgramEnrollmentRequestStatus.Requested,
            ProgramEnrollmentRequestStatus.Approved,
            ProgramEnrollmentRequestStatus.Rejected,
            ProgramEnrollmentRequestStatus.Superseded
        };

        // "Submitted" is the word for the pending status, because the row's timestamp is its
        // submission; the stored value stays "requested". One mapping feeds both the criterion and
        // the row badge, so the dropdown and the grid never disagree about what a status is called.
        private static string GetStatusText(string status)
        {
            switch (status)
            {
                case ProgramEnrollmentRequestStatus.Requested:
                    return "Submitted";
                case ProgramEnrollmentRequestStatus.Approved:
                    return "Approved";
                case ProgramEnrollmentRequestStatus.Rejected:
                    return "Rejected";
                case ProgramEnrollmentRequestStatus.Superseded:
                    return "Superseded";
                default:
                    return status;
            }
        }

        private void LoadData()
        {
            var isPartition = Organization.Identifier == ServiceLocator.Partition.Identifier;
            // Blank means every status. Select treats an empty status the same as none, so the
            // blank option needs no translation here.
            var status = Status.Value;

            var items = isPartition
                ? TProgramEnrollmentRequestStore.Select(null, null, status)
                : TProgramEnrollmentRequestStore.Select(Organization.Identifier, ProgramEnrollmentRequestFundingPath.CompanySponsored, status);

            BindApprovalTerms(items);

            Grid.DataSource = items;
            Grid.DataBind();

            if (items.Count == 0)
                ScreenStatus.AddMessage(AlertType.Information, GetEmptyMessage(status));
        }

        // The same terms the detail page shows, for the approvals made straight from the grid. Each
        // banner appears only while at least one listed request on its funding path is still pending,
        // since the terms are about approving and a decided row has nothing left to approve. The
        // organization named is the one on the request, because that is who is billed. A Company
        // Administrator only ever sees their own organization's requests, but the partition can see
        // several at once, and then no single name is right.
        private void BindApprovalTerms(List<ProgramEnrollmentRequestListItem> items)
        {
            _organizationNames.Clear();

            var pending = items
                .Where(x => x.RequestStatus == ProgramEnrollmentRequestStatus.Requested)
                .ToList();

            var sponsoredOrganizations = pending
                .Where(x => x.RequestFundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored)
                .Select(x => x.OrganizationIdentifier)
                .Distinct()
                .ToList();

            var hasSelfFunded = pending
                .Any(x => x.RequestFundingPath == ProgramEnrollmentRequestFundingPath.SelfFunded);

            CompanySponsoredTerms.Visible = sponsoredOrganizations.Count > 0;
            SelfFundedTerms.Visible = hasSelfFunded;

            var organizationName = sponsoredOrganizations.Count == 1
                ? GetOrganizationName(sponsoredOrganizations[0])
                : "the learner's organization";

            CompanySponsoredTermsOrganization.Text = HttpUtility.HtmlEncode(organizationName);
        }

        // Organization names for the rows on screen, looked up once each rather than once per row.
        private readonly Dictionary<Guid, string> _organizationNames = new Dictionary<Guid, string>();

        private string GetOrganizationName(Guid organizationId)
        {
            if (_organizationNames.TryGetValue(organizationId, out var name))
                return name;

            var organization = OrganizationSearch.Select(organizationId);
            name = (organization?.CompanyName).IfNullOrEmpty("the organization");

            _organizationNames[organizationId] = name;

            return name;
        }

        // The Approve prompt, worded to match the detail page: a company-sponsored approval
        // authorizes billing to the request's organization, and a self-funded one confirms the
        // payment is in.
        protected string ApproveConfirmText(object fundingPath, object organizationId)
        {
            var path = fundingPath as string;

            if (path == ProgramEnrollmentRequestFundingPath.CompanySponsored && organizationId is Guid id)
            {
                var organizationName = GetOrganizationName(id);

                return $"I approve this enrollment and authorize billing to {organizationName}.";
            }

            if (path == ProgramEnrollmentRequestFundingPath.SelfFunded)
                return "I confirm payment has been received and this user is ready for enrollment.";

            return "Approve this enrollment request?";
        }

        // An empty queue is worth stating as a fact about the status that was asked for, rather than
        // as a search that came back with nothing - "there are no rejected requests" answers the
        // question, where "no matches" leaves the reader wondering what they filtered. The status
        // word comes from the same mapping the dropdown and the badge use, lowercased because here
        // it sits mid-sentence. Blank status drops the word instead of saying "no all requests".
        private static string GetEmptyMessage(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "There are no enrollment requests.";

            var text = GetStatusText(status).ToLowerInvariant();

            return $"There are no {text} enrollment requests.";
        }

        // Approve and Reject belong to a request that is still waiting on a decision. A decided or
        // superseded row is history, and the engine ignores a second decision anyway, so the buttons
        // come off the row rather than failing quietly when it is clicked.
        protected bool IsPending(object status)
        {
            var value = status as string;

            return value == ProgramEnrollmentRequestStatus.Requested;
        }

        // The status, coloured by what it means, in the same palette the detail page uses for the
        // same four values.
        protected string StatusText(object status)
        {
            var value = status as string;
            if (string.IsNullOrEmpty(value))
                return null;

            var style = GetStatusStyle(value);
            var text = GetStatusText(value);

            return $"<span class=\"{style}\">{HttpUtility.HtmlEncode(text)}</span>";
        }

        private static string GetStatusStyle(string status)
        {
            switch (status)
            {
                case ProgramEnrollmentRequestStatus.Approved:
                    return "text-success";
                case ProgramEnrollmentRequestStatus.Rejected:
                    return "text-danger";
                case ProgramEnrollmentRequestStatus.Superseded:
                    return "text-secondary";
                default:
                    return "text-info";
            }
        }

        // The learner cell: name on top, email in small muted text beneath. Plain text, deliberately
        // - a linked learner name reads as a link to the learner, which is not where it went. Row
        // navigation is the Outline icon in the action column, the same affordance every other admin
        // grid uses. The name falls back to the email when there is no live user row (an archived
        // learner), and the second line drops rather than repeating it.
        protected string LearnerCell(object learnerName, object learnerEmail)
        {
            var name = learnerName as string;
            var email = learnerEmail as string;

            if (string.IsNullOrEmpty(name))
                return HttpUtility.HtmlEncode(email);

            if (string.IsNullOrEmpty(email))
                return HttpUtility.HtmlEncode(name);

            return HttpUtility.HtmlEncode(name)
                + $"<div class=\"fs-sm text-muted\">{HttpUtility.HtmlEncode(email)}</div>";
        }

        // The funding path as a badge under the fee. Deliberately one neutral colour for both paths:
        // the Status column carries the colour that means something on this page, and a second hue in
        // the same row would read as a second status. The two words carry the distinction.
        protected string FundingPathBadge(object fundingPath)
        {
            var path = fundingPath as string;
            if (string.IsNullOrEmpty(path))
                return null;

            return $"<div class=\"mt-1\"><span class=\"badge bg-secondary\">{HttpUtility.HtmlEncode(path)}</span></div>";
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Approve" && e.CommandName != "Reject")
                return;

            var requestId = Grid.GetDataKey<Guid>(e);
            var deciderId = Identity.User.Identifier;

            if (e.CommandName == "Approve")
            {
                ProgramEnrollmentRequestHelper.Approve(requestId, deciderId);
                ScreenStatus.AddMessage(AlertType.Success, "The enrollment request was approved.");
            }
            else
            {
                ProgramEnrollmentRequestHelper.Reject(requestId, deciderId);
                ScreenStatus.AddMessage(AlertType.Success, "The enrollment request was rejected.");
            }

            LoadData();
        }
    }
}
