using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Controls.Contacts.Companies.Competencies;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;
using IconButton = InSite.Custom.CMDS.Common.Controls.Server.IconButton;

namespace InSite.Cmds.Actions.Contact.Company.Competency
{
    public partial class Manage : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        #region Constants

        private const int DepartmentPageSize = 5;

        #endregion

        #region Class: HeaderTemplate

        private class HeaderTemplate : ITemplate
        {
            #region Construction

            public HeaderTemplate(Manage control, DataControlField owner, Guid department)
            {
                _control = control;
                _owner = owner;
                _department = department;
            }

            #endregion

            #region ITemplate Members

            public void InstantiateIn(Control container)
            {
                var text = new System.Web.UI.WebControls.Literal();
                container.Controls.Add(text);
                text.Text = _owner.HeaderText + "<br/>";

                var checkButton = new IconButton();
                container.Controls.Add(checkButton);
                checkButton.ID = "SelectAllCompetencies";
                checkButton.IsFontIcon = true;
                checkButton.CssClass = "check-square";
                checkButton.ToolTip = "Select All";
                checkButton.PreRender += CheckButton_PreRender;

                var space = new System.Web.UI.WebControls.Literal();
                container.Controls.Add(space);
                space.Text = "&nbsp;";

                var uncheckButton = new IconButton();
                container.Controls.Add(uncheckButton);
                uncheckButton.ID = "UnselectAllCompetencies";
                uncheckButton.IsFontIcon = true;
                uncheckButton.CssClass = "times";
                uncheckButton.ToolTip = "Deselect All";
                uncheckButton.PreRender += UncheckButton_PreRender;
            }

            #endregion

            #region Fields

            private readonly Manage _control;
            private readonly DataControlField _owner;
            private readonly Guid _department;

            #endregion

            #region Event handlers

            private void CheckButton_PreRender(object sender, EventArgs e)
            {
                var checkButton = (IconButton)sender;
                var selectedDepartmentID = (HiddenField)ControlHelper.GetControl(_control, "SelectedDepartmentID");
                var selectAllCompetenciesButton = ControlHelper.GetControl(_control, "SelectAllCompetenciesButton");

                var script = new StringBuilder();
                script.Append("if(confirm('Are you sure you want to select all competencies for this department?')){");
                script.AppendFormat("$get('{0}').disabled = true;", checkButton.ClientID);
                script.AppendFormat("$get('{0}').value = '{1}';", selectedDepartmentID.ClientID, _department);
                script.AppendFormat("__doPostBack('{0}', '');", selectAllCompetenciesButton.UniqueID);
                script.Append("}; return false;");

                checkButton.OnClientClick = script.ToString();
            }

            private void UncheckButton_PreRender(object sender, EventArgs e)
            {
                var checkButton = (IconButton)sender;
                var selectedDepartmentID = (HiddenField)ControlHelper.GetControl(_control, "SelectedDepartmentID");
                var unselectAllCompetenciesButton = ControlHelper.GetControl(_control, "UnselectAllCompetenciesButton");

                var script = new StringBuilder();
                script.Append("if(confirm('Are you sure you want to deselect all competencies for this department?')){");
                script.AppendFormat("$get('{0}').disabled = true;", checkButton.ClientID);
                script.AppendFormat("$get('{0}').value = '{1}';", selectedDepartmentID.ClientID, _department);
                script.AppendFormat("__doPostBack('{0}', '');", unselectAllCompetenciesButton.UniqueID);
                script.Append("}; return false;");

                checkButton.OnClientClick = script.ToString();
            }

            #endregion
        }

        #endregion

        #region Class: DepartmentName

        [Serializable]
        private class DepartmentName
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Fields

        private Dictionary<Guid, int> _departmentMap;

        private DataTable _summaryTable;

        #endregion

        #region Properties

        private Guid OrganizationIdentifier => Guid.TryParse(Request.QueryString["id"], out Guid value) ? value : Guid.Empty;

        private DepartmentName[] DepartmentNames
        {
            get => (DepartmentName[])ViewState[nameof(DepartmentNames)];
            set => ViewState[nameof(DepartmentNames)] = value;
        }

        private Guid? CurrentProfileIdentifier
        {
            get => (Guid?)ViewState[nameof(CurrentProfileIdentifier)];
            set => ViewState[nameof(CurrentProfileIdentifier)] = value;
        }

        private Dictionary<Guid, int> DepartmentMap
        {
            get
            {
                if (_departmentMap == null)
                {
                    _departmentMap = new Dictionary<Guid, int>();

                    for (var i = 0; i < DepartmentNames.Length; i++)
                        _departmentMap.Add(DepartmentNames[i].Identifier, i + 3);
                }

                return _departmentMap;

            }
        }

        private Dictionary<Guid, int> CompetencyMap { get; set; }

        private IEnumerable<Guid> Departments => CsvConverter.CsvTextToGuidList(DepartmentFilter.Value);

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProfileIdentifier.AutoPostBack = true;
            ProfileIdentifier.ValueChanged += (s, a) => OnProfileChanged();

            SaveButton.Click += SaveButton_Click;
            SaveAndCloseButton.Click += SaveAndCloseButton_Click;

            DeleteCompetencyButton.Click += DeleteCompetencyButton_Click;
            CopyCompetencyButton.Click += CopyCompetencyButton_Click;
            SetLevelButton.Click += SetLevelButton_Click;

            SelectAllCompetenciesButton.Click += SelectAllCompetenciesButton_Click;
            UnselectAllCompetenciesButton.Click += UnselectAllCompetenciesButton_Click;

            Grid.DataBinding += Grid_DataBinding;
            Grid.PageIndexChanging += Grid_PageIndexChanging;

            FilterWindow.FilterApplied += FilterWindow_FilterApplied;

            RemoveCompetenciesButton.Click += RemoveCompetenciesButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                ProfileIdentifier.Filter.OrganizationIdentifier = OrganizationIdentifier;

                OnProfileChanged();

                CloseButton.NavigateUrl = GetParentUrl();
            }
            else
            {
                SetGridHeaderTemplates();
            }

            RemovedLabel.Visible = false;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"id={OrganizationIdentifier}"
                : null;
        }

        private Persistence.Plugin.CMDS.CompetencyFilter GetCompetencyFilter(Guid profileId) =>
            new Persistence.Plugin.CMDS.CompetencyFilter { Profiles = new[] { profileId } };

        private string GetParentUrl() => $"/ui/cmds/admin/organizations/edit?id={OrganizationIdentifier}";

        #endregion

        #region Event handlers

        private void OnProfileChanged()
        {
            var profileId = ProfileIdentifier.Value;
            var hasProfile = profileId.HasValue;

            ProfileDetails.Visible = hasProfile;
            FilterWindow.Visible = hasProfile;
            ResultTab.Visible = false;

            if (!hasProfile)
                return;

            FilterWindow.LoadData(profileId.Value);

            LoadProfileDetails(profileId.Value);
        }

        private void FilterWindow_FilterApplied(string mode, string departments)
        {
            SaveOptionField.Value = mode;
            DepartmentFilter.Value = departments;
            CurrentProfileIdentifier = ProfileIdentifier.Value;

            LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Save(CurrentProfileIdentifier.Value);

            Grid.DataBind();

            ScreenStatus.AddMessage(AlertType.Success, "Your changes have been saved.");
        }

        private void SaveAndCloseButton_Click(object sender, EventArgs e)
        {
            Save(CurrentProfileIdentifier.Value);

            Response.Redirect(GetParentUrl());
        }

        private void DeleteCompetencyButton_Click(object sender, EventArgs e)
        {
            var deleteCompetencyID = Guid.Parse(AffectedCompetencyStandardIdentifier.Value);
            var info = Persistence.Plugin.CMDS.CompetencyRepository.Select(deleteCompetencyID);

            ScreenStatus.AddMessage(AlertType.Success, $"Competency #{info.Number} has been deleted from this profile.");

            LoadDataAfterCompetencyDeletedOrCopied(CurrentProfileIdentifier.Value);
        }

        private void CopyCompetencyButton_Click(object sender, EventArgs e)
        {
            var profileId = CurrentProfileIdentifier.Value;
            var competencyId = Guid.Parse(AffectedCompetencyStandardIdentifier.Value);
            var sourceDepartment = Guid.Parse(OperationOptions.Value);

            var original = DepartmentProfileCompetencySearch.SelectFirst(
                x => x.DepartmentIdentifier == sourceDepartment
                  && x.ProfileStandardIdentifier == profileId
                  && x.CompetencyStandardIdentifier == competencyId);

            var insertList = new List<DepartmentProfileCompetency>();
            var updateList = new List<DepartmentProfileCompetency>();

            foreach (var department in Departments)
            {
                var copy = DepartmentProfileCompetencySearch.SelectFirst(
                    x => x.DepartmentIdentifier == department
                      && x.ProfileStandardIdentifier == profileId
                      && x.CompetencyStandardIdentifier == competencyId);

                if (copy != null)
                    updateList.Add(copy);
                else
                    insertList.Add(copy = new DepartmentProfileCompetency
                    {
                        DepartmentIdentifier = department,
                        ProfileStandardIdentifier = profileId,
                        CompetencyStandardIdentifier = competencyId
                    });

                copy.IsCritical = original.IsCritical;
                copy.LifetimeMonths = original.LifetimeMonths;
            }

            DepartmentProfileCompetencyStore.InsertUpdateDelete(insertList, updateList, null);

            var competency = Persistence.Plugin.CMDS.CompetencyRepository.Select(competencyId);

            ScreenStatus.AddMessage(AlertType.Success, $"Settings for competency #{competency.Number} have been copied to all departments.");

            LoadDataAfterCompetencyDeletedOrCopied(profileId);
        }

        private void SetLevelButton_Click(object sender, EventArgs e)
        {
            var parts = OperationOptions.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var allProfiles = bool.Parse(parts[0]);
            var departmentId = Guid.Parse(parts[1]);
            var competencyId = Guid.Parse(AffectedCompetencyStandardIdentifier.Value);
            var profileId = CurrentProfileIdentifier.Value;

            var entities = allProfiles
                ? DepartmentProfileCompetencySearch.Select(x => x.DepartmentIdentifier == departmentId)
                : DepartmentProfileCompetencySearch.Select(x => x.DepartmentIdentifier == departmentId && x.ProfileStandardIdentifier == profileId);

            var original = entities.FirstOrDefault(x => x.ProfileStandardIdentifier == profileId && x.CompetencyStandardIdentifier == competencyId);

            if (original != null)
            {
                foreach (var entity in entities)
                {
                    entity.IsCritical = original.IsCritical;
                    entity.LifetimeMonths = original.LifetimeMonths;
                }

                DepartmentProfileCompetencyStore.InsertUpdateDelete(null, entities, null);

                var department = DepartmentSearch.Select(departmentId);
                var text = string.Format("Specified settings have been applied to ALL competencies in this profile of department {0}.", department.DepartmentName);

                ScreenStatus.AddMessage(AlertType.Success, text);
            }

            LoadDataAfterCompetencyDeletedOrCopied(profileId);
        }

        private void SelectAllCompetenciesButton_Click(object sender, EventArgs e)
        {
            var department = Guid.Parse(SelectedDepartmentID.Value);
            SelectAllCompetencies(CurrentProfileIdentifier.Value, department);
        }

        private void UnselectAllCompetenciesButton_Click(object sender, EventArgs e)
        {
            var department = Guid.Parse(SelectedDepartmentID.Value);
            UnselectAllCompetencies(department);
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            _summaryTable = new DataTable();
            _summaryTable.Columns.Add("CompetencyStandardIdentifier", typeof(Guid));
            _summaryTable.Columns.Add("CompetencyNumber", typeof(string));
            _summaryTable.Columns.Add("CompetencySummary", typeof(string));

            for (var i = 1; i <= DepartmentPageSize; i++)
            {
                var fieldName = string.Format("Department{0}", i);
                _summaryTable.Columns.Add(fieldName, typeof(DepartmentCompetency));
            }

            LoadProfileCompetencies();
            LoadDepartmentCompetencies();

            Grid.DataSource = _summaryTable;
        }

        private void Grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Save(CurrentProfileIdentifier.Value);

            Grid.PageIndex = e.NewPageIndex;
            Grid.DataBind();
        }

        private void RemoveCompetenciesButton_Click(object sender, EventArgs e)
        {
            RemoveCompetencies();

            LoadData();
        }

        #endregion

        #region Init grid

        private void SetGridHeaderTemplates()
        {
            if (DepartmentNames == null)
                return;

            for (var i = 0; i < DepartmentPageSize; i++)
            {
                var dep = i < DepartmentNames.Length ? DepartmentNames[i] : null;

                var column = Grid.Columns[i + 2];
                column.HeaderText = dep == null ? null : dep.Name;
                //column.HeaderTemplate = dep == null ? null : new HeaderTemplate(this, column, dep.Identifier);
            }
        }

        #endregion

        #region Load data

        private void LoadData()
        {
            var hasDepartments = LoadDepartments();

            ResultTab.Visible = hasDepartments;

            if (!hasDepartments)
                return;

            var filter = GetCompetencyFilter(CurrentProfileIdentifier.Value);

            ResultTab.IsSelected = true;

            Grid.VirtualItemCount = Persistence.Plugin.CMDS.CompetencyRepository.CountSearchResults(filter);
            Grid.PageIndex = 0;
            Grid.DataBind();
        }

        private void LoadDataAfterCompetencyDeletedOrCopied(Guid profileId)
        {
            LoadProfileDetails(profileId);

            var filter = GetCompetencyFilter(profileId);

            Grid.VirtualItemCount = Persistence.Plugin.CMDS.CompetencyRepository.CountSearchResults(filter);

            Grid.DataBind();
        }

        private void LoadProfileDetails(Guid profileId)
        {
            var count = Persistence.Plugin.CMDS.ProfileCompetencyRepository.SelectCount(profileId, false);

            CompetencyCount.Text = string.Format("{0:n0} Competenc{1}", count, count == 1 ? "y" : "ies");
        }

        private bool LoadDepartments()
        {
            var departments = Departments;
            var departmentNames = new List<DepartmentName>();

            foreach (var departmentKey in departments)
            {
                var info = DepartmentSearch.Select(departmentKey);

                if (info == null || info.OrganizationIdentifier != OrganizationIdentifier)
                    continue;

                departmentNames.Add(new DepartmentName
                {
                    Identifier = info.DepartmentIdentifier,
                    Name = info.DepartmentName
                });
            }

            if (departmentNames.Count == 0)
                return false;

            DepartmentNames = departmentNames.ToArray();

            SetGridHeaderTemplates();

            return true;
        }

        #endregion

        #region Bind grid

        private void LoadProfileCompetencies()
        {
            var profileId = CurrentProfileIdentifier.Value;
            var filter = GetCompetencyFilter(profileId);

            filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var table = Persistence.Plugin.CMDS.CompetencyRepository.SelectSearchResults(filter);

            CompetencyMap = new Dictionary<Guid, int>();

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                var competencyID = (Guid)row["CompetencyStandardIdentifier"];

                CompetencyMap.Add(competencyID, i);

                var summaryRow = _summaryTable.NewRow();
                _summaryTable.Rows.Add(summaryRow);

                summaryRow["CompetencyStandardIdentifier"] = competencyID;
                summaryRow["CompetencyNumber"] = row["Number"];
                summaryRow["CompetencySummary"] = row["Summary"];

                for (var k = 0; k < DepartmentPageSize; k++)
                {
                    var dc = new DepartmentCompetency();

                    if (k < DepartmentNames.Length)
                    {
                        dc.DepartmentIdentifier = DepartmentNames[k].Identifier;
                        dc.CompetencyStandardIdentifier = competencyID;
                        dc.ProfileStandardIdentifier = profileId;
                    }

                    summaryRow[k + 3] = dc;
                }
            }
        }

        private void LoadDepartmentCompetencies()
        {
            var competencyStartRow = Grid.PageIndex * Grid.PageSize + 1;
            var competencyEndRow = competencyStartRow + Grid.PageSize - 1;

            var departments = new Guid[DepartmentNames.Length];

            for (var i = 0; i < DepartmentNames.Length; i++)
                departments[i] = DepartmentNames[i].Identifier;

            var table = Persistence.Plugin.CMDS.DepartmentProfileCompetencyRepository2.SelectCompetenciesByProfileId(departments, CurrentProfileIdentifier.Value, OrganizationIdentifier, competencyStartRow, competencyEndRow);

            foreach (DataRow row in table.Rows)
            {
                var departmentKey = (Guid)row["DepartmentIdentifier"];
                var competencyStandardIdentifier = (Guid)row["CompetencyStandardIdentifier"];

                var columnIndex = DepartmentMap[departmentKey];
                var rowIndex = CompetencyMap[competencyStandardIdentifier];

                var dc = (DepartmentCompetency)_summaryTable.Rows[rowIndex][columnIndex];

                dc.IsSelected = true;
                dc.ValidForCount = row["ValidForCount"] as int?;
                dc.ValidForUnit = row["ValidForUnit"] as string;
                dc.IsTimeSensitive = dc.ValidForCount.HasValue;
                dc.IsCritical = string.Equals(row["Criticality"] as string, "Critical", StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region Save

        private void Save(Guid profileId)
        {
            var insertList = new List<DepartmentProfileCompetency>();
            var deleteList = new List<DepartmentProfileCompetency>();

            for (var i = 0; i < Grid.Rows.Count; i++)
                GetItems(profileId, i, insertList, deleteList);

            if (insertList.Count > 0 || deleteList.Count > 0)
                DepartmentProfileCompetencyStore.InsertUpdateDelete(insertList, null, deleteList);
        }

        private void GetItems(Guid profileId, int rowIndex, List<DepartmentProfileCompetency> insertList, List<DepartmentProfileCompetency> deleteList)
        {
            var item = Grid.Rows[rowIndex];

            for (var i = 1; i <= DepartmentPageSize; i++)
            {
                var field = (DepartmentCompetencyField)item.FindControl(string.Format("Department{0}", i));

                if (field.DepartmentIdentifier == Guid.Empty || field.IsSelected == field.IsSelectedOld)
                    continue;

                var entity = DepartmentProfileCompetencySearch.SelectFirst(x => x.DepartmentIdentifier == field.DepartmentIdentifier && x.ProfileStandardIdentifier == profileId && x.CompetencyStandardIdentifier == field.CompetencyStandardIdentifier);

                if (field.IsSelected)
                {
                    if (entity == null)
                        insertList.Add(new DepartmentProfileCompetency { DepartmentIdentifier = field.DepartmentIdentifier, ProfileStandardIdentifier = profileId, CompetencyStandardIdentifier = field.CompetencyStandardIdentifier });
                }
                else if (entity != null)
                {
                    deleteList.Add(entity);
                }
            }
        }

        private void SelectAllCompetencies(Guid profileId, Guid departmentId)
        {
            Persistence.Plugin.CMDS.DepartmentProfileCompetencyRepository2.InsertProfileCompetencies(departmentId, profileId);

            Grid.DataBind();
        }

        private void UnselectAllCompetencies(Guid department)
        {
            var entities = DepartmentProfileCompetencySearch.Select(x => x.DepartmentIdentifier == department && x.ProfileStandardIdentifier == CurrentProfileIdentifier.Value);

            DepartmentProfileCompetencyStore.Delete(entities);

            Grid.DataBind();
        }

        #endregion

        #region Remove competencies

        private void RemoveCompetencies()
        {
            var competencies = GetCompetenciesToRemove();
            var removedCount = competencies.Length == 0 ? 0 : RemoveCompetencies(competencies);

            string text;

            if (removedCount == 0)
            {
                text = "No competencies to remove from departments.";
            }
            else
            {
                var competencyWord = removedCount == 1 ? "competency" : "competencies";
                text = string.Format("{0} {1} have been removed from departments.", removedCount, competencyWord);
            }

            RemovedLabel.Text = string.Format("<br/><br/><strong>{0}</strong>", text);
            RemovedLabel.Visible = true;
        }

        private int RemoveCompetencies(string[] competencies)
        {
            var profileId = CurrentProfileIdentifier.Value;
            var departments = Departments;
            var deleteList = new List<DepartmentProfileCompetency>();

            foreach (var competencyNumber in competencies)
            {
                var competency = Persistence.Plugin.CMDS.CompetencyRepository.Select(competencyNumber);

                if (competency == null)
                    continue;

                var competencyStandardIdentifier = competency.StandardIdentifier;

                var profileCompetency = Persistence.Plugin.CMDS.ProfileCompetencyRepository.Select(profileId, competencyStandardIdentifier);

                if (profileCompetency == null)
                    continue;

                foreach (var departmentKey in departments)
                {
                    var entity = DepartmentProfileCompetencySearch.SelectFirst(
                        x => x.DepartmentIdentifier == departmentKey
                          && x.ProfileStandardIdentifier == profileId
                          && x.CompetencyStandardIdentifier == competencyStandardIdentifier);

                    if (entity != null)
                        deleteList.Add(entity);
                }
            }

            DepartmentProfileCompetencyStore.Delete(deleteList);

            return deleteList.Count;
        }

        private string[] GetCompetenciesToRemove()
        {
            var result = new List<string>();

            var lineParts = CompetenciesToRemove.Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linePart in lineParts)
            {
                var commaParts = linePart.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var commaPart in commaParts)
                {
                    var s = commaPart.Trim();

                    if (!string.IsNullOrEmpty(s))
                        result.Add(s);
                }
            }

            return result.ToArray();
        }

        #endregion
    }
}