using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Admin.Achievements.Models;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class Edit2 : AdminBasePage, ICmdsUserControl
    {
        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (!Access.Read || AchievementIdentifier == Guid.Empty)
                Access = Access.SetAll(false);

            var info = VCmdsAchievementSearch.Select(AchievementIdentifier);
            if (info?.OrganizationIdentifier != CurrentIdentityFactory.ActiveOrganizationIdentifier)
                Access = Access.SetAll(false);

            AchievementDetails.DisableEdit();

            Access = Access.SetDelete(false);
            Access = Access.SetCreate(false);

            SaveButton.Visible = CanEdit;
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/cmds/design/achievements/search";

        private Guid AchievementIdentifier =>
            Guid.TryParse(Request["id"], out var id) ? id : Guid.Empty;

        private bool IsNewCompetenciesSearched
        {
            get { return ViewState[nameof(IsNewCompetenciesSearched)] != null && (bool)ViewState[nameof(IsNewCompetenciesSearched)]; }
            set { ViewState[nameof(IsNewCompetenciesSearched)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCompetencyButton.Click += DeleteCompetencyButton_Click;
            AddCompetencyButton.Click += AddCompetencyButton_Click;
            AddMultipleButton.Click += AddMultipleButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                Open();

                SearchAchievement.Filter.ExcludeAchievementIdentifier = AchievementIdentifier;

                ViewReferencesButton.NavigateUrl = $"/ui/cmds/admin/achievements/view-references?achievement={AchievementIdentifier}&return=design";
                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", CompetencyPanel.ClientID);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", CompetencyPanel.ClientID);

            DeleteCompetencyButton.OnClientClick = "return confirm('Are you sure you want to delete selected competencies?');";

            SelectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', true);", CompetencyList.ClientID);
            UnselectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', false);", CompetencyList.ClientID);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void AddMultipleButton_Click(Object sender, EventArgs e)
        {
            AddMultipleCompetencies();
        }

        private void DeleteCompetencyButton_Click(Object sender, EventArgs e)
        {
            DeleteCompetencies();
        }

        private void AddCompetencyButton_Click(Object sender, EventArgs e)
        {
            AddCompetency();
        }

        private void FilterButton_Click(Object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = true;
            LoadCompetencies();
        }

        private void ClearButton_Click(Object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = false;
            SearchText.Text = null;
            SearchAchievement.Value = null;
            LoadCompetencies();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        #endregion

        #region Loading and saving

        private void Open()
        {
            var info = VCmdsAchievementSearch.Select(AchievementIdentifier);
            if (info == null)
                HttpResponseHelper.Redirect("/ui/cmds/design/achievements/search", true);

            if (!info.AchievementIsEnabled)
            {
                SaveButton.Visible = false;
                EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
            }

            AchievementDetails.SetInputValues(info);

            var departmentCount = DepartmentChecklist.LoadAchievements(info.AchievementIdentifier);
            DepartmentSection.SetTitle("Departments", departmentCount);

            LoadCompetencies();
        }

        private void Save()
        {
            var entity = VCmdsAchievementSearch.Select(AchievementIdentifier);

            var info = AchievementDetails.GetInputValues();

            var commands = new List<Command>();

            if (entity.OrganizationIdentifier != info.OrganizationIdentifier)
                commands.Add(new ChangeAchievementOrganization(AchievementIdentifier, entity.OrganizationIdentifier));

            if (entity.OrganizationIdentifier != info.OrganizationIdentifier || entity.ValidForCount != info.LifetimeMonths || entity.ValidForUnit != "Month")
            {
                var expiration = info.LifetimeMonths.HasValue
                    ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = info.LifetimeMonths.Value, Unit = "Month" } }
                    : null;

                commands.Add(new ChangeAchievementExpiry(AchievementIdentifier, expiration));
            }

            if (!string.Equals(entity.AchievementTitle, info.AchievementTitle)
                || !string.Equals(entity.AchievementLabel, info.AchievementLabel)
                || !string.Equals(entity.AchievementDescription, info.AchievementDescription)
                || entity.AchievementAllowSelfDeclared != info.AllowSelfDeclared
                )
            {
                commands.Add(new DescribeAchievement(AchievementIdentifier, info.AchievementLabel, info.AchievementTitle, info.AchievementDescription, info.AllowSelfDeclared));
            }

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            DepartmentChecklist.SaveChanges();

            AchievementDetails.SaveAchievementCategoriesAndAccessControl(AchievementIdentifier);

            var before = new CmdsAchievementChanged
            {
                OrganizationIdentifier = entity.OrganizationIdentifier,
                Type = entity.AchievementLabel,
                Title = entity.AchievementTitle,
                Lifetime = entity.ValidForCount ?? 0,
                Description = entity.AchievementDescription,
                Hours = 0m
            };

            var after = new CmdsAchievementChanged
            {
                OrganizationIdentifier = info.OrganizationIdentifier,
                Type = info.AchievementLabel,
                Title = info.AchievementTitle,
                Lifetime = info.LifetimeMonths ?? 0,
                Description = info.AchievementDescription,
                Hours = 0m
            };

            var author = new EmailAddress(User.Email, User.FullName);

            var observer = new AchievementObserver(ServiceLocator.ChangeQueue, author, before);

            observer.Modified(after);
        }

        #endregion

        #region Competencies methods

        private void LoadCompetencies()
        {
            DataTable data = CompetencyRepository.SelectAchievementCompetencies(AchievementIdentifier);

            Competencies.DataSource = data;
            Competencies.DataBind();
            CompetencyTab.SetTitle("Competencies", data.Rows.Count);
            CompetencyTab.Visible = data.Rows.Count > 0;

            CompetencySection.SetTitle("Competencies", data.Rows.Count);

            if (IsNewCompetenciesSearched)
            {
                DataTable newCompetencies = CompetencyRepository.SelectNewAchievementCompetencies(AchievementIdentifier, SearchText.Text, SearchAchievement.Value);

                NewCompetencies.DataSource = newCompetencies;
                NewCompetencies.DataBind();

                CompetencyList.Visible = newCompetencies.Rows.Count > 0;

                FoundCompetency.Visible = true;

                if (newCompetencies.Rows.Count > 0)
                    FoundCompetency.InnerHtml = string.Format("Found {0} competencies:", newCompetencies.Rows.Count);
                else
                    FoundCompetency.InnerHtml = "No competencies found";
            }
            else
            {
                NewCompetencies.DataSource = null;
                NewCompetencies.DataBind();

                CompetencyList.Visible = false;
                FoundCompetency.Visible = false;
            }
        }

        private void AddCompetency()
        {
            var list = new List<TAchievementStandard>();

            foreach (RepeaterItem item in NewCompetencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyStandardIdentifierControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyStandardIdentifier = Guid.Parse(competencyStandardIdentifierControl.Text);

                if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == competencyStandardIdentifier && x.AchievementIdentifier == AchievementIdentifier))
                    list.Add(new TAchievementStandard { StandardIdentifier = competencyStandardIdentifier, AchievementIdentifier = AchievementIdentifier });
            }

            TAchievementStandardStore.Insert(list);

            LoadCompetencies();
        }

        private void AddMultipleCompetencies()
        {
            string text = MultipleCompetencyNumbers.Text;

            if (string.IsNullOrEmpty(text))
                return;

            string[] numbers = StringHelper.Split(text);

            var list = new List<TAchievementStandard>();

            foreach (string number in numbers)
            {
                var competencies = CompetencyRepository.SelectByNumber(number);

                foreach (var competency in competencies)
                {
                    if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == competency.StandardIdentifier && x.AchievementIdentifier == AchievementIdentifier))
                        list.Add(new TAchievementStandard { StandardIdentifier = competency.StandardIdentifier, AchievementIdentifier = AchievementIdentifier });
                }
            }

            TAchievementStandardStore.Insert(list);

            EditorStatus.AddMessage(AlertType.Success, string.Format("{0:n0} competencies have been added to this achievement", list.Count));

            MultipleCompetencyNumbers.Text = string.Empty;
            LoadCompetencies();
        }

        private void DeleteCompetencies()
        {
            var list = new List<Guid>();

            foreach (RepeaterItem item in Competencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyStandardIdentifierControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyStandardIdentifier = Guid.Parse(competencyStandardIdentifierControl.Text);

                list.Add(competencyStandardIdentifier);
            }

            var entities = TAchievementStandardSearch.Select(x => x.AchievementIdentifier == AchievementIdentifier && list.Contains(x.StandardIdentifier));
            TAchievementStandardStore.Delete(entities);

            LoadCompetencies();
        }

        #endregion
    }
}
