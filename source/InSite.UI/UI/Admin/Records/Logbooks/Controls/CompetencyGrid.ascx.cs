using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class CompetencyGrid : BaseUserControl
    {
        protected Guid JournalSetupIdentifier { get; set; }

        private Dictionary<Guid, QAreaRequirement> _areaRequirements;

        public void LoadData(Guid journalSetupId, Guid frameworkId)
        {
            JournalSetupIdentifier = journalSetupId;

            var standard = StandardSearch.BindFirst(x => x, x => x.StandardIdentifier == frameworkId);
            FrameworkTitle.Text = !string.IsNullOrEmpty(standard.ContentTitle)
                ? $"<a href=\"/ui/admin/standards/edit?id={standard.StandardIdentifier}\">{standard.ContentTitle} </a>"
                : "None";

            AddCompetencies.NavigateUrl = $"/ui/admin/records/logbooks/add-competencies?journalsetup={journalSetupId}";

            BindRepeater(journalSetupId, frameworkId);
        }

        private void BindRepeater(Guid journalSetupId, Guid frameworkId)
        {
            var areas = CompetencyHelper.GetAreas(journalSetupId, frameworkId, true);
            var hasAreas = areas.IsNotEmpty();

            AreaRepeater.Visible = hasAreas;

            if (!hasAreas)
                return;

            _areaRequirements = ServiceLocator.JournalSearch.GetAreaRequirements(journalSetupId)
                .Where(x => x.AreaHours.HasValue && x.AreaHours.Value > 0)
                .ToDictionary(x => x.AreaStandardIdentifier, x => x);

            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas.Select(x => new
            {
                Identifier = x.Identifier,
                Name = x.Name,
                Hours = _areaRequirements.GetOrDefault(x.Identifier)?.AreaHours,
                Competencies = x.Competencies
            });
            AreaRepeater.DataBind();
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            competencyRepeater.DataBind();
        }
    }
}