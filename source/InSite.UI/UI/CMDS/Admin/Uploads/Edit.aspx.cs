using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Path = System.IO.Path;

namespace InSite.Cmds.Admin.Uploads.Forms
{
    public partial class Edit : AdminBasePage
    {
        #region Load competencies

        private void LoadCompetencies()
        {
            var data = CompetencyRepository.SelectUploadCompetencies(UploadID.Value);

            Competencies.DataSource = data;
            Competencies.DataBind();
            CompetencyTab.SetTitle("Competencies", data.Rows.Count);
            CompetencyTab.Visible = data.Rows.Count > 0;

            CompetencySection.SetTitle("Competencies", data.Rows.Count);

            if (IsNewCompetenciesSearched)
            {
                var newCompetencies = CompetencyRepository.SelectNewCompanyUploadCompetencies(UploadID.Value, CurrentIdentityFactory.ActiveOrganizationIdentifier, SearchText.Text);
                NewCompetencies.DataSource = newCompetencies;
                NewCompetencies.DataBind();

                CompetencyList.Visible = newCompetencies.Rows.Count > 0;

                FoundCompetency.Visible = true;

                FoundCompetency.InnerHtml = newCompetencies.Rows.Count > 0
                    ? string.Format("Found {0} competencies:", newCompetencies.Rows.Count)
                    : FoundCompetency.InnerHtml = "No competencies found";
            }
            else
            {
                NewCompetencies.DataSource = null;
                NewCompetencies.DataBind();

                CompetencyList.Visible = false;
                FoundCompetency.Visible = false;
            }
        }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/cmds/admin/uploads/search";

        private bool IsNewCompetenciesSearched { get; set; }

        private bool IsFile { get; set; }

        private Guid? UploadID
        {
            get
            {
                var value = Request.QueryString["id"];

                return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out var id) ? id : (Guid?)null;
            }
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "download");

            DeleteCompetencyButton.Click += DeleteCompetencyButton_Click;
            AddCompetencyButton.Click += AddCompetencyButton_Click;
            AddMultipleButton.Click += AddMultipleButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                Open();

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", CompetencyPanel.ClientID);
            UnselectAllButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", CompetencyPanel.ClientID);

            DeleteCompetencyButton.OnClientClick = "return confirm('Are you sure you want to delete selected qualifications?');";

            SelectAllButton2.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", CompetencyList.ClientID);
            UnselectAllButton2.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", CompetencyList.ClientID);

