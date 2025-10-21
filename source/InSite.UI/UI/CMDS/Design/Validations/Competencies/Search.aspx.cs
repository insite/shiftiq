using System;

using InSite.Cmds.Controls.Talents.EmployeeCompetencies;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant.CMDS;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Validation
{
    public partial class Search : EmployeeCompetencyFinderController
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SearchCriteria.DisableCurrentStatus();
        }

        protected override EmployeeCompetencyFilter GetFilterFromRequestParameters(EmployeeCompetencyFilter filter)
        {
            filter = base.GetFilterFromRequestParameters(filter);

            var newFilter = filter == null ? new EmployeeCompetencyFilter() : filter;

            newFilter.Statuses = new string[] { ValidationStatuses.SubmittedForValidation };

            return newFilter;
        }
    }
}