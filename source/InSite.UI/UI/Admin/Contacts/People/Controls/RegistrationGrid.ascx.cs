using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class RegistrationGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        public virtual bool AllowEdit
        {
            get => (bool)(ViewState[nameof(AllowEdit)] ?? true);
            set => ViewState[nameof(AllowEdit)] = value;
        }

        public Guid UserIdentifier
        {
            get => (Guid)ViewState[nameof(UserIdentifier)];
            set => ViewState[nameof(UserIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;

            Grid.EnablePaging = false;
            Grid.RowDataBound += Grid_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            Grid.Columns.FindByName("Commands").Visible = AllowEdit;

            base.OnPreRender(e);
        }

        public void LoadData(Guid userIdentifier, bool allowEdit)
        {
            AllowEdit = allowEdit;

            UserIdentifier = userIdentifier;

            Search(new NullFilter());

            if (RowCount == 0)
                MultiView.SetActiveView(NoRecordsView);
            else
                MultiView.SetActiveView(GridView);
        }

        public void Clear()
        {
            Clear(new NullFilter());
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var eventType = (string)DataBinder.Eval(e.Row.DataItem, "Event.EventType");
            var eventIdentifier = (Guid)DataBinder.Eval(e.Row.DataItem, "Event.EventIdentifier");
            var registrationIdentifier = (Guid)DataBinder.Eval(e.Row.DataItem, "RegistrationIdentifier");

            var isClass = eventType.Equals("Class", StringComparison.OrdinalIgnoreCase);
            var isExam = eventType.Equals("Exam", StringComparison.OrdinalIgnoreCase);

            var eventLink = (HyperLink)e.Row.FindControl("EventLink");
            eventLink.Visible = isClass || isExam;
            eventLink.NavigateUrl = isClass
                ? $"/ui/admin/events/classes/outline?event={eventIdentifier}&panel=registrations"
                : $"/ui/admin/events/exams/outline?event={eventIdentifier}&panel=candidates";

            var registrtaionLink = (IconLink)e.Row.FindControl("RegistrtaionLink");
            registrtaionLink.Visible = isClass || isExam;
            registrtaionLink.NavigateUrl = isClass
                ? new ReturnUrl($"contact={UserIdentifier}&panel=registrations").GetRedirectUrl($"/ui/admin/registrations/classes/edit?id={registrationIdentifier}&user=1")
                : new ReturnUrl($"contact={UserIdentifier}&panel=registrations").GetRedirectUrl($"/ui/admin/registrations/exams/edit?registration={registrationIdentifier}");

            var deleteLink = (IconLink)e.Row.FindControl("DeleteLink");
            deleteLink.Visible = isClass || isExam;
            deleteLink.NavigateUrl = isClass
                ? new ReturnUrl($"contact={UserIdentifier}&panel=registrations").GetRedirectUrl($"/ui/admin/registrations/classes/delete?id={registrationIdentifier}")
                : new ReturnUrl($"contact={UserIdentifier}&panel=registrations").GetRedirectUrl($"/ui/admin/registrations/exams/delete?id={registrationIdentifier}");
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(RegistrationGrid),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        protected override int SelectCount(NullFilter filter)
        {
            return FilterData().Count;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var registrations = FilterData()
                .Select(x => new
                {
                    RegistrationIdentifier = x.RegistrationIdentifier,
                    Event = x.Event,
                    RegistrationRequestedOn = x.RegistrationRequestedOn,
                    ApprovalStatus = x.ApprovalStatus,
                    RegistrationFee = x.RegistrationFee,
                    AttendanceStatus = x.AttendanceStatus,
                    Score = x.Score,
                    RegistrationComment = x.RegistrationComment
                })
                .OrderByDescending(x => x.RegistrationRequestedOn)
                .ThenBy(x => x.Event.EventTitle)
                .ToList()
                .ToSearchResult();

            return registrations;
        }

        private List<QRegistration> FilterData()
        {
            var filter = new QRegistrationFilter
            {
                CandidateIdentifier = UserIdentifier,
                OrganizationIdentifier = Organization.Identifier
            };

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event)
                .OrderByDescending(x => x.Event.EventScheduledStart)
                .ThenBy(x => x.RegistrationRequestedOn)
                .ToList();

            if (!string.IsNullOrEmpty(FilterTextBox.Text))
            {
                var keyword = FilterTextBox.Text;
                registrations = registrations.Where(x => x.Event.EventTitle.Contains(keyword)).ToList();
            }

            return registrations;
        }
    }
}