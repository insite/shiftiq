using System;
using System.Web;

using InSite.Application.Records.Read;
using InSite.Persistence;
using InSite.UI.Admin.Messages.Subscribers.Controls;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Programs
{
    public partial class AddUser : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ProgramIdentifier => Guid.TryParse(Request["program"], out var id) ? id : (Guid?)null;

        // A program with a fee is paid: every enrollment must leave an approved request row behind for billing.
        private bool IsPaid => Program?.ProgramFee > 0;

        private TProgram _program;
        private TProgram Program
        {
            get
            {
                if (_program == null && ProgramIdentifier.HasValue)
                    _program = ProgramSearch.GetProgram(ProgramIdentifier.Value);

                return _program;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ScreenControl.SaveContact = AddEnrollment;
            ScreenControl.PersonOrganizationScope = GetPersonOrganizationScope;

            // The people below are drawn from this organization, so choosing one has to redraw them.
            // Without the post back the grid keeps showing the previous organization's learners,
            // which reads as though the choice did nothing.
            LearnerOrganizationIdentifier.AutoPostBack = true;
            LearnerOrganizationIdentifier.ValueChanged += (s, a) =>
            {
                ScreenControl.RefreshSearch();
                BindSaveConfirmText();
            };

            // The confirmation names who pays, so it has to follow the funding path as it changes.
            FundingPath.AutoPostBack = true;
            FundingPath.SelectedIndexChanged += (s, a) => BindSaveConfirmText();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanCreate)
                    Search.Redirect();

                Open();
            }
        }

        private void Open()
        {
            if (Program == null || Program.OrganizationIdentifier != Organization.Identifier)
                Search.Redirect();

            PageHelper.AutoBindHeader(Page, qualifier: Program.ProgramName);

            BindFundingPanel();

            var returnValue = Request.QueryString["return"];
            var returnUrl = HttpUtility.UrlEncode($"/ui/admin/learning/programs/enrollments/add?program={ProgramIdentifier}");
            if (returnValue.IsNotEmpty())
                returnUrl += $"&return={HttpUtility.UrlEncode(returnValue)}";

            returnUrl = HttpUtility.UrlEncode(returnUrl);

            var parentUrl = returnValue.IfNullOrEmpty(() => Outline.GetNavigateUrl(ProgramIdentifier ?? Guid.Empty, tab: "enrollments"));

            ScreenControl.LoadData(new AddControl.InitData
            {
                IsUserCreated = Request.QueryString["userCreated"] == "1",
                CreateUserUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&achievement={ProgramIdentifier}&action=programs_addpeople",
                UploadUserUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&achievement={ProgramIdentifier}&action=programs_addpeople",
                ParentUrl = parentUrl,
                AllowNewContact = Identity.IsGranted(ActionName.Admin_Records_Programs_AddUser_NewContact),
                AllowUploadContact = Identity.IsGranted(ActionName.Admin_Records_Programs_AddUser_UploadContact),
                SaveProgressHeaderText = "Adding contacts to program...",
                DefaultContactType = "Person"
            });
        }

        // Shown only for a paid program. The funding path and, for a company-sponsored request, the
        // organization to bill are the two things the request row needs that only the admin knows:
        // this page runs in the program's (partition) organization, so the learner's own
        // organization is not implied by the context.
        private void BindFundingPanel()
        {
            FundingPanel.Visible = IsPaid;
            if (!IsPaid)
                return;

            ProgramFeeText.Text = Program.ProgramFee.Value.ToString("C");

            FundingPath.Items.Clear();
            FundingPath.Items.Add(new System.Web.UI.WebControls.ListItem("Self-funded (billed to the learner)", ProgramEnrollmentRequestFundingPath.SelfFunded));
            FundingPath.Items.Add(new System.Web.UI.WebControls.ListItem("Company-sponsored (billed to the organization below)", ProgramEnrollmentRequestFundingPath.CompanySponsored));

            BindSaveConfirmText();
        }

        // A paid enrollment is billed the moment it is saved, so the admin confirms who pays before
        // the Add button runs. No prompt when the save would be refused anyway - no funding path, or
        // a company-sponsored path with no organization - since AddPaidEnrollment reports those.
        // An unpaid program never prompts.
        private void BindSaveConfirmText()
        {
            ScreenControl.SaveConfirmText = GetSaveConfirmText();
        }

        private string GetSaveConfirmText()
        {
            if (!IsPaid)
                return null;

            var fundingPath = FundingPath.SelectedValue;

            if (fundingPath == ProgramEnrollmentRequestFundingPath.SelfFunded)
                return "I confirm payment has been received for this enrollment.";

            if (fundingPath != ProgramEnrollmentRequestFundingPath.CompanySponsored)
                return null;

            var organizationId = LearnerOrganizationIdentifier.Value;
            if (!organizationId.HasValue)
                return null;

            var organization = OrganizationSearch.Select(organizationId.Value);
            var organizationName = (organization?.CompanyName).IfNullOrEmpty("the selected organization");

            return $"I confirm these enrollments and authorize billing to {organizationName}.";
        }

        // Which organization's people the picker searches. A learner buying a partition-owned paid
        // program belongs to their own company, not to the organization that owns the program, so
        // the picker searches the organization the admin named. Without this it can only ever find
        // people in the program's own organization, and the enrollment the funding panel invites
        // cannot be made at all.
        //
        // This does not depend on the funding path. The path says who pays, not where the learner
        // lives, so both paths search the same place. Until the admin names an organization the
        // default scope stands; AddPaidEnrollment refuses a company-sponsored save until they do.
        private Guid? GetPersonOrganizationScope()
        {
            if (!IsPaid)
                return null;

            return LearnerOrganizationIdentifier.Value;
        }

        private bool AddEnrollment(AddControl.ContactForSave contact)
        {
            if (Program == null)
                return false;

            if (IsPaid)
                return AddPaidEnrollment(contact);

            if (contact.IsGroup)
                ProgramStore.InsertGroupEnrollment(Organization.Identifier, Program.ProgramIdentifier, contact.Identifier, User.Identifier);
            else
                ProgramHelper.EnrollLearner(Organization.Identifier, Program.ProgramIdentifier, contact.Identifier);

            return true;
        }

        // The admin bypass for a paid program: EnrollLearners enrols the learner and writes an
        // approved request directly, superseding any open request for the same learner and program.
        // Returning false stops the shared control's save loop, so the message added here is what
        // the admin sees.
        private bool AddPaidEnrollment(AddControl.ContactForSave contact)
        {
            if (contact.IsGroup)
            {
                ScreenStatus.AddMessage(AlertType.Error, "A paid program cannot be assigned to a group. Add learners individually so each enrollment has a billing record.");
                return false;
            }

            var fundingPath = FundingPath.SelectedValue;
            if (string.IsNullOrEmpty(fundingPath))
            {
                ScreenStatus.AddMessage(AlertType.Error, "Choose a funding path before adding learners to a paid program.");
                return false;
            }

            // Self-funded is billed to the learner, so the request is recorded against the
            // program's organization; company-sponsored is billed to the organization the admin
            // selected.
            var organizationId = fundingPath == ProgramEnrollmentRequestFundingPath.CompanySponsored
                ? LearnerOrganizationIdentifier.Value
                : Organization.Identifier;

            if (!organizationId.HasValue)
            {
                ScreenStatus.AddMessage(AlertType.Error, "Choose the organization to bill for a company-sponsored enrollment.");
                return false;
            }

            ProgramEnrollmentRequestHelper.EnrollLearners(Program.ProgramIdentifier, contact.Identifier, organizationId.Value, fundingPath, User.Identifier);

            return true;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={Request.QueryString["program"]}&tab=enrollments"
                : null;
        }
    }
}
