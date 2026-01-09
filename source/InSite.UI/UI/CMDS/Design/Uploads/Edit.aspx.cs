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
using Shift.Common.Events;
using Shift.Constant;
using Shift.Contract;

using AlertType = Shift.Constant.AlertType;
using Path = System.IO.Path;

namespace InSite.Cmds.Design.Uploads
{
    public partial class Edit : AdminBasePage
    {
        #region Security

        public override void ApplyAccessControl()
        {
            if (!IsAchievementUploader || OrganizationIdentifier != CurrentIdentityFactory.ActiveOrganizationIdentifier)
            {
                if (!Identity.IsGranted(PermissionNames.Custom_CMDS_Administrators))
                    CreateAccessDeniedException();
            }

            base.ApplyAccessControl();

            SaveButton.Visible = IsCompanySpecific;
            DeleteButton.Visible = IsCompanySpecific;
        }

        #endregion

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
                var newCompetencies = CompetencyRepository.SelectNewCompanyUploadCompetencies(UploadID.Value, OrganizationIdentifier, SearchText.Text);
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

        #endregion

        #region Load company

        private void LoadCompany()
        {
            var containerId = UploadSearch.BindFirst(x => x.ContainerIdentifier, x => x.UploadIdentifier == UploadID.Value);
            var organizationId = OrganizationSearch.Select(containerId)?.OrganizationIdentifier;

            _organizationId = organizationId ?? CurrentIdentityFactory.ActiveOrganizationIdentifier;
            _isCompanySpecific = organizationId.HasValue;
        }

        #endregion

        #region Fields

        private Guid _organizationId;
        private bool _isCompanySpecific;

        #endregion

        #region Properties

        private bool IsAchievementUploader =>
            StringHelper.Equals(Request["achievement-uploader"], "yes");

        private Guid OrganizationIdentifier
        {
            get
            {
                if (_organizationId == Guid.Empty)
                    LoadCompany();

                return _organizationId;
            }
        }

        private bool IsCompanySpecific
        {
            get
            {
                if (_organizationId == Guid.Empty)
                    LoadCompany();

                return _isCompanySpecific;
            }
        }

        private bool IsNewCompetenciesSearched { get; set; }

        private bool IsFile
        {
            get => (bool)ViewState[nameof(IsFile)];
            set => ViewState[nameof(IsFile)] = value;
        }

        private Guid? UploadID
        {
            get
            {
                var value = Request.QueryString["id"];

                return !string.IsNullOrEmpty(value) && Guid.TryParse(value, out var id) ? id : (Guid?)null;
            }
        }

        private string SearchUrl
        {
            get
            {
                return IsAchievementUploader
                    ? string.Format("/ui/cmds/admin/uploads/upload?id={0}", OrganizationIdentifier)
                    : string.Format("/ui/cmds/design/uploads/search?id={0}", OrganizationIdentifier);
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

            AchievementEditor.Refreshed += AchievementEditor_Refreshed;

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

            if (IsPostBack)
                return;

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick = $"return setCheckboxes('{CompetencyPanel.ClientID}', true);";
            UnselectAllButton.OnClientClick = $"return setCheckboxes('{CompetencyPanel.ClientID}', false);";

            DeleteCompetencyButton.OnClientClick = "return confirm('Are you sure you want to delete selected qualifications?');";

            SelectAllButton2.OnClientClick = $"return setCheckboxes('{CompetencyList.ClientID}', true);";
            UnselectAllButton2.OnClientClick = $"return setCheckboxes('{CompetencyList.ClientID}', false);";

            base.OnPreRender(e);
        }

        #endregion

        #region Save & Load view state

        protected override object SaveViewState()
        {
            return new[]
            {
                _organizationId,
                _isCompanySpecific,
                IsNewCompetenciesSearched,
                base.SaveViewState()
            };
        }

        protected override void LoadViewState(object savedState)
        {
            var list = (object[])savedState;

            _organizationId = (Guid)list[0];
            _isCompanySpecific = (bool)list[1];
            IsNewCompetenciesSearched = (bool)list[2];

            base.LoadViewState(list[3]);
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

        private void AchievementEditor_Refreshed(object sender, IntValueArgs e)
        {
            SetAchievementSectionTitle(e.Value);
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
            CompanyAchievementHelper.Delete(UploadID.Value, OrganizationIdentifier, User);

            HttpResponseHelper.Redirect(SearchUrl);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var entity = UploadSearch.Select(UploadID.Value);
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            SetInputValues(entity);

            LoadCompetencies();

            AchievementEditor.SetEditable(CanEdit, CanEdit);
            var achievementCount = AchievementEditor.LoadAchievements(GroupByEnum.Type);
            SetAchievementSectionTitle(achievementCount);

            NavigationUrl.ReadOnly = !IsCompanySpecific;
            TitleInput.ReadOnly = !IsCompanySpecific;
            Description.ReadOnly = !IsCompanySpecific;

            GlobalAchievementPanel.Visible = !IsCompanySpecific;
        }

        private void SetAchievementSectionTitle(int count)
        {
            AchievementCount.Text = $"<span class='form-text'>{count:n0}</span>";
        }

        private bool Save()
        {
            if (IsFile)
            {
                CmdsUploadProvider.Current.Update(UploadID.Value, model => GetFileInputValues(model));
                return true;
            }

            var upload = UploadSearch.Select(UploadID.Value);

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
            var organization = OrganizationSearch.Select(OrganizationIdentifier);
            var title = $"{entity.Title} ({organization.CompanyName})";

            if (IsAchievementUploader)
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[]{
                    new BreadcrumbItem("Uploads", SearchUrl),
                    new BreadcrumbItem("Edit", null)
                }, null, title);
            }
            else
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[]{
                    new BreadcrumbItem("Organizations", "/ui/cmds/admin/organizations/search"),
                    new BreadcrumbItem("Edit", "/ui/cmds/admin/organizations/edit?id=" + OrganizationIdentifier),
                    new BreadcrumbItem("Uploads", SearchUrl),
                    new BreadcrumbItem("Edit", null)
                }, null, title);
            }

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

                var competencyIDControl = (System.Web.UI.WebControls.Literal)item.FindControl("CompetencyStandardIdentifier");
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
                {
                    if (UploadRepository.AttachCompetency(UploadID.Value, competency.StandardIdentifier))
                    {
                        count++;
                        insertedCompetencies.Add(competency.StandardIdentifier);
                    }
                }
            }

            AddCompetenciesToAchievements(insertedCompetencies.ToArray());

            EditorStatus.AddMessage(AlertType.Success, string.Format("{0:n0} competencies have been added to this download", count));

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

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var count = 0;
            var uploadCompetencies = UploadRepository.SelecteCompetencies(UploadID.Value);
            var standards = new List<TAchievementStandard>();

            foreach (var achievementID in achievements)
            {
                if (UploadRepository.AttachAchievement(UploadID.Value, achievementID))
                    count++;

                foreach (var competencyId in uploadCompetencies)
                    standards.Add(new TAchievementStandard { StandardIdentifier = competencyId, AchievementIdentifier = achievementID });
            }

            TAchievementStandardStore.Insert(standards);

            return count;
        }

        #endregion
    }
}
