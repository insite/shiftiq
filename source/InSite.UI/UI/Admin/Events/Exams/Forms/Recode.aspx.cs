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
    public partial class Recode : AdminBasePage, IHasParentLinkParameters
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

            BillingCode.Settings.CollectionName = CollectionName.Activities_Exams_Billing_Type;
            BillingCode.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                NavigateToSearch();

            LoadEvent();

            if (_event == null || _event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            if (!IsPostBack)
            {
                BindEvent();
            }
        }

        private void BindEvent()
        {
            BackButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(this);

            ExamInfoSummary.LoadData(_event, _event.VenueLocation, showAchievement: false);

            ClassCode.Text = _event.EventClassCode;
            ClassCodeField.Visible = _event.ExamType == EventExamType.Class.Value;
            BillingCode.Value = _event.EventBillingType;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (EventIdentifier.HasValue)
            {
                var before = _event;
                var id = before.EventIdentifier;
                var newClassCode = ClassCode.Text;
                var newBillingCode = BillingCode.Value;

                if (before.EventClassCode != newClassCode || before.EventBillingType != newBillingCode)
                    ServiceLocator.SendCommand(new RecodeEvent(id, newClassCode, newBillingCode));
            }

            HttpResponseHelper.Redirect(BackButton.NavigateUrl);
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private void LoadEvent()
        {
            _event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
        }
    }
}
