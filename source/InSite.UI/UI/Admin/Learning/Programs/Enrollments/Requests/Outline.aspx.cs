using System;
using System.Web;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Programs
{
    // The Program Enrollment Request Detail page - the target of the $EnrollmentRequested deep link
    // in the funder notification, and of the learner link on the search page. Shows the request and
    // offers Approve and Reject while it is still pending; a decided request renders read-only with
    // its decision stamps. Scoping matches the search page: the partition organization opens any
    // request; any other organization opens only its own company-sponsored requests.
    public partial class EnrollmentRequestOutline : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/enrollments/requests/outline";

        public static string GetNavigateUrl(Guid requestId, string status = null)
        {
            var url = NavigateUrl + "?request=" + requestId;

            if (status.IsNotEmpty())
                url += "&status=" + status;

            return url;
        }

        private Guid? RequestId => Guid.TryParse(Request["request"], out var id) ? id : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ApproveButton.Click += (s, a) => Decide(approve: true);
            RejectButton.Click += (s, a) => Decide(approve: false);
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
            var request = GetRequest();
            if (request == null)
            {
                EnrollmentRequests.Redirect();
                return;
            }

            var program = ProgramSearch.GetProgram(request.ProgramIdentifier);
            var learner = ServiceLocator.ContactSearch.GetUser(request.LearnerUserIdentifier);
            var organization = OrganizationSearch.Select(request.OrganizationIdentifier);
            var decider = request.RequestDecidedBy.HasValue
                ? ServiceLocator.ContactSearch.GetUser(request.RequestDecidedBy.Value)
                : null;

            PageHelper.AutoBindHeader(Page, null, program?.ProgramName);

            // The redirect carries the decision that was attempted, but the helper ignores it on a
            // request that is no longer pending - a second decision from a stale tab, or a
            // replayed URL. Confirm the attempt against the request before reporting success, so a
            // decision that changed nothing is never announced as one that did.
            var status = Request["status"];

            var isDecision = status == ProgramEnrollmentRequestStatus.Approved
                || status == ProgramEnrollmentRequestStatus.Rejected;

            if (isDecision && status == request.RequestStatus)
                StatusAlert.AddMessage(AlertType.Success,
                    status == ProgramEnrollmentRequestStatus.Approved
                        ? "The enrollment request was approved."
                        : "The enrollment request was rejected.");
            else if (isDecision)
                StatusAlert.AddMessage(AlertType.Information,
                    $"No decision was applied. This request was already {request.RequestStatus}.");

            BindStatus(request.RequestStatus);

            ProgramName.Text = (program?.ProgramName).IfNullOrEmpty("N/A");
            RequestFundingPath.Text = request.RequestFundingPath;
            RequestFee.Text = request.RequestFee.HasValue ? request.RequestFee.Value.ToString("C") : "None";
            RequestSubmittedAt.Text = TimeZones.Format(request.RequestSubmittedAt, User.TimeZone);
            RequestDecidedAt.Text = request.RequestDecidedAt.HasValue ? TimeZones.Format(request.RequestDecidedAt.Value, User.TimeZone) : "Pending";
            RequestDecidedBy.Text = request.RequestDecidedBy.HasValue
                ? (decider?.UserFullName).IfNullOrEmpty(request.RequestDecidedBy.Value.ToString())
                : "Pending";
            RequestFunderEmail.Text = request.RequestFunderEmail.IfNullOrEmpty("N/A");
            RequestIdentifier.Text = request.ProgramEnrollmentRequestIdentifier.ToString();
            ProgramEnrollmentIdentifier.Text = request.ProgramEnrollmentIdentifier.HasValue ? request.ProgramEnrollmentIdentifier.Value.ToString() : "None";

            // The learner name comes from the live user row; the email is the snapshot on the request,
            // which is what survives archival.
            LearnerName.Text = (learner?.UserFullName).IfNullOrEmpty("N/A");
            LearnerEmail.Text = request.LearnerUserEmail.IfNullOrEmpty("N/A");
            OrganizationName.Text = (organization?.CompanyName).IfNullOrEmpty("N/A");

            // Deciding bills the learner's organization, so it needs Write on the resource; a Read grant
            // opens the page but renders every request read-only.
            var isDecidable = request.RequestStatus == ProgramEnrollmentRequestStatus.Requested && CanEdit;
            ApproveButton.Visible = isDecidable;
            RejectButton.Visible = isDecidable;

            BindApprovalTerms(request, organization, isDecidable);

            CancelButton.NavigateUrl = EnrollmentRequests.NavigateUrl;
        }

        // Approving enrols the learner and bills the fee, so the approver sees the terms and must accept
        // them in the Approve prompt - the admin-side match for the terms the learner agreed to when
        // requesting. The organization named is the one on the request, since that is who is billed,
        // not the approver's own organization (the partition decides other companies' requests).
        // Shown only while the viewer can decide a pending request; otherwise there is nothing to approve.
        private void BindApprovalTerms(TProgramEnrollmentRequest request, OrganizationState organization, bool isDecidable)
        {
            var isCompanySponsored = request.RequestFundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored;
            var isSelfFunded = request.RequestFundingPath == ProgramEnrollmentRequestFundingPath.SelfFunded;

            CompanySponsoredTerms.Visible = isDecidable && isCompanySponsored;
            SelfFundedTerms.Visible = isDecidable && isSelfFunded;

            var organizationName = (organization?.CompanyName).IfNullOrEmpty("the organization");
            CompanySponsoredTermsOrganization.Text = HttpUtility.HtmlEncode(organizationName);

            if (isCompanySponsored)
                ApproveButton.ConfirmText = $"I approve this enrollment and authorize billing to {organizationName}.";
            else if (isSelfFunded)
                ApproveButton.ConfirmText = "I confirm payment has been received and this user is ready for enrollment.";
            else
                ApproveButton.ConfirmText = "Approve this enrollment request?";
        }

        private void BindStatus(string status)
        {
            RequestStatus.Text = status;

            switch (status)
            {
                case ProgramEnrollmentRequestStatus.Approved:
                    RequestStatus.CssClass = "badge bg-success";
                    break;
                case ProgramEnrollmentRequestStatus.Rejected:
                    RequestStatus.CssClass = "badge bg-danger";
                    break;
                case ProgramEnrollmentRequestStatus.Superseded:
                    RequestStatus.CssClass = "badge bg-secondary";
                    break;
                default:
                    RequestStatus.CssClass = "badge bg-info";
                    break;
            }
        }

        // Approve or reject, then redirect back to this page so the decided state renders and a refresh
        // cannot repeat the decision. The helper ignores a request that is no longer pending.
        private void Decide(bool approve)
        {
            var request = GetRequest();
            if (request == null)
            {
                EnrollmentRequests.Redirect();
                return;
            }

            var requestId = request.ProgramEnrollmentRequestIdentifier;

            // The buttons are hidden without Write, but a crafted postback can still raise the click.
            if (!CanEdit)
            {
                HttpResponseHelper.Redirect(GetNavigateUrl(requestId));
                return;
            }

            var deciderId = Identity.User.Identifier;

            if (approve)
                ProgramEnrollmentRequestHelper.Approve(requestId, deciderId);
            else
                ProgramEnrollmentRequestHelper.Reject(requestId, deciderId);

            HttpResponseHelper.Redirect(GetNavigateUrl(requestId, approve ? ProgramEnrollmentRequestStatus.Approved : ProgramEnrollmentRequestStatus.Rejected));
        }

        // The same scoping rule as the search page. The partition organization is the approver of last
        // resort - self-funded requests and the no-Company-Administrator fallback both route to it - so it
        // may open any request. Any other organization opens only its own company-sponsored requests; a
        // request outside that scope is treated as not found, so the deep link cannot expose another
        // organization's request.
        private TProgramEnrollmentRequest GetRequest()
        {
            if (!RequestId.HasValue)
                return null;

            var request = TProgramEnrollmentRequestStore.Get(RequestId.Value);
            if (request == null)
                return null;

            var isPartition = Organization.Identifier == ServiceLocator.Partition.Identifier;

            var inScope = isPartition
                || (request.OrganizationIdentifier == Organization.Identifier
                    && request.RequestFundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored);

            return inScope ? request : null;
        }
    }
}
