using System;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Appointments.Forms
{
    public partial class Describe : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? EventID => Guid.TryParse(Request["event"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                RedirectToSearch();

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void Open()
        {
            var @event = EventID.HasValue ? ServiceLocator.EventSearch.GetEvent(EventID.Value) : null;
            if (@event == null)
                RedirectToSearch();

            SetInputValues(@event);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var title = ContentEditor.GetValue(ContentSectionDefault.Title);
            var description = ContentEditor.GetValue(ContentSectionDefault.Description);

            ServiceLocator.SendCommand(new DescribeAppointment(EventID.Value, title, description));

            return true;
        }

        private void SetInputValues(QEvent @event)
        {
            var content = ContentEventClass.Deserialize(@event.Content);
            var uploadFolderPath = $"/events/{@event.EventNumber}";

            if (string.IsNullOrEmpty(content.Title.Default))
                content.Title.Default = @event.EventTitle;

            PageHelper.AutoBindHeader(this, null, content.Title.Default);

            if (ContentEditor.IsEmpty)
            {
                ContentEditor.Add(ContentSectionDefault.Title, content);

                {
                    var descriptionPill = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Description, content);
                    descriptionPill.AllowUpload = true;
                    descriptionPill.UploadFolderPath = uploadFolderPath;
                    ContentEditor.Add(descriptionPill);
                }

                ContentEditor.OpenTab(Tab);
            }
        }

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/events/appointments/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/events/appointments/outline?event={EventID}&panel=content&tab={ContentEditor.GetCurrentTab()}", true);

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"event={EventID}"
                : null;
        }
    }
}
