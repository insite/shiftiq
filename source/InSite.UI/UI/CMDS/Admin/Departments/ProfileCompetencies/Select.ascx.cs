using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

namespace InSite.Cmds.Actions.Contact.Company.Competency.Popup
{
    public delegate void FilterHandler(string mode, string departments);

    public partial class Filter : UserControl
    {
        public event FilterHandler FilterApplied;

        private Guid OrganizationIdentifier => Guid.TryParse(Request["id"], out var value) ? value : CurrentIdentityFactory.ActiveOrganizationIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ApplyFilterButton.Click += ApplyFilterButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(ApplyFilterButton);
        }

        public void LoadData(Guid profile)
        {
            if (LoadDepartments(profile))
            {
                SaveOption.SelectedValue = "All";
            }
            else
            {
                ContentContainer.Visible = false;

                ScreenStatus.Type = AlertType.Warning;
                ScreenStatus.Message = "The selected profile is not assigned to any of your departments.";
            }
        }

        private void ApplyFilterButton_Click(object sender, EventArgs e) =>
            FilterApplied?.Invoke(SaveOption.SelectedValue, GetDepartments());

        private bool LoadDepartments(Guid profileId)
        {
            var profile = StandardSearch.Select(profileId);
            if (profile == null)
                return false;

            var filter = new DepartmentFilter
            {
                OrganizationIdentifier = OrganizationIdentifier,
                ProfileStandardIdentifier = profileId,
                Paging = Shift.Common.Paging.SetPage(1, int.MaxValue)
            };

            var table = ContactRepository3.SelectDepartmentsWithCounts(filter);

            var selectedDepartments = IsPostBack ? null : Request["departments"];

            Departments.Items.Clear();

            foreach (DataRow row in table.Rows)
            {
                var text = row["DepartmentName"] as string;
                var value = row["DepartmentIdentifier"].ToString();
                var item = new ListItem(text, value)
                {
                    Selected = !string.IsNullOrEmpty(selectedDepartments) && selectedDepartments.IndexOf(value + ",") >= 0
                };

                Departments.Items.Add(item);
            }

            ProfileName.Text = profile.ContentTitle;

            NoDepartments.Visible = table.Rows.Count == 0;
            DepartmentPanel.Visible = table.Rows.Count > 0;
            ApplyFilterButton.Visible = table.Rows.Count > 0;

            return true;
        }

        private string GetDepartments()
        {
            var values = Departments.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();

            return values.Length == 0 || values.Length > 5 ? null : string.Join(",", values);
        }
    }
}