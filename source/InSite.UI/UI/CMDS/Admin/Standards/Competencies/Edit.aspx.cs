using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Standards;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

using CmdsRole = Shift.Constant.CmdsRole;

namespace InSite.Cmds.Admin.Competencies.Forms
{
    public partial class Edit : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string SearchUrl = "/ui/cmds/admin/standards/competencies/search";
        private const string CreateUrl = "/ui/cmds/admin/standards/competencies/create";
        private const string SelfUrl = "/ui/cmds/admin/standards/competencies/edit";

        #endregion

        #region Properties

        private Guid StandardIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "competency");

            UniqueNumber.ServerValidate += UniqueNumber_ServerValidate;

            ProfileGrid.EnablePaging = false;
            ProfileGrid.DataBinding += ProfileGrid_DataBinding;

            DepartmentGrid.DataBinding += DepartmentGrid_DataBinding;

            OnlyActiveCompanyProfiles.AutoPostBack = true;
            OnlyActiveCompanyProfiles.CheckedChanged += OnlyActiveCompanyProfiles_CheckedChanged;

            KnowledgeApplyButton.Click += KnowledgeApplyButton_Click;
            SkillsApplyButton.Click += SkillsApplyButton_Click;

            SaveButton.Click += SaveButton_Click;
            CopyButton.Click += CopyButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            UndeleteButton.Click += UndeleteButton_Click;

            DestroyButton.ServerClick += DestroyButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var status = Request.QueryString["status"];

            if (status == "copied")
                ScreenStatus.AddMessage(AlertType.Information, $"A copy of this competency has been made. You are now editing the copy.");
            else if (status == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            Access = Access.SetRead(true);
            Access = Access.SetCreate(StandardIdentifier != Guid.Empty && Access.Write);
            Access = Access.SetWrite(Access.Create && !(CompetencyRepository.Select(StandardIdentifier)?.IsDeleted ?? false));
            Access = Access.SetDelete(Access.Create && (Identity.IsInRole(CmdsRole.SystemAdministrators) || Identity.IsInRole(CmdsRole.Programmers)));

            EditKnowledgeButton.Visible = Access.Write;
            EditSkillsButton.Visible = Access.Write;
            SaveButton.Visible = Access.Write;
            CopyButton.Visible = Access.Write;
            DeleteButton.Visible = Access.Delete;
            UndeleteButton.Visible = Access.Delete;
        }

        #endregion

        #region Event handlers

        private void UniqueNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var info = CompetencyRepository.Select(Number.Text);

            args.IsValid = info == null || info.StandardIdentifier == StandardIdentifier;

            if (args.IsValid)
                return;

            UniqueNumber.ErrorMessage = info.IsDeleted
                ? $"Competency #{Number.Text} is assigned to a competency that is now deleted. If you re-number the deleted competency then you can re-use this number."
                : $"Competency #{Number.Text} is already in use by another competency.";
        }

        private void ProfileGrid_DataBinding(object sender, EventArgs e)
        {
            var filter = new ProfileCompetencyFilter { CompetencyStandardIdentifier = StandardIdentifier };

            if (OnlyActiveCompanyProfiles.Checked)
                filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            var table = ProfileCompetencyRepository.SelectByFilter(filter, null);

            ProfileGrid.DataSource = table;

            var profileText = table.Rows.Count == 1 ? "profile" : "profiles";
            ProfileCount.Text = string.Format("{0} {1}", table.Rows.Count, profileText);

            NoProfilePanel.Visible = table.Rows.Count == 0;
            ProfilePanel.Visible = table.Rows.Count > 0;
        }

        private void DepartmentGrid_DataBinding(object sender, EventArgs e)
        {
            DepartmentGrid.DataSource =
                DepartmentProfileCompetencyRepository2.SelectCompetenciesByOrganizationIdentifier(
                    CurrentIdentityFactory.ActiveOrganizationIdentifier, StandardIdentifier, DepartmentGrid.PageIndex,
                    DepartmentGrid.PageSize);
        }

        private void OnlyActiveCompanyProfiles_CheckedChanged(object sender, EventArgs e)
        {
            ProfileGrid.DataBind();
        }

        private void KnowledgeApplyButton_Click(object sender, EventArgs e)
        {
            KnowledgeHtml.Text = Markdown.ToHtml(KnowledgeValue.Value);
        }

        private void SkillsApplyButton_Click(object sender, EventArgs e)
        {
            SkillsHtml.Text = Markdown.ToHtml(SkillsValue.Value);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete(false))
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void DestroyButton_Click(object sender, EventArgs e)
        {
            var competency = StandardSearch.Select(StandardIdentifier, x => x.Organization);

            var isConfirmed = competency.Code == DestroyInput.Value;

            if (!isConfirmed)
                HttpResponseHelper.Redirect(Request.RawUrl, "status=delete-cancelled");

            else if (Delete(isConfirmed))
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void UndeleteButton_Click(object sender, EventArgs e)
        {
            Undelete();
            SetupCommandButtons();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            var id = Copy();
            if (id == Guid.Empty)
                return;

            var editUrl = SelfUrl + $"?id={id}&status=copied";

            HttpResponseHelper.Redirect(editUrl);
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            var info = StandardSearch.Select(StandardIdentifier, x => x.Organization);
            if (info == null || !string.Equals(info.StandardType, Shift.Constant.StandardType.Competency, StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                this,
                new BreadcrumbItem("Add New Competency", CreateUrl, null, null),
                "#" + info.Code);

            var canEdit = !info.IsHidden;

            SetInputValues(info);

            if (info.IsHidden)
                ScreenStatus.AddMessage(AlertType.Warning, "This competency is deleted.");

            AchievementEditor.SetEditable(canEdit, canEdit);
            AchievementEditor.LoadAchievements();

            ProfileGrid.DataBind();

            LoadCompanyCompetencyDetails(info);
            LoadDepartments(info);

            ValidationGrid.LoadData(info.StandardIdentifier);
            ValidationTab.SetTitle("Validations", ValidationGrid.RowCount);

            SetupCommandButtons();
        }

        private void LoadCompanyCompetencyDetails(Standard competency)
        {
            var isFound = VCmdsCompetencyOrganizationRepository.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier, StandardIdentifier) != null;

            NoCompanyCompetencyPanel.Visible = !isFound;
            CompanyCompetencyPanel.Visible = isFound;

            if (!isFound)
                NoCompanyCompetencyLabel.Text = string.Format("Competency {0} is not used by {1}.",
                    competency.Code,
                    Organization.CompanyName);
        }

        private void LoadDepartments(Standard competency)
        {
            var rowCount = DepartmentProfileCompetencyRepository2.SelectCompetenciesByOrganizationIdentifier(CurrentIdentityFactory.ActiveOrganizationIdentifier, StandardIdentifier, null, null).Rows.Count;
            var departmentText = rowCount == 1 ? "department" : "departments";
            DepartmentCount.Text = string.Format("{0} {1}", rowCount, departmentText);

            NoDepartmentsPanel.Visible = rowCount == 0;
            DepartmentsPanel.Visible = rowCount > 0;

            DepartmentGrid.VirtualItemCount = rowCount;
            DepartmentGrid.DataBind();

            NoDepartmentsLabel.Text = string.Format("Competency {0} is not in use by any departments in {1}.",
                competency.Code, Organization.CompanyName);
        }

        private void SetupCommandButtons()
        {
            if (!Access.Delete)
                return;

            var isDeleted = CompetencyRepository.Select(StandardIdentifier)?.IsDeleted ?? false;
            DeleteButton.Visible = !isDeleted;
            UndeleteButton.Visible = isDeleted;
        }

        private bool Save()
        {
            var info = StandardStore.Update(StandardIdentifier, asset =>
            {
                GetInputValues(asset);
                asset.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
            });

            StandardClassificationStore.ReplaceCategory(info.StandardIdentifier, CategoryIdentifier.ValueAsGuid);

            var author = $"{User.FullName} ({User.Email})";
            var change = $"Competency **{info.Code}** modified by **{author}**";
            var changed = new CmdsCompetencyChanged(author, change);

            ServiceLocator.ChangeQueue.Publish(changed);

            return true;
        }

        private Guid Copy()
        {
            var info = ServiceLocator.StandardSearch.GetStandard(StandardIdentifier);
            var copy = StandardFactory.Create(StandardType.Competency);

            copy.StandardIdentifier = UniqueIdentifier.Create();
            copy.AssetNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);

            copy.Code = info.Code + " - Copy";
            copy.ContentTitle = info.ContentTitle + " - Copy";
            copy.CreditHours = info.CreditHours;
            copy.ContentSummary = info.ContentSummary;
            copy.ContentDescription = info.ContentDescription;

            StandardStore.Insert(copy);

            var categoryId = StandardClassificationSearch.SelectFirstCategoryIdentifier(info.StandardIdentifier);
            if (categoryId.HasValue)
                StandardClassificationStore.ReplaceCategory(copy.StandardIdentifier, categoryId.Value);

            return copy.StandardIdentifier;
        }

        private bool Delete(bool force)
        {
            if (DisallowDeletion() && !force)
                return false;

            var info = StandardSearch.Select(StandardIdentifier);

            ServiceLocator.SendCommand(new ModifyStandardFieldBool(StandardIdentifier, StandardField.IsHidden, true));

            var author = $"{User.FullName} ({User.Email})";
            var change = $"Competency **{info.Code}** deleted by **{author}**";
            var changed = new CmdsCompetencyChanged(author, change);

            ServiceLocator.ChangeQueue.Publish(changed);

            return true;
        }

        private void Undelete()
        {
            ServiceLocator.SendCommand(new ModifyStandardFieldBool(StandardIdentifier, StandardField.IsHidden, false));

            ScreenStatus.AddMessage(AlertType.Success, "Competency was undeleted");
        }

        #endregion

        #region Setting and getting input values

        private void SetInputValues(Standard info)
        {
            CategoryIdentifier.EnsureDataBound();
            CategoryIdentifier.ValueAsGuid = StandardClassificationSearch.SelectFirstCategoryIdentifier(info.StandardIdentifier);
            Number.Text = info.Code;
            Summary.Text = info.ContentTitle;
            NumberOld.Text = info.SourceDescriptor;
            ProgramHours.ValueAsDecimal = info.CreditHours;

            NotUsedPanel.Visible = !string.IsNullOrEmpty(info.SourceDescriptor);

            KnowledgeValue.Value = info.ContentSummary;
            SkillsValue.Value = info.ContentDescription;

            KnowledgeHtml.Text = Markdown.ToHtml(KnowledgeValue.Value);
            SkillsHtml.Text = Markdown.ToHtml(SkillsValue.Value);
        }

        private void GetInputValues(QStandard info)
        {
            info.ContentTitle = Summary.Text;
            info.ContentSummary = KnowledgeValue.Value;
            info.ContentDescription = SkillsValue.Value;

            info.Code = Number.Text;
            info.SourceDescriptor = NumberOld.Text;
            info.CreditHours = ProgramHours.ValueAsDecimal;
        }

        #endregion

        #region Achievement Editor

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var standardId = StandardIdentifier;

            var standardAchievementIds = TAchievementStandardSearch
                .Bind(x => x.AchievementIdentifier, x => x.StandardIdentifier == standardId);

            var assignedAchievementIds = standardAchievementIds.Select(x => x).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            var list = TAchievementStandardSearch.Select(x => x.StandardIdentifier == StandardIdentifier && achievements.Contains(x.AchievementIdentifier));
            TAchievementStandardStore.Delete(list);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var list = new List<TAchievementStandard>();

            foreach (var achievementID in achievements)
                if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == StandardIdentifier && x.AchievementIdentifier == achievementID))
                    list.Add(new TAchievementStandard
                    {
                        StandardIdentifier = StandardIdentifier,
                        AchievementIdentifier = achievementID
                    });

            TAchievementStandardStore.Insert(list);

            return list.Count;
        }

        #endregion

        #region Check delete allowed

        private bool DisallowDeletion()
        {
            var groups = CompetencyRepository.SelectRelatedGroups(StandardIdentifier);
            var employees = CompetencyRepository.SelectRelatedEmployees(StandardIdentifier);
            var profiles = CompetencyRepository.SelectRelatedProfiles(StandardIdentifier);

            if (groups.Rows.Count == 0 && employees.Rows.Count == 0 && profiles.Rows.Count == 0)
                return false;

            var competency = CompetencyRepository.Select(StandardIdentifier);

            var names = new List<string>();

            if (groups.Rows.Count > 0)
                names.Add("organizations");

            if (employees.Rows.Count > 0)
                names.Add("employees");

            if (profiles.Rows.Count > 0)
                names.Add("profiles");

            var nameList = BuildNameList(names);

            var message = new StringBuilder();

            message.AppendFormat(
                "<p>Competency <strong>{0}</strong> is referenced by the following {1}:</p>",
                competency.Number, nameList);

            if (groups.Rows.Count > 0)
                AddRelatedGroups(message, groups, true);

            if (employees.Rows.Count > 0)
                AddRelatedEmployees(message, employees, true);

            if (profiles.Rows.Count > 0)
                AddRelatedProfiles(message, profiles, true);

            var isProgrammer = Identity.IsInRole(CmdsRole.Programmers);

            var instruction = isProgrammer
                ? "<p>Are you sure you want to delete the competency?</p>"
                : $"<p>Before you can delete the competency, remove it from the {nameList} that reference it.</p>";

            message.AppendFormat(instruction);

            ScreenStatus.AddMessage(AlertType.Warning, message.ToString());

            DestroyLabel1.Text = DestroyLabel2.Text = competency.Number;

            DestroyPanel.Visible = isProgrammer;

            return true;
        }

        private static string BuildNameList(IList<string> names)
        {
            var nameList = new StringBuilder();

            for (var i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    if (i == names.Count - 1)
                        nameList.Append(", and ");
                    else
                        nameList.Append(", ");

                nameList.Append(names[i]);
            }

            return nameList.ToString();
        }

        private void AddRelatedGroups(StringBuilder message, DataTable groups, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Organizations</strong>");

            message.Append("<ul>");

            var prevOrganizationIdentifier = Guid.Empty;
            var isDepartmentAdded = false;

            foreach (DataRow row in groups.Rows)
            {
                var organizationId = (Guid)row["OrganizationIdentifier"];
                var companyName = (string)row["CompanyName"];
                var departmentName = row["DepartmentName"] as string;

                if (prevOrganizationIdentifier != organizationId)
                {
                    if (prevOrganizationIdentifier != Guid.Empty)
                    {
                        if (isDepartmentAdded)
                        {
                            message.Append(" )");
                            isDepartmentAdded = false;
                        }

                        message.Append("</li>");
                    }

                    prevOrganizationIdentifier = organizationId;

                    message.AppendFormat("<li>{0}", companyName);
                }
                else
                {
                    if (isDepartmentAdded)
                    {
                        message.Append(", ");
                    }
                    else
                    {
                        message.Append("&nbsp;&nbsp;&nbsp;( ");
                        isDepartmentAdded = true;
                    }

                    message.Append(departmentName);
                }
            }

            if (isDepartmentAdded)
                message.Append(" )");

            message.Append("</li></ul>");
        }

        private void AddRelatedEmployees(StringBuilder message, DataTable employees, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Employees</strong>");

            message.Append("<ul><li>");

            foreach (DataRow row in employees.Rows)
            {
                var fullName = (string)row["FullName"];

                message.Append(fullName);

                if (employees.Rows.IndexOf(row) < employees.Rows.Count - 1)
                    message.Append(", ");
            }

            message.Append("</li></ul>");
        }

        private void AddRelatedProfiles(StringBuilder message, DataTable profiles, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Profiles</strong>");

            message.Append("<ul>");

            foreach (DataRow row in profiles.Rows)
            {
                var profileNumber = row["ProfileNumber"] as string;
                var profileTitle = (string)row["ProfileTitle"];

                message.AppendFormat("<li>{0}: {1}</li>", profileNumber, profileTitle);
            }

            message.Append("</ul>");
        }

        #endregion
    }
}