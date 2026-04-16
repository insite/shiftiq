using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Learning.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning.Programs
{
    public partial class Plan : BasePlanPage
    {
        private class DataItem
        {
            public string ProgramName { get; set; }
            public PlanAchievementTypeRepeater.DataSource DataSource { get; set; }
        }

        protected override string PageUrl => "/ui/portal/learning/programs/plan";

        private int _totalItemsCount;
        private int _validItemsCount;

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SignOff.CanBeSigned = Access.Write;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DisplayToggle.AutoPostBack = true;
            DisplayToggle.CheckedChanged += (s, a) => HttpResponseHelper.Redirect(GetUrl(null));

            SignOff.SignedOff += (s, a) => HttpResponseHelper.Redirect(GetUrl(null));

            ProgramRepeater.ItemDataBound += ProgramRepeater_ItemDataBound;
        }

        protected override Toggle GetDisplayToggle() => DisplayToggle;

        protected override Repeater GetProfilesInTrainingRepeater() => ProfilesInTraining;

        protected override Control GetProfilesInTrainingSection() => ProfilesInTrainingSection;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var person = UserSearch.Select(EmployeeID);

            PageHelper.AutoBindHeader(this, null, $"Program Training Plan for {person.FullName}");

            LoadData();
        }

        private void LoadData()
        {
            SignOff.LoadData(EmployeeID, CredentialIdentifier, true);

            _totalItemsCount = 0;
            _validItemsCount = 0;

            var programs = ProgramSearch.GetPrograms(new TProgramFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                EnrollmentUserIdentifier = EmployeeID,
            });

            ProgramRepeater.DataSource = programs
                .Where(x => x.AchievementIdentifier.HasValue)
                .Select(CreateDataItem)
                .Where(x => x.DataSource.Items.IsNotEmpty());
            ProgramRepeater.DataBind();

            var hasCredentials = _totalItemsCount > 0;

            if (!hasCredentials)
                PlanAlert.AddMessage(AlertType.Information, "Your training plan has not yet been set up by your administrator.");

            else if (_totalItemsCount == _validItemsCount)
                PlanAlert.AddMessage(AlertType.Success, "All the items in your training plan are complete!");

            DisplayTogglePanel.Visible = hasCredentials;
            UpdatePanel.Visible = hasCredentials;

            LoadProfilesInTraining();
        }

        private DataItem CreateDataItem(TProgram program)
        {
            var taskFilter = new TTaskFilter
            {
                ProgramIdentifier = program.ProgramIdentifier,
                IncludeObjectTypes = new[] { "Achievement" }
            };
            taskFilter.OrganizationIdentifiers.Add(Organization.OrganizationIdentifier);

            var taskObjects = ProgramSearch1.GetProgramTasks(taskFilter).Select(y => y.ObjectIdentifier);
            if (program.AchievementIdentifier.HasValue)
                taskObjects = taskObjects.Append(program.AchievementIdentifier.Value);

            var achievements = taskObjects.Distinct().ToArray();

            var credentials = achievements.IsNotEmpty()
                ? VCmdsCredentialSearch.SelectForTrainingPlan(EmployeeID, Organization.Identifier, achievements)
                : new List<VCmdsCredentialAndExperience>();

            return new DataItem
            {
                ProgramName = program.ProgramName,
                DataSource = PlanAchievementTypeRepeater.GetDataSource(credentials, !DisplayToggle.Checked)
            };
        }

        private void ProgramRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            var achievementTypes = (PlanAchievementTypeRepeater)e.Item.FindControl("AchievementTypes");
            achievementTypes.LoadData(dataItem.DataSource.Items, GetUrl);

            _totalItemsCount += dataItem.DataSource.TotalItemsCount;
            _validItemsCount += dataItem.DataSource.ValidItemsCount;
        }
    }
}