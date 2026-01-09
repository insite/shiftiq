using System;

using InSite.Admin.Events.Exams.Controls;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using PermissionOperation = Shift.Constant.PermissionOperation;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class ChangeVenue : AdminBasePage, IHasParentLinkParameters
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

            LoadEvent();

            if (_event == null || _event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                NavigateToSearch();

            if (!IsPostBack)
                BindEvent();
        }

        private void BindEvent()
        {
            if (!IsPostBack)
            {
                VenueOfficeIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                VenueOfficeIdentifier.Filter.GroupType = GroupTypes.Venue;
                VenueOfficeIdentifier.Value = _event.VenueOfficeIdentifier;

                VenueLocationIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                VenueLocationIdentifier.Filter.GroupType = GroupTypes.Venue;
                VenueLocationIdentifier.Filter.AncestorName = _event.ExamType == EventExamType.Arc.Value ? "ARC" : null;
                VenueLocationIdentifier.Value = _event.VenueLocationIdentifier;
            }

            BackButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(this);

            ExamInfoSummary.LoadData(_event, _event.VenueLocation, showAchievement: false, showVenue: false);

            CurrentVenueOffice.Text = _event.VenueOfficeName;
            CurrentVenueLocation.Text = _event.VenueLocationName;
            VenueOfficeEqualsLocation.Visible = _event.VenueOfficeIdentifier == _event.VenueLocationIdentifier;
            VenueOfficeNotEqualsLocation.Visible = _event.VenueOfficeIdentifier != _event.VenueLocationIdentifier;

            CurrentVenueRoomField.Visible = _event.ExamType == EventExamType.Class.Value;
            CurrentVenueRoom.Text = _event.VenueRoom;
            NewVenueRoom.Text = _event.VenueRoom;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        private void Save()
        {
            if (Page.IsValid)
            {
                if (_event.VenueRoom != NewVenueRoom.Text
                 || _event.VenueLocationIdentifier != VenueLocationIdentifier.Value
                 || _event.VenueOfficeIdentifier != VenueOfficeIdentifier.Value
                 )
                {
                    var change = new ChangeEventVenue(_event.EventIdentifier, VenueOfficeIdentifier.Value, VenueLocationIdentifier.Value, NewVenueRoom.Text);
                    ServiceLocator.SendCommand(change);
                }
                HttpResponseHelper.Redirect(BackButton.NavigateUrl);
            }
        }

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private void LoadEvent()
            => _event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueOffice, x => x.VenueLocation)
                : null;
    }
}
