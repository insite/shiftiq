using System;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class ReturnMaterial : AdminBasePage, IHasParentLinkParameters
    {
        private QEvent _event;

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/exams/outline?event={EventIdentifier.Value}";

        private string SearchUrl
            => $"/ui/admin/events/exams/search";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                NavigateToSearch();

            if (EventIdentifier.HasValue)
                _event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation);

            if (_event == null || _event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            if (!IsPostBack)
                BindEvent();
        }

        private void BindEvent()
        {
            BackButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(this);

            ExamInfoSummary.LoadData(_event, _event.VenueLocation, showAchievement: false);

            ExamMaterialReturnShipmentCode.Text = _event.ExamMaterialReturnShipmentCode;
            ExamMaterialReturnShipmentReceived.Value = _event.ExamMaterialReturnShipmentReceived;
            ExamMaterialReturnShipmentCondition.Checked = _event.ExamMaterialReturnShipmentCondition == "Full";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new ReturnExamMaterial(_event.EventIdentifier, ExamMaterialReturnShipmentCode.Text, ExamMaterialReturnShipmentReceived.Value, ExamMaterialReturnShipmentCondition.Checked ? "Full" : "Partial"));
            HttpResponseHelper.Redirect(BackButton.NavigateUrl);
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);
    }
}
