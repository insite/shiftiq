using System;
using System.Linq;
using System.Web;

using InSite.Application.Records.Read;
using InSite.Common.Web;
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

        private bool AddEnrollment(AddControl.ContactForSave contact)
        {
            if (Program == null)
                return false;

            if (contact.IsGroup)
                ProgramStore.InsertGroupEnrollment(Organization.Identifier, Program.ProgramIdentifier, contact.Identifier, User.Identifier);
            else
                ProgramHelper.EnrollLearner(Organization.Identifier, Program.ProgramIdentifier, contact.Identifier);

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