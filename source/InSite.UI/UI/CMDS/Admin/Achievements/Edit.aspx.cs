using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Admin.Achievements.Models;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class Edit : AdminBasePage
    {
        #region Check delete allowed

        private bool CheckDeleteAllowed()
        {
            var references = VCmdsAchievementHelper.BuildReferencesText(AchievementIdentifier);
            if (references == null || references.Count == 0)
                return true;

            var achievement = VCmdsAchievementSearch.Select(AchievementIdentifier);

            var message = new StringBuilder();

            message.Append($"You can't delete this achievement ({achievement.AchievementTitle}) because it is referenced {"time".ToQuantity(references.Count)} by other data in the system: {references.ToHtml()}");

            EditorStatus.AddMessage(AlertType.Error, message.ToString());

            return false;
        }

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds() { }

        #endregion

        #region Fields

        private bool _showAchievementFileDeletePopup;
        private bool _isDeleteConfirmed;

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/cmds/admin/achievements/search";

        private Guid AchievementIdentifier =>
            Guid.TryParse(Request["id"], out var id) ? id : Guid.Empty;

        private string Panel => Request.QueryString["panel"];

        private bool IsNewCompetenciesSearched
        {
            get => ViewState[nameof(IsNewCompetenciesSearched)] != null &&
                   (bool)ViewState[nameof(IsNewCompetenciesSearched)];
            set => ViewState[nameof(IsNewCompetenciesSearched)] = value;
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
            ConfirmDeleteButton.Click += ConfirmDeleteButton_Click;

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            DuplicateButton.Click += DuplicateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                ViewReferencesButton.NavigateUrl = $"/ui/cmds/admin/achievements/view-references?achievement={AchievementIdentifier}";
                CancelButton.NavigateUrl = SearchUrl;

                SearchAchievement.Filter.ExcludeAchievementIdentifier = AchievementIdentifier;

                Open();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", CompetencyPanel.ClientID);
            UnselectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", CompetencyPanel.ClientID);

            DeleteCompetencyButton.OnClientClick = "return confirm('Are you sure you want to delete selected competencies?');";

            SelectAllButton2.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", CompetencyList.ClientID);
            UnselectAllButton2.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", CompetencyList.ClientID);

            if (_showAchievementFileDeletePopup)
            {
                var popupScript = string.Format("showAchievementFileDeleteConfirm('{0}');", AchievementIdentifier);
                Page.ClientScript.RegisterStartupScript(GetType(), "ShowPopup", popupScript, true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void AddMultipleButton_Click(object sender, EventArgs e)
        {
            AddMultipleCompetencies();
        }

        private void DeleteCompetencyButton_Click(object sender, EventArgs e)
        {
            DeleteCompetencies();
        }

        private void AddCompetencyButton_Click(object sender, EventArgs e)
        {
            AddCompetency();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = true;
            LoadCompetencies();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = false;
            SearchText.Text = null;
            SearchAchievement.Value = null;
            LoadCompetencies();
        }

        private void ConfirmDeleteButton_Click(object sender, EventArgs e)
        {
            _isDeleteConfirmed = true;

            if (Delete())
                Response.Redirect(SearchUrl);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete())
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void DuplicateButton_Click(object sender, EventArgs e)
        {
            Duplicate();
        }

        #endregion

        #region Loading and saving

        private void Open()
        {
            var info = VCmdsAchievementSearch.Select(AchievementIdentifier);
            if (info == null)
                HttpResponseHelper.Redirect(SearchUrl);

            if (!info.AchievementIsEnabled)
            {
                SaveButton.Visible = false;
                EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
            }

            if (Panel == "credentials")
                CredentialSection.IsSelected = true;

            PageHelper.AutoBindHeader(this, null, info.AchievementTitle);

            AchievementDetails.SetInputValues(info);

            var count = DepartmentChecklist.LoadAchievements(info.AchievementIdentifier);
            DepartmentSection.SetTitle("Departments", count);

            LoadCompetencies();

            var returnUrl = $"/ui/cmds/admin/achievements/edit?id={AchievementIdentifier}&panel=credentials";

            CredentialGrid.LoadDataByAchievementID(info.AchievementIdentifier, false, returnUrl);
            CredentialSection.SetTitle("Learners", CredentialGrid.RowCount);
        }

        private bool Save()
        {
            var entity = VCmdsAchievementSearch.Select(AchievementIdentifier);
            if (entity == null)
                return false;

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

            if (info.OrganizationIdentifier == OrganizationIdentifiers.CMDS && entity.AchievementLabel == "Module")
                VCmdsAchievementSearch.PublishGlobalModule();

            return true;
        }

        private void Duplicate()
        {
            var info = AchievementDetails.GetInputValues();
            info.AchievementTitle = $"Copy of {info.AchievementTitle}";

            var expiration = info.LifetimeMonths.HasValue
                ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = info.LifetimeMonths.Value, Unit = "Month" } }
                : new Expiration { Type = ExpirationType.None };

            var command = new CreateAchievement(UniqueIdentifier.Create(), Organization.Identifier, info.AchievementLabel, info.AchievementTitle, info.AchievementDescription, expiration);
            command.OriginOrganization = info.OrganizationIdentifier == OrganizationIdentifiers.CMDS ? CurrentIdentityFactory.ActiveOrganizationIdentifier : info.OrganizationIdentifier;

            ServiceLocator.SendCommand(command);

            AchievementDetails.SaveAchievementCategoriesAndAccessControl(command.AggregateIdentifier);

            TAchievementOrganizationStore.InsertOrganizationAchievement(Organization.Identifier, command.AggregateIdentifier);

            var editUrl = $"/ui/cmds/admin/achievements/edit?id={command.AggregateIdentifier}&status=copied";
            HttpResponseHelper.Redirect(editUrl);
        }

        private bool Delete()
        {
            if (!CheckDeleteAllowed())
                return false;

            if (!_isDeleteConfirmed && UploadRepository.IsAchievementHasUploads(AchievementIdentifier))
            {
                _showAchievementFileDeletePopup = true;
                return false;
            }

            var removeFiles = new List<Upload>();

            UploadRepository.DetachAchievement(AchievementIdentifier);

            foreach (var uploadInfo in UploadRepository.SelectAchievementUploads(AchievementIdentifier))
            {
                UploadRepository.DeleteRelations(uploadInfo.UploadIdentifier);

                if (uploadInfo.UploadType == UploadType.CmdsFile)
                    removeFiles.Add(uploadInfo);
                else
                    UploadStore.DeleteLink(uploadInfo.UploadIdentifier);
            }

            var list = TAchievementClassificationSearch.Select(x => x.AchievementIdentifier == AchievementIdentifier);
            TAchievementClassificationStore.Delete(list);

            ServiceLocator.SendCommand(new DeleteAchievement(AchievementIdentifier, true));

            using (var fileStorage = CmdsUploadProvider.Current.CreateFileStorage())
            {
                foreach (var uploadInfo in removeFiles)
                    CmdsUploadProvider.Delete(uploadInfo.ContainerIdentifier, uploadInfo.Name, fileStorage);

                fileStorage.Commit();
            }

            return true;
        }

        #endregion

        #region Competencies methods

        private void LoadCompetencies()
        {
            var data = CompetencyRepository.SelectAchievementCompetencies(AchievementIdentifier);

            Competencies.DataSource = data;
            Competencies.DataBind();
            CompetencyTab.SetTitle("Competencies", data.Rows.Count);
            CompetencyTab.Visible = data.Rows.Count > 0;

            CompetencySection.SetTitle("Competencies", data.Rows.Count);

            if (IsNewCompetenciesSearched)
            {
                var newCompetencies = CompetencyRepository.SelectNewAchievementCompetencies(AchievementIdentifier, SearchText.Text, SearchAchievement.Value);

                NewCompetencies.DataSource = newCompetencies;
                NewCompetencies.DataBind();

                CompetencyList.Visible = newCompetencies.Rows.Count > 0;

                FoundCompetency.Visible = true;

                if (newCompetencies.Rows.Count > 0)
                    FoundCompetency.InnerHtml =
                        string.Format("Found {0} competencies:", newCompetencies.Rows.Count);
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

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                if (!TAchievementStandardSearch.Exists(x =>
                    x.StandardIdentifier == competencyID && x.AchievementIdentifier == AchievementIdentifier))
                    list.Add(new TAchievementStandard
                    {
                        StandardIdentifier = competencyID,
                        AchievementIdentifier = AchievementIdentifier
                    });
            }

            TAchievementStandardStore.Insert(list);

            LoadCompetencies();
        }

        private void AddMultipleCompetencies()
        {
            var text = MultipleCompetencyNumbers.Text;

            if (string.IsNullOrEmpty(text))
                return;

            var numbers = StringHelper.Split(text);

            var list = new List<TAchievementStandard>();

            foreach (var number in numbers)
            {
                var competencies = CompetencyRepository.SelectByNumber(number);

                foreach (var competency in competencies)
                    if (!TAchievementStandardSearch.Exists(x =>
                        x.StandardIdentifier == competency.StandardIdentifier && x.AchievementIdentifier == AchievementIdentifier))
                        list.Add(new TAchievementStandard
                        {
                            StandardIdentifier = competency.StandardIdentifier,
                            AchievementIdentifier = AchievementIdentifier
                        });
            }

            TAchievementStandardStore.Insert(list);

            EditorStatus.AddMessage(AlertType.Success, $"{list.Count:n0} competencies have been added to this achievement");

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

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                list.Add(competencyID);
            }

            var standards = TAchievementStandardSearch.Select(x => x.AchievementIdentifier == AchievementIdentifier && list.Contains(x.StandardIdentifier));

            TAchievementStandardStore.Delete(standards);

            LoadCompetencies();
        }

        #endregion
    }
}
