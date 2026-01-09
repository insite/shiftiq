using System;
using System.Web.UI;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Controls.User
{
    public partial class RelatedPersons : UserControl
    {
        #region Fields

        private int _counter;

        #endregion

        #region Properties

        protected string RowCssClass => ++_counter % 2 == 0 ? "alt1" : "alt2";

        #endregion

        #region Load data

        public bool HasContacts() => MainPanel.Visible;

        public void LoadData(Guid personID)
        {
            var leaders = UserConnectionSearch.SelectCmdsDetails(personID, CurrentSessionState.Identity.Organization.Identifier, true, null, null, null);
            Leaders.DataSource = leaders;
            Leaders.DataBind();

            LeaderRow.Visible = leaders.Count > 0;

            var managers = UserConnectionSearch.SelectCmdsDetails(personID, CurrentSessionState.Identity.Organization.Identifier, null, true, null, null);
            Managers.DataSource = managers;
            Managers.DataBind();

            ManagerRow.Visible = managers.Count > 0;

            var supervisors = UserConnectionSearch.SelectCmdsDetails(personID, CurrentSessionState.Identity.Organization.Identifier, null, null, true, null);
            Supervisors.DataSource = supervisors;
            Supervisors.DataBind();

            SupervisorRow.Visible = supervisors.Count > 0;

            var validators = UserConnectionSearch.SelectCmdsDetails(personID, CurrentSessionState.Identity.Organization.Identifier, null, null, null, true);
            Validators.DataSource = validators;
            Validators.DataBind();

            ValidatorRow.Visible = validators.Count > 0;

            var filter = new PersonFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                ParentUserIdentifier = personID,
                ExcludeUserIdentifier = personID,

                //Employees
                RelationWithParent = new[] { RelationCategory.Manager, RelationCategory.Supervisor }
            };

            var employees = ContactRepository3.SelectPersons(filter, CurrentSessionState.Identity.Organization.Identifier);
            Employees.DataSource = employees;
            Employees.DataBind();

            LearnerCount.Text = Shift.Common.Humanizer.ToQuantity(employees.Rows.Count, "learner");

            var workersPermission = PermissionNames.Custom_CMDS_Workers;

            GroupCompetencySummary.Visible = employees.Rows.Count > 0 && (
                   CurrentSessionState.Identity.IsGranted(workersPermission, PermissionOperation.Delete)
                || CurrentSessionState.Identity.IsGranted(workersPermission, PermissionOperation.Configure)
                || CurrentSessionState.Identity.IsGranted(workersPermission, PermissionOperation.Delete)
                || CurrentSessionState.Identity.IsGranted(workersPermission, PermissionOperation.Configure));

            GroupCompetencySummary.LoadData(personID, CurrentIdentityFactory.ActiveOrganizationIdentifier,
                CompetencySummaryType.ManagerGroup);

            //Students
            filter.RelationWithParent = new[] { RelationCategory.Validator };

            var students = ContactRepository3.SelectPersons(filter, CurrentSessionState.Identity.Organization.Identifier);
            StudentsPanel.Visible = students.Rows.Count > 0;
            Students.DataSource = students;
            Students.DataBind();

            EmployeeRow.Visible = employees.Rows.Count > 0 || students.Rows.Count > 0;

            MainPanel.Visible = leaders.Count > 0 || managers.Count > 0 || supervisors.Count > 0 || validators.Count > 0
                                                   || employees.Rows.Count > 0 || students.Rows.Count > 0;

            NoContacts.Visible = !MainPanel.Visible;
        }

        #endregion
    }
}