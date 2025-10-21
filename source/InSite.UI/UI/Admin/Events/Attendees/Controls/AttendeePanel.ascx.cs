using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Events.Attendees.Controls
{
    public partial class AttendeePanel : BaseUserControl
    {
        public event EventHandler Refreshed;
        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        private Guid EventIdentifier
        {
            get => (Guid)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        public int InvigilatorCount
        {
            get => (int)(ViewState[nameof(InvigilatorCount)] ?? 0);
            private set => ViewState[nameof(InvigilatorCount)] = value;
        }

        public int PersonCount
        {
            get => (int)(ViewState[nameof(PersonCount)] ?? 0);
            private set => ViewState[nameof(PersonCount)] = value;
        }

        private bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        private class RoleControlInfo
        {
            public HtmlGenericControl Wrapper { get; }
            public PersonAttendeePanel Grid { get; }
            public HtmlGenericControl Subtitle { get; }

            public RoleControlInfo(Control item)
            {
                Wrapper = (HtmlGenericControl)item.FindControl("Wrapper");
                Grid = (PersonAttendeePanel)item.FindControl("PersonAttendeePanel");
                Subtitle = (HtmlGenericControl)item.FindControl("Subtitle");
            }

            public void UpdateSubtitle()
            {
                Subtitle.InnerText = $"({Grid.RowCount})";
            }
        }

        private Dictionary<int, RoleControlInfo> _personGrids = new Dictionary<int, RoleControlInfo>();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PersonAttendeeRepeater.DataBinding += PersonRoleRepeater_DataBinding;
            PersonAttendeeRepeater.ItemCreated += PersonRoleRepeater_ItemCreated;
            PersonAttendeeRepeater.ItemDataBound += PersonRoleRepeater_ItemDataBound;

            FilterButton.Click += FilterButton_Click;
        }

        private void PersonRoleRepeater_DataBinding(object sender, EventArgs e)
        {
            var filter = new QEventAttendeeFilter
            {
                EventIdentifier = EventIdentifier
            };

            var roles = ServiceLocator.EventSearch
                .GetAttendees(filter)
                .Select(x => new { x.AttendeeRole })
                .Distinct()
                .OrderBy(x => x.AttendeeRole)
                .ToArray();

            PersonAttendeeRepeater.DataSource = roles;

            _personGrids.Clear();
        }

        private void PersonRoleRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var info = new RoleControlInfo(e.Item);

            info.Grid.EntityUpdated += RolePersonGrid_Refreshed;
            info.Grid.EntityDeleted += RolePersonGrid_Refreshed;

            _personGrids.Add(e.Item.ItemIndex, info);
        }

        private void PersonRoleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var role = (string)DataBinder.Eval(e.Item.DataItem, "AttendeeRole");

            var info = _personGrids[e.Item.ItemIndex];
            info.Grid.LoadData(EventIdentifier, FilterText.Text, role ?? string.Empty, CanWrite);
            info.UpdateSubtitle();
        }

        private void RolePersonGrid_Refreshed(object sender, EventArgs e)
        {
            PersonAttendeeRepeater.DataBind();
            UpdateData();
            OnRefreshed();
        }

        private void PersonGrid_Refreshed(object sender, EventArgs e)
        {
            UpdateData();
            OnRefreshed();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            foreach (var kv in _personGrids)
            {
                var info = kv.Value;
                info.Grid.ContactKeyword = FilterText.Text;
                info.Grid.RefreshGrid();
                info.UpdateSubtitle();
            }

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(AttendeePanel),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterText.ClientID}', true);",
                true);
        }

        public void LoadData(Guid id, bool canWrite)
        {
            EventIdentifier = id;
            CanWrite = canWrite;

            AddButton.NavigateUrl = $"/ui/admin/events/attendees/add?event={EventIdentifier}";
            AddButton.Visible = Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write);

            PersonAttendeeRepeater.DataBind();

            UpdateData();
        }

        //public void Refresh()
        //{
        //    UpdateData();

        //    RefreshPersonGrid();

        //    OnRefreshed();
        //}

        //private void RefreshPersonGrid()
        //{
        //    PersonAttendeeRepeater.DataBind();
        //}

        private void UpdateData()
        {
            var filter = new QEventAttendeeFilter
            {
                EventIdentifier = EventIdentifier
            };
            PersonCount = ServiceLocator.EventSearch.CountAttendees(filter);

            filter.ContactRole = "Invigilator";
            InvigilatorCount = ServiceLocator.EventSearch.CountAttendees(filter);

            PersonAttendeeRepeater.Visible = PersonCount > 0;
            FilterPanel.Visible = PersonCount > 0;
        }
    }
}