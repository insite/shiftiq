using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class View : AdminBasePage, ICmdsUserControl
    {
        private Guid? ItemID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private const string SearchUrl = "/ui/cmds/design/standards/profiles/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProfileHierarchy.AllowToAddProfilesFromCompany = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            var info = ItemID.HasValue ? StandardSearch.Select(ItemID.Value, x => x.Organization) : null;
            if (info == null || !string.Equals(info.StandardType, "Profile", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, "Profile #" + info.Code);

            SetInputValues(info);

            LoadCompetencies();
            LoadPersons();

            ProfileHierarchy.SwitchToViewMode();

            CloseButton.NavigateUrl = SearchUrl;
            PrintButton.NavigateUrl = $"/ui/cmds/admin/reports/competencies-per-profile?id={ItemID}&parent=outline";
        }

        private void SetInputValues(Standard info)
        {
            Number.Text = info.Code;
            TitleInput.Text = info.ContentTitle;
            Description.Text = info.ContentDescription;

            ProfileHierarchy.SetInputValues(info);
        }

        private void LoadCompetencies()
        {
            var data = CompetencyRepository.SelectProfileCompetencies(ItemID.Value);

            Competencies.DataSource = data;
            Competencies.DataBind();

            CompetencyTab.SetTitle("Competencies", data.Rows.Count);
            CompetencyTab.Visible = data.Rows.Count > 0;
        }

        private void LoadPersons()
        {
            var personFilter = new PersonFilter();
            personFilter.ProfileStandardIdentifier = ItemID.Value;

            PersonGrid.SetVisibleColumns(new string[] { "NameWithoutLink", "Organization", "EmailWork" });
            PersonGrid.LoadData(personFilter);

            NoPersonPanel.Visible = !PersonGrid.HasRows;
            PersonPanel.Visible = PersonGrid.HasRows;

            PeopleTab.SetTitle("People", PersonGrid.RowCount);
        }
    }
}