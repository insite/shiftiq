using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class OccupationSection : UserControl
    {
        protected Guid DepartmentIdentifier
        {
            get => (Guid)ViewState[nameof(DepartmentIdentifier)];
            set => ViewState[nameof(DepartmentIdentifier)] = value;
        }

        public void LoadProfiles(Guid department)
        {
            DepartmentIdentifier = department;

            var data = TDepartmentStandardSearch.SelectDepartmentProfilesOnly(department);

            AddedProfiles.DataSource = data;
            AddedProfiles.DataBind();

            ProfileCount.Text = data.Rows.Count > 0
                ? $"<h3>Profiles ({data.Rows.Count:n0})</h3>"
                : "<i>None</i>";
        }
    }
}