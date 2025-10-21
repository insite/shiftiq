using System;

using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Talent.Employee.Competency.Assessment
{
    public partial class Begin : AdminBasePage, ICmdsUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            RedirectToSelfAssessment();
        }

        private void RedirectToSelfAssessment()
        {
            var filter = new EmployeeCompetencyFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                UserIdentifier = User.UserIdentifier,
                Statuses = new[]
                {
                    ValidationStatuses.NotCompleted,
                    ValidationStatuses.NotApplicable,
                    ValidationStatuses.NeedsTraining
                },
                Paging = Paging.SetSkipTake(0, 1)
            };

            var workersPermission = PermissionNames.Custom_CMDS_Workers;
            var parentUserID = Identity.IsGranted(workersPermission, PermissionOperation.Delete) || Identity.IsGranted(workersPermission, PermissionOperation.Configure)
                ? (Guid?)null
                : User.UserIdentifier;

            var table = UserCompetencyRepository.SelectSearchResults(filter, null, parentUserID);

            if (table.Rows.Count == 0)
            {
                CompetencySummary.LoadData(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, CompetencySummaryType.Employee);
                return;
            }

            var settings = new SearchSettings(filter);
            settings.Save("ui/cmds/portal/validations/competencies/search");

            var competency = (Guid)table.Rows[0]["CompetencyStandardIdentifier"];
            var url = string.Format("/ui/cmds/portal/validations/competencies/edit?id={0}", competency);
            Response.Redirect(url);
        }
    }
}
