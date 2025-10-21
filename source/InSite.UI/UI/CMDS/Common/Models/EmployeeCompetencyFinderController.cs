using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Talents.EmployeeCompetencies
{
    public abstract class EmployeeCompetencyFinderController : SearchPage<EmployeeCompetencyFilter>, ICmdsUserControl
    {
        protected override void LoadSearchedResults()
        {
            if (!IsPostBack)
                Linker.Results.Filter = GetFilterFromRequestParameters(Linker.Results.Filter);

            base.LoadSearchedResults();
        }

        protected virtual EmployeeCompetencyFilter GetFilterFromRequestParameters(EmployeeCompetencyFilter filter)
        {
            var hasParameters = false;
            var newFilter = new EmployeeCompetencyFilter { UserIdentifier = User.UserIdentifier };

            Guid tempId;

            if (Guid.TryParse(Request["profile"], out tempId))
            {
                newFilter.ProfileStandardIdentifier = tempId;
                hasParameters = true;
            }

            if (Guid.TryParse(Request["userID"], out tempId))
            {
                newFilter.UserIdentifier = tempId;
                hasParameters = true;
            }

            if (Guid.TryParse(Request["department"], out tempId))
            {
                newFilter.DepartmentIdentifier = tempId;
                hasParameters = true;
            }

            if (!string.IsNullOrEmpty(Request["status"]))
            {
                newFilter.Statuses = new[] { Request["status"] };
                hasParameters = true;
            }

            if (StringHelper.Equals(Request["mode"], "group"))
            {
                newFilter.ManagerUserIdentifier = !string.IsNullOrEmpty(Request["userID"])
                    ? Guid.Parse(Request["userID"])
                    : User.UserIdentifier;

                hasParameters = true;
            }

            if (!string.IsNullOrEmpty(Request["organization"]))
            {
                newFilter.OrganizationIdentifier = Guid.Parse(Request["organization"]);
                hasParameters = true;
            }

            if (!string.IsNullOrEmpty(Request["criticality"]))
            {
                newFilter.Criticality = Request["criticality"];
                hasParameters = true;
            }

            if (!string.IsNullOrEmpty(Request["validated"]))
            {
                newFilter.Statuses = null;
                hasParameters = true;
            }

            if (Request["compliance"] == "1")
            {
                newFilter.ProfileStandardIdentifier = null;
                hasParameters = true;
            }

            return hasParameters ? newFilter : filter;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}