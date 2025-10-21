using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Registrations
{
    public partial class Search : PortalBasePage
    {
        #region Properties

        private bool ShowPastEvents
        {
            get => (bool)ViewState[nameof(ShowPastEvents)];
            set => ViewState[nameof(ShowPastEvents)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowPastEventsButton.Click += (s, a) => OnShowPastEvents();
            ShowCurrentEventsButton.Click += (s, a) => OnShowCurrentEvents();

            MyRegistrationsPagination.PageChanged += (s, a) => MyRegistrationsPanel.DataBind();
            OtherRegistrationsPagination.PageChanged += (s, a) => OtherRegistrationsPanel.DataBind();

            MyRegistrationsRepeater.DataBinding += MyRegistrationsRepeater_DataBinding;
            OtherRegistrationsRepeater.DataBinding += OtherRegistrationsRepeater_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            OnShowCurrentEvents();
        }

        #endregion

        #region Event handlers

        private void MyRegistrationsRepeater_DataBinding(object sender, EventArgs e)
        {
            var filter = GetMyRegistrationsFilter();
            filter.Paging = Paging.SetSkipTake(MyRegistrationsPagination.ItemsSkip, MyRegistrationsPagination.ItemsTake);

            var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(
                filter,
                x => x.Event);

            var userNames = GetUserNames(
                registrations
                    .Where(x => x.RegistrationRequestedBy.HasValue
                             && x.RegistrationRequestedBy.Value != User.Identifier)
                    .Select(x => x.RegistrationRequestedBy.Value));

            MyRegistrationsRepeater.DataSource = registrations
                .Select(x => new
                {
                    EventStartDate = x.Event.EventScheduledStart,
                    EventTitle = x.Event.EventTitle,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    RegistrationRequestedBy = !x.RegistrationRequestedBy.HasValue
                        ? "Unknown"
                        : x.RegistrationRequestedBy.Value == User.Identifier
                            ? "Self-Registration"
                            : userNames.ContainsKey(x.RegistrationRequestedBy.Value)
                                ? userNames[x.RegistrationRequestedBy.Value]
                                : UserNames.Someone,
                    ApprovalStatus = x.ApprovalStatus,
                    AttendanceStatus = x.AttendanceStatus
                });
        }

        private void OtherRegistrationsRepeater_DataBinding(object sender, EventArgs e)
        {
            var filter = GetOtherRegistrationsFilter();
            filter.Paging = Paging.SetSkipTake(OtherRegistrationsPagination.ItemsSkip, OtherRegistrationsPagination.ItemsTake);

            var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(
                filter,
                x => x.Event);

            var userNames = GetUserNames(
                registrations.Select(x => x.CandidateIdentifier));

            OtherRegistrationsRepeater.DataSource = registrations
                .Select(x => new
                {
                    EventStartDate = x.Event.EventScheduledStart,
                    EventTitle = x.Event.EventTitle,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    CandidateName = userNames.ContainsKey(x.CandidateIdentifier)
                        ? userNames[x.CandidateIdentifier]
                        : UserNames.Someone,
                    ApprovalStatus = x.ApprovalStatus,
                    AttendanceStatus = x.AttendanceStatus
                });
        }

        private void OnShowPastEvents()
        {
            ShowPastEvents = true;

            LoadData();

            ShowPastEventsButton.Visible = false;
            ShowCurrentEventsButton.Visible = true;
        }

        private void OnShowCurrentEvents()
        {
            ShowPastEvents = false;

            LoadData();

            ShowPastEventsButton.Visible = true;
            ShowCurrentEventsButton.Visible = false;
        }

        #endregion

        #region Data binding

        private void LoadData()
        {
            var hasMyRegistrations = LoadMyRegistrations();
            var hasOtherRegistrations = LoadOtherRegistrations();
            var hasRegistration = hasMyRegistrations || hasOtherRegistrations;

            MyRegistrationsPanel.Visible = hasMyRegistrations;
            OtherRegistrationsPanel.Visible = hasOtherRegistrations;
            MainAccordion.Visible = hasRegistration;

            if (!hasMyRegistrations && !hasOtherRegistrations)
            {
                if (ShowPastEvents)
                    ScreenStatus.AddMessage(AlertType.Warning, Translate("There are no past registrations"));
                else
                    ScreenStatus.AddMessage(AlertType.Warning, Translate("There are no current registrations"));
            }
        }

        private bool LoadMyRegistrations()
        {
            var count = ServiceLocator.RegistrationSearch.CountRegistrations(GetMyRegistrationsFilter());

            MyRegistrationsPagination.ItemsCount = count;
            MyRegistrationsFooter.Visible = MyRegistrationsPagination.PageCount > 1;

            MyRegistrationsRepeater.DataBind();

            return count > 0;
        }

        private bool LoadOtherRegistrations()
        {
            var count = ServiceLocator.RegistrationSearch.CountRegistrations(GetOtherRegistrationsFilter());

            OtherRegistrationsPagination.ItemsCount = count;
            OtherRegistrationsFooter.Visible = OtherRegistrationsPagination.PageCount > 1;

            OtherRegistrationsRepeater.DataBind();

            return count > 0;
        }

        #endregion

        #region Helpers

        private QRegistrationFilter GetMyRegistrationsFilter()
        {
            var filter = GetRegistrationFilter();
            filter.CandidateIdentifier = User.Identifier;
            return filter;
        }

        private QRegistrationFilter GetOtherRegistrationsFilter()
        {
            var filter = GetRegistrationFilter();
            filter.RegistrationRequestedBy = User.Identifier;
            filter.ExcludeCandidateIdentifier = new[] { User.Identifier };
            return filter;
        }

        private QRegistrationFilter GetRegistrationFilter()
        {
            var result = new QRegistrationFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                EventType = "Class",
                OrderBy = "Event.EventScheduledStart DESC, RegistrationRequestedOn"
            };

            if (!ShowPastEvents)
                result.EventScheduledSince = DateTimeOffset.UtcNow.AddHours(-12);
            else
                result.EventScheduledBefore = DateTimeOffset.UtcNow.AddHours(-12);

            return result;
        }

        private Dictionary<Guid, string> GetUserNames(IEnumerable<Guid> ids)
        {
            return UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = ids.Distinct().ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var date = (DateTimeOffset)DataBinder.Eval(dataItem, name);
            return date.Format(User.TimeZone, true);
        }

        protected string GetLocalDate(string name)
        {
            var dataItem = Page.GetDataItem();
            var date = (DateTimeOffset)DataBinder.Eval(dataItem, name);
            return date.FormatDateOnly(User.TimeZone);
        }

        #endregion
    }
}