using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.BulkTool.Assign
{
    public partial class AssignDepartment : AdminBasePage, ICmdsUserControl
    {
        #region AchievementItem

        private class AchievementItem
        {
            public Guid ID { get; set; }
            public String Title { get; set; }
        }

        #endregion

        #region AchievementTypeItem class

        private class AchievementTypeItem
        {
            private readonly List<AchievementItem> _achievements = new List<AchievementItem>();

            public String Name { get; set; }

            public List<AchievementItem> Achievements
            {
                get { return _achievements; }
            }
        }

        #endregion

        #region Properties

        private VCmdsAchievementFilter AchievementFilter
        {
            get
            {
                var filter = (VCmdsAchievementFilter)ViewState[nameof(AchievementFilter)];

                if (filter == null)
                {
                    filter = new VCmdsAchievementFilter();
                    filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

                    ViewState[nameof(AchievementFilter)] = filter;
                }

                return filter;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementsValidator.ServerValidate += AchievementsValidator_ServerValidate;
            DepartmentsValidator.ServerValidate += DepartmentsValidator_ServerValidate;

            AchievementTypes.ItemDataBound += AchievementTypes_ItemDataBound;

            FilterButton.Click += FilterButton_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                LoadData();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", AchievementTypesPanel.ClientID);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", AchievementTypesPanel.ClientID);
        }

        #endregion

        #region Event handlers

        private void AchievementsValidator_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedAchievements().Length > 0;
        }

        private void DepartmentsValidator_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedDepartments().Length > 0;
        }

        private void AchievementTypes_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            AchievementTypeItem achievementType = (AchievementTypeItem)e.Item.DataItem;

            Repeater achievementsRepeater = (Repeater)e.Item.FindControl("Achievements");
            achievementsRepeater.DataSource = achievementType.Achievements;
            achievementsRepeater.DataBind();
        }

        private void FilterButton_Click(Object sender, EventArgs e)
        {
            AchievementFilter.AchievementType = SubType.Value;
            AchievementFilter.Title = AchievementTitle.Text;

            LoadAchievements();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var n = SaveData();

            var entities = Shift.Common.Humanizer.ToQuantity(n, "new assignment");

            var message = n == 0
                ? "All the selected achievements are already added to the selected departments"
                : "All the selected achievements are now added to the selected departments " +
                  $"({entities} created)";

            EditorStatus.AddMessage(AlertType.Success, message);
        }

        #endregion

        #region Save data methods

        private int SaveData()
        {
            var achievements = GetSelectedAchievements();

            var departments = GetSelectedDepartments();

            var entities = TAchievementDepartmentSearch.Bind(
                x => new { x.AchievementIdentifier, x.DepartmentIdentifier },
                x => departments.Any(d => d == x.DepartmentIdentifier));

            var inserts = new List<TAchievementDepartment>();

            foreach (var achievement in achievements)
            {
                foreach (var department in departments)
                {
                    var exists = entities.Any(x => x.AchievementIdentifier == achievement && x.DepartmentIdentifier == department);
                    if (!exists)
                        inserts.Add(new TAchievementDepartment { AchievementIdentifier = achievement, DepartmentIdentifier = department });
                }
            }

            TAchievementDepartmentStore.Insert(inserts);

            return inserts.Count;
        }

        private Guid[] GetSelectedAchievements()
        {
            List<Guid> selectedAchievements = new List<Guid>();

            foreach (RepeaterItem achievementTypeItem in AchievementTypes.Items)
            {
                Repeater achievementsRepeater = (Repeater)achievementTypeItem.FindControl("Achievements");

                foreach (RepeaterItem achievementItem in achievementsRepeater.Items)
                {
                    var selected = (Common.Web.UI.CheckBox)achievementItem.FindControl("Selected");
                    if (!selected.Checked)
                        continue;

                    if (Guid.TryParse(selected.Value, out Guid id))
                        selectedAchievements.Add(id);
                }
            }

            return selectedAchievements.ToArray();
        }

        private Guid[] GetSelectedDepartments()
        {
            List<Guid> selectedDepartments = new List<Guid>();

            foreach (RepeaterItem item in Departments.Items)
            {
                var selected = (Common.Web.UI.CheckBox)item.FindControl("Selected");
                if (!selected.Checked)
                    continue;

                if (Guid.TryParse(selected.Value, out Guid id))
                    selectedDepartments.Add(id);
            }

            return selectedDepartments.ToArray();
        }

        #endregion

        #region Load data methods

        private void LoadData()
        {
            LoadDepartments();
            LoadAchievements();
        }

        private void LoadDepartments()
        {
            DepartmentFilter filter = new DepartmentFilter();
            filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            DataTable table = ContactRepository3.SelectDepartments(filter);

            Departments.DataSource = table;
            Departments.DataBind();
        }

        private void LoadAchievements()
        {
            var list = VCmdsAchievementSearch.SelectByFilter(AchievementFilter);

            List<AchievementTypeItem> achievementTypes = new List<AchievementTypeItem>();
            AchievementTypeItem lastAchievementType = null;

            foreach (var achievement in list)
            {
                var subType = achievement.AchievementLabel ?? "Unspecified";
                var achievementID = achievement.AchievementIdentifier;
                var title = achievement.AchievementTitle;

                if (lastAchievementType == null || lastAchievementType.Name != subType)
                {
                    lastAchievementType = new AchievementTypeItem();
                    lastAchievementType.Name = subType;

                    achievementTypes.Add(lastAchievementType);
                }

                AchievementItem _achievement = new AchievementItem() { ID = achievementID, Title = title };

                lastAchievementType.Achievements.Add(_achievement);
            }

            AchievementTypes.DataSource = achievementTypes;
            AchievementTypes.DataBind();
        }

        #endregion
    }
}