            base.OnPreRender(e);
        }

        #endregion

        #region Save & Load view state

        protected override object SaveViewState()
        {
            return new[]
            {
                IsNewCompetenciesSearched,
                IsFile,
                base.SaveViewState()
            };
        }

        protected override void LoadViewState(object savedState)
        {
            var list = (object[])savedState;

            IsNewCompetenciesSearched = (bool)list[0];
            IsFile = (bool)list[1];

            base.LoadViewState(list[2]);
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
            LoadCompetencies();
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
            CompanyAchievementHelper.Delete(UploadID.Value, CurrentIdentityFactory.ActiveOrganizationIdentifier, User);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var entity = UploadID.HasValue ? UploadSearch.Select(UploadID.Value) : null;
            if (entity == null || !HasAccess(entity))
                HttpResponseHelper.Redirect(SearchUrl);

            SetInputValues(entity);

            CompetencySection.Visible = entity.ContainerType == UploadContainerType.Oganization;
            AchievementSection.Visible = entity.ContainerType == UploadContainerType.Oganization;

            if (entity.ContainerType == UploadContainerType.Oganization)
            {
                LoadCompetencies();

                AchievementEditor.SetEditable(CanEdit, CanEdit);
                var achievementCount = AchievementEditor.LoadAchievements(GroupByEnum.Type);
                AchievementSection.SetTitle("Achievements", achievementCount);
            }

            NavigationUrl.ReadOnly = false;
            TitleInput.ReadOnly = false;
            Description.ReadOnly = false;

            GlobalAchievementPanel.Visible = false;
        }

        private bool HasAccess(Upload entity)
        {
            if (entity.ContainerType == UploadContainerType.Asset)
                return false;

            if (entity.ContainerType == UploadContainerType.ContactExperience)
                return VCmdsCredentialAndExperienceSearch.Exists(x => x.ExperienceIdentifier == entity.ContainerIdentifier);
            // && x.User.TenantIdentifier == Identity.Tenant.Identifier);

            if (entity.ContainerType == UploadContainerType.Oganization)
                return OrganizationSearch.Select(entity.ContainerIdentifier)?.OrganizationIdentifier == CurrentIdentityFactory.ActiveOrganizationIdentifier;

            return false;
        }

        private bool Save()
        {
            if (IsFile)
            {
                CmdsUploadProvider.Current.Update(UploadID.Value, model => GetFileInputValues(model));
                return true;
            }

            var upload = UploadSearch.Select(UploadID.Value);
            if (upload == null)
                return true;

            GetLinkInputValues(upload);

            if (UploadSearch.Exists(x => x.UploadIdentifier != upload.UploadIdentifier && x.ContainerIdentifier == upload.ContainerIdentifier && x.Name == upload.Name))
            {
                EditorStatus.AddMessage(AlertType.Error, string.Format("This link already exists: {0}", upload.Name));
                return false;
            }

            var entity = UploadStore.UpdateLink(upload);

            return entity != null;
        }

        #endregion

        #region Getting and setting input values

        private void SetInputValues(Upload entity)
        {
            var organization = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier);

            PageHelper.AutoBindHeader(this, null, $"{entity.Title} ({organization.CompanyName})");

            TitleInput.Text = entity.Title;
            LastUpdatedOn.Text = string.Format("{0:MMM d, yyyy}", entity.Uploaded);
            Description.Text = entity.Description;

            IsFile = entity.UploadType == UploadType.CmdsFile;
            FileRow.Visible = IsFile;
            SizeInKiloBytesRow.Visible = IsFile;
            SizeInKiloBytes.Text = IsFile ? string.Format("{0:n0}", entity.ContentSize.Value / 1024) : null;

            UrlRow.Visible = !IsFile;

            if (IsFile)
            {
                FileName.Text = Path.GetFileNameWithoutExtension(entity.Name);
                FileExtension.Text = Path.GetExtension(entity.Name);
                FileLink.NavigateUrl = CmdsUploadProvider.GetFileRelativeUrl(entity);
            }
            else
            {
                NavigationUrl.Text = entity.Name;
            }
        }

        private void GetLinkInputValues(Upload entity)
        {
            entity.Title = TitleInput.Text;
            entity.Description = Description.Text;
            entity.Name = NavigationUrl.Text;
        }

        private void GetFileInputValues(ICmdsUploadModel model)
        {
            if (model == null)
                return;

            model.Title = TitleInput.Text;
            model.Description = Description.Text;
            model.Name = FileName.Text + Path.GetExtension(model.Name);
        }

        #endregion

        #region Save & Delete Download Competencies

        private void AddCompetency()
        {
            var insertedCompetencies = new List<Guid>();

            foreach (RepeaterItem item in NewCompetencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                UploadRepository.AttachCompetency(UploadID.Value, competencyID);

                insertedCompetencies.Add(competencyID);
            }

            AddCompetenciesToAchievements(insertedCompetencies.ToArray());

            LoadCompetencies();
        }

        private void AddMultipleCompetencies()
        {
            var text = MultipleCompetencyNumbers.Text;

            if (string.IsNullOrEmpty(text))
                return;

            var count = 0;

            var numbers = StringHelper.Split(text);

            var insertedCompetencies = new List<Guid>();

            foreach (var number in numbers)
            {
                var list = CompetencyRepository.SelectByNumber(number);

                foreach (var competency in list)
                    if (UploadRepository.AttachCompetency(UploadID.Value, competency.StandardIdentifier))
                    {
                        count++;
                        insertedCompetencies.Add(competency.StandardIdentifier);
                    }
            }

            AddCompetenciesToAchievements(insertedCompetencies.ToArray());

            EditorStatus.AddMessage(AlertType.Success,
                string.Format("{0:n0} competencies have been added to this download", count));

            MultipleCompetencyNumbers.Text = string.Empty;
            LoadCompetencies();
        }

        private void AddCompetenciesToAchievements(Guid[] competencies)
        {
            var relatedUploadAchievements = UploadRepository.SelectAchievements(UploadID.Value);

            var list = new List<TAchievementStandard>();

            foreach (var competencyId in competencies)
            {
                foreach (var achievementId in relatedUploadAchievements)
                {
                    if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == competencyId && x.AchievementIdentifier == achievementId))
                    {
                        if (!list.Any(x => x.StandardIdentifier == competencyId && x.AchievementIdentifier == achievementId))
                            list.Add(new TAchievementStandard
                            {
                                StandardIdentifier = competencyId,
                                AchievementIdentifier = achievementId
                            });
                    }
                }
            }

            TAchievementStandardStore.Insert(list);
        }

        private void DeleteCompetencies()
        {
            foreach (RepeaterItem item in Competencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                UploadRepository.DetachCompetency(UploadID.Value, competencyID);
            }

            LoadCompetencies();
        }

        #endregion

        #region Achievement Editor

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var uploadId = UploadID.Value;

            var uploadAchievementIds = VCmdsAchievementSearch.SelectNewUploadAchievements(null, uploadId, null);

            var assignedAchievementIds = uploadAchievementIds.Select(x => x.AchievementIdentifier).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            foreach (var achievementID in achievements)
                UploadRepository.DetachAchievement(UploadID.Value, achievementID);
        }

        private int InsertAchievements(IEnumerable<Guid> achievementes)
        {
            var count = 0;

            Guid[] uploadCompetencies = UploadRepository.SelecteCompetencies(UploadID.Value);

            var achievementCompetencies = new List<TAchievementStandard>();

            foreach (var achievementID in achievementes)
            {
                if (UploadRepository.AttachAchievement(UploadID.Value, achievementID))
                    count++;

                foreach (var competencyId in uploadCompetencies)
                    if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == competencyId && x.AchievementIdentifier == achievementID))
                        achievementCompetencies.Add(new TAchievementStandard
                        {
                            StandardIdentifier = competencyId,
                            AchievementIdentifier = achievementID
                        });
            }

            TAchievementStandardStore.Insert(achievementCompetencies);

            return count;
        }

        #endregion
    }
}
