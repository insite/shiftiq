using System;
using System.Web;

using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Domain.Events;
using InSite.UI.Admin.Messages.Subscribers.Controls;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Subscribers.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private Guid? MessageIdentifier => Guid.TryParse(Request.QueryString["message"], out Guid value) ? value : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/messages/outline?message={MessageIdentifier}";
        private string DefaultParameters => $"message={MessageIdentifier}&tab=subscribers";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ScreenControl.SaveContact = AddSubscriber;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanCreate)
                    HttpResponseHelper.Redirect(SearchUrl);

                Open();
            }
        }

        private void Open()
        {
            var message = MessageIdentifier.HasValue
                ? ServiceLocator.MessageSearch.GetMessage(MessageIdentifier.Value)
                : null;

            if (message == null || message.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page, null, message.MessageTitle);

            var returnUrl = HttpUtility.UrlEncode($"/ui/admin/messages/subscribers/add?message={MessageIdentifier}");

            ScreenControl.LoadData(new AddControl.InitData
            {
                IsUserCreated = Request.QueryString["userCreated"] == "1",
                CreateUserUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&message={MessageIdentifier}&action=subscribers_add",
                UploadUserUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&message={MessageIdentifier}&action=subscribers_add",
                ParentUrl = GetParentUrl(DefaultParameters),
                AllowNewContact = Identity.IsGranted(ActionName.Admin_Messages_Subscribers_Add_NewContact),
                AllowUploadContact = Identity.IsGranted(ActionName.Admin_Messages_Subscribers_Add_UploadContact),
                SaveProgressHeaderText = "Adding contacts to message...",
                DefaultContactType = null
            });
        }

        private bool AddSubscriber(AddControl.ContactForSave contact)
        {
            const string role = "Message Recipient";

            var validate = !Identity.IsOperator;

            try
            {
                ServiceLocator.SendCommand(new AddSubscriber(MessageIdentifier.Value, contact.Identifier, role, validate, contact.IsGroup));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is ExamCandidateNotAllowedException inner)
                    return false;
                else
                    throw ex;
            }

            return true;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => GetParentLinkParameters(parent, null).IfNullOrEmpty($"message={MessageIdentifier}");

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();
    }
}