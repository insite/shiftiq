using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Cmds.Actions.Contact.Company.Achievement
{
    public partial class Upload : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int MaxFileSize = 20 * 1024 * 1024;

        #endregion

        #region Properties

        private Guid OrganizationIdentifier => CurrentIdentityFactory.ActiveOrganizationIdentifier;

        private Guid? CompanyThumbprint
        {
            get => (Guid?)ViewState[nameof(CompanyThumbprint)];
            set => ViewState[nameof(CompanyThumbprint)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FileUpload.MaxFileSize = MaxFileSize;

            // URL Links
            HomeButton1.NavigateUrl = Navigator.GetHomeUrl(Identity);
            HomeButton2.NavigateUrl = Navigator.GetHomeUrl(Identity);
            UrlAchievementCategorySelector.AutoPostBack = true;
            UrlAchievementCategorySelector.ValueChanged += UrlAchievementCategorySelector_ValueChanged;

            UrlAchievementType.AutoPostBack = true;
            UrlAchievementType.ValueChanged += UrlAchievementType_ValueChanged;

            SubmitLinkButton.Click += SubmitLinkButton_Click;

            UrlDownloads.DataBinding += UrlDownloads_DataBinding;
            UrlDownloads.RowCommand += UrlDownloads_RowCommand;
            UrlDownloads.RowDataBound += UrlDownloads_RowDataBound;

            // Files
            FileAchievementCategorySelector.AutoPostBack = true;
            FileAchievementCategorySelector.ValueChanged += FileAchievementCategorySelector_ValueChanged;

            FileAchievementType.AutoPostBack = true;
            FileAchievementType.ValueChanged += FileAchievementType_ValueChanged;

            SubmitFileButton.Click += SubmitFileButton_Click;

            FileDownloads.DataBinding += FileDownloads_DataBinding;
            FileDownloads.RowCommand += UrlDownloads_RowCommand;
            FileDownloads.RowDataBound += UrlDownloads_RowDataBound;

            CompanyAchievementProblems.Fixed += AchievementIssues_Fixed;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                InitData();

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SubmitFileButton);
        }

        #endregion

        #region Event handlers

        private void UrlDownloads_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                var grid = (Grid)sender;
                var uploadId = grid.GetDataKey<Guid>(e);

                CompanyAchievementHelper.Delete(uploadId, OrganizationIdentifier, User);

                EditorStatus.AddMessage(AlertType.Success, "Download has been deleted.");

                LoadData();
            }
        }

        private void UrlDownloads_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;
            var uploadId = (Guid)row["UploadIdentifier"];

            var achievementsRepeater = (Repeater)e.Row.FindControl("Achievements");
            var competenciesRepeater = (Repeater)e.Row.FindControl("Competencies");

            achievementsRepeater.DataSource = UploadRepository.SelectUploadAchievements(uploadId);
            achievementsRepeater.DataBind();

            competenciesRepeater.ItemDataBound += Competencies_ItemDataBound;
            competenciesRepeater.DataSource = UploadRepository.SelectUploadCompetencies(uploadId);
            competenciesRepeater.DataBind();
        }

        private void UrlAchievementCategorySelector_ValueChanged(object sender, EventArgs e)
        {
            UrlAchievementSelector.Filter.CategoryIdentifier = UrlAchievementCategorySelector.ValueAsGuid;
            UrlAchievementSelector.Value = null;
            UrlAchievementText.Text = null;
        }

        private void UrlAchievementType_ValueChanged(object sender, EventArgs e)
        {
            UrlAchievementCategorySelector.ListFilter.AchievementLabel = UrlAchievementType.Value.IfNullOrEmpty("NA");
            UrlAchievementCategorySelector.RefreshData();
            UrlAchievementCategorySelectorView.IsActive = true;
            UrlAchievementCategoryText.Text = null;

            UrlAchievementSelector.Filter.AchievementType = UrlAchievementType.Value.IfNullOrEmpty("NA");
            UrlAchievementSelector.Filter.CategoryIdentifier = null;
            UrlAchievementSelector.Value = null;
            UrlAchievementSelectorView.IsActive = true;
            UrlAchievementText.Text = null;
        }

        private void SubmitLinkButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var url = NavigationUrl.Text;

            if (SaveLink())
            {
                LoadData();

                EditorStatus.AddMessage(AlertType.Success, $"Link '{url}' has been successfully uploaded.");
            }
        }

        private void FileAchievementCategorySelector_ValueChanged(object sender, EventArgs e)
        {
            FileAchievementSelector.Filter.CategoryIdentifier = FileAchievementCategorySelector.ValueAsGuid;
            FileAchievementSelector.Value = null;
            FileAchievementText.Text = null;
        }

        private void FileAchievementType_ValueChanged(object sender, EventArgs e)
        {
            FileAchievementCategorySelector.ListFilter.AchievementLabel = FileAchievementType.Value.IfNullOrEmpty("NA");
            FileAchievementCategorySelector.RefreshData();
            FileAchievementCategorySelectorView.IsActive = true;
            FileAchievementCategoryText.Text = null;

            FileAchievementSelector.Filter.AchievementType = FileAchievementType.Value.IfNullOrEmpty("NA");
            FileAchievementSelector.Filter.CategoryIdentifier = null;
            FileAchievementSelector.Value = null;
            FileAchievementSelectorView.IsActive = true;
            FileAchievementText.Text = null;
        }

        private void SubmitFileButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!FileUpload.HasFile)
                return;

            if (FileUpload.FileSize > MaxFileSize)
            {
                EditorStatus.AddMessage(AlertType.Error, "You cannot upload a file larger than 20MB. Please reduce the size of your file before you upload it. If the size of your file cannot be reduced then please contact your administrator.");
                return;
            }

            if (SaveFile())
            {
                LoadData();

                EditorStatus.AddMessage(AlertType.Success, $"File '{FileUpload.Metadata.FileName}' has been successfully uploaded.");
            }
        }

        private void UrlDownloads_DataBinding(object sender, EventArgs e)
        {
            UrlDownloads.DataSource =
                UploadRepository.SelectAllCompanyUploads(
                    OrganizationIdentifier, UploadType.Link, UrlDownloads.PageIndex, UrlDownloads.PageSize);
        }

        private void FileDownloads_DataBinding(object sender, EventArgs e)
        {
            FileDownloads.DataSource =
                UploadRepository.SelectAllCompanyUploads(
                    OrganizationIdentifier, UploadType.CmdsFile, FileDownloads.PageIndex, FileDownloads.PageSize);
        }

        private void Competencies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var isIndirectAssignment = (bool)row["IsIndirectAssignment"];

            var competencyLabel = (ITextControl)e.Item.FindControl("Competency");
            competencyLabel.Text = isIndirectAssignment
                ? $"<span class='indirect-competency-assignment'>{row["Number"]}</span>"
                : $"<span class='direct-competency-assignment'>{row["Number"]}</span>";
        }

        private void AchievementIssues_Fixed(object sender, EventArgs e)
        {
            EditorStatus.AddMessage(AlertType.Success, "Potential data issues have been fixed.");

            CompanyAchievementProblems.Visible = CompanyAchievementProblems.LoadData(CompanyThumbprint.Value);
        }

        #endregion

        #region Init & Load data

        private void InitData()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier);

            CompanyThumbprint = organization.OrganizationIdentifier;

            FileAchievementType.ExcludeSubType = "Module,Orientation";
            FileAchievementType.RefreshData();

            UrlAchievementType.ExcludeSubType = "Module,Orientation";
            UrlAchievementType.RefreshData();

            UrlAchievementCategorySelector.ListFilter.OrganizationIdentifier = organization.OrganizationIdentifier;
            UrlAchievementCategorySelector.ListFilter.AchievementLabel = "NA";
            UrlAchievementCategorySelector.RefreshData();

            UrlAchievementSelector.Filter.AchievementType = "NA";
            UrlAchievementSelector.Filter.OrganizationIdentifier = OrganizationIdentifier;
            UrlAchievementSelector.Value = null;

            FileAchievementCategorySelector.ListFilter.OrganizationIdentifier = organization.OrganizationIdentifier;
            FileAchievementCategorySelector.ListFilter.AchievementLabel = "NA";
            FileAchievementCategorySelector.RefreshData();

            FileAchievementSelector.Filter.AchievementType = "NA";
            FileAchievementSelector.Filter.OrganizationIdentifier = OrganizationIdentifier;
            FileAchievementSelector.Value = null;

            LoadData();
        }

        private void LoadData()
        {
            var urlCount = UploadRepository.CountAllCompanyUploads(OrganizationIdentifier, UploadType.Link);

            NoUploadedUrlsPanel.Visible = urlCount == 0;
            UploadedUrlsPanel.Visible = urlCount > 0;

            UrlDownloads.VirtualItemCount = urlCount;
            UrlDownloads.DataBind();

            var fileCount = UploadRepository.CountAllCompanyUploads(OrganizationIdentifier, UploadType.CmdsFile);

            NoUploadedFilesPanel.Visible = fileCount == 0;
            UploadedFilesPanel.Visible = fileCount > 0;

            FileDownloads.VirtualItemCount = fileCount;
            FileDownloads.DataBind();

            CompanyAchievementProblems.Visible = CompanyAchievementProblems.LoadData(CompanyThumbprint.Value);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PageHelper.AutoBindHeader(this, null, $"Upload Files and Links ({Organization.CompanyName})");
        }

        #endregion

        #region Save

        private bool SaveLink()
        {
            var uploadEntity = UploadSearch.Select(CompanyThumbprint.Value, NavigationUrl.Text)
                ?? UploadStore.InsertLink(new Persistence.Upload
                {
                    OrganizationIdentifier = CompanyThumbprint.Value,
                    ContainerIdentifier = CompanyThumbprint.Value,
                    ContainerType = UploadContainerType.Oganization,
                    UploadPrivacyScope = "Platform",
                    Name = NavigationUrl.Text,
                    Title = UrlTitle.Text,
                    Uploader = User.UserIdentifier,
                    Uploaded = DateTimeOffset.Now
                });

            var categoryId = UrlAchievementCategorySelectorView.IsActive
                ? UrlAchievementCategorySelector.ValueAsGuid
                : CreateAchievementCategory(UrlAchievementCategoryText.Text, UrlAchievementType.Value);
            var achievementId = UrlAchievementSelectorView.IsActive
                ? UrlAchievementSelector.Value.Value
                : GetAchievement(UrlAchievementText.Text, UrlAchievementType.Value);

            EnsureAchievementRelations(OrganizationIdentifier, achievementId, categoryId);

            Save(uploadEntity.UploadIdentifier, achievementId, UrlCompetency.GetValues(), UrlOption.SelectedValue);

            NavigationUrl.Text = null;
            UrlTitle.Text = null;
            UrlAchievementType.ClearSelection();
            UrlCompetency.ClearValues();

            UrlAchievementCategorySelector.ValueAsGuid = null;
            UrlAchievementCategorySelectorView.IsActive = true;
            UrlAchievementCategoryText.Text = null;

            UrlAchievementText.Text = null;
            UrlAchievementSelector.Filter.AchievementType = "NA";
            UrlAchievementSelectorView.IsActive = true;
            UrlAchievementSelector.Value = null;

            return true;
        }

        private bool SaveFile()
        {
            var metadata = FileUpload.Metadata;
            var fileName = CmdsUploadProvider.AdjustFileName(metadata.FileName);

            if (!OverwriteFile.Checked && CmdsUploadProvider.Current.GetInfo(CompanyThumbprint.Value, fileName) != null)
            {
                EditorStatus.AddMessage(
                    AlertType.Error,
                    $"This file already exists: {Path.GetFileName(fileName)}");

                return false;
            }

            var uploadModel = CmdsUploadProvider.Current.Update(UploadContainerType.Oganization, CompanyThumbprint.Value, fileName, model =>
            {
                model.Title = metadata.FileName;

                using (var stream = new FileStream(metadata.FilePath, FileMode.Open))
                    model.Write(stream);
            });

            var categoryId = FileAchievementCategorySelectorView.IsActive
                ? FileAchievementCategorySelector.ValueAsGuid
                : CreateAchievementCategory(FileAchievementCategoryText.Text, FileAchievementType.Value);
            var achievementId = FileAchievementSelectorView.IsActive
                ? FileAchievementSelector.Value.Value
                : GetAchievement(FileAchievementText.Text, FileAchievementType.Value);

            EnsureAchievementRelations(OrganizationIdentifier, achievementId, categoryId);

            Save(uploadModel.UploadID, achievementId, FileCompetency.GetValues(), FileOption.SelectedValue);

            FileAchievementType.ClearSelection();
            FileCompetency.ClearValues();

            FileAchievementCategorySelector.ValueAsGuid = null;
            FileAchievementCategorySelectorView.IsActive = true;
            FileAchievementCategoryText.Text = null;

            FileAchievementText.Text = null;
            FileAchievementSelector.Filter.AchievementType = "NA";
            FileAchievementSelector.Value = null;

            return true;
        }

        private void Save(Guid uploadId, Guid achievement, Guid[] competencies, string attachmentOption)
        {
            UploadRepository.AttachAchievement(uploadId, achievement);

            if (competencies != null)
            {
                var competencyUploadId = attachmentOption == "Both" ? uploadId : (Guid?)null;

                SaveCompetencies(competencyUploadId, achievement, competencies);
            }
        }

        private void SaveCompetencies(Guid? uploadId, Guid achievement, Guid[] competencies)
        {
            var achievementCompetencies = new List<TAchievementStandard>();

            foreach (var competencyId in competencies)
            {
                if (uploadId.HasValue)
                    UploadRepository.AttachCompetency(uploadId.Value, competencyId);

                if (!TAchievementStandardSearch.Exists(x => x.StandardIdentifier == competencyId && x.AchievementIdentifier == achievement))
                    achievementCompetencies.Add(new TAchievementStandard { StandardIdentifier = competencyId, AchievementIdentifier = achievement });
            }

            TAchievementStandardStore.Insert(achievementCompetencies);
        }

        private Guid? CreateAchievementCategory(string name, string subType)
        {
            if (name.IsEmpty())
                return null;

            var categoryId = UniqueIdentifier.Create();

            TAchievementCategoryStore.Insert(new VAchievementCategory
            {
                CategoryIdentifier = categoryId,
                OrganizationIdentifier = OrganizationIdentifier,
                AchievementLabel = subType,
                CategoryName = name
            });

            return categoryId;
        }

        private Guid GetAchievement(string title, string subType)
        {
            var list = VCmdsAchievementSearch.Select(x => x.AchievementTitle == title && x.AchievementLabel == subType);

            if (list.Count > 0)
                return list[0].AchievementIdentifier;

            var achievementId = UniqueIdentifier.Create();

            ServiceLocator.SendCommand(new CreateAchievement(achievementId, Organization.Identifier, subType, title, null, null));

            return achievementId;
        }

        private void EnsureAchievementRelations(Guid organizationId, Guid achievementId, Guid? categoryIdentifier)
        {
            if (categoryIdentifier.HasValue)
            {
                if (!TAchievementClassificationSearch.Exists(x => x.AchievementIdentifier == achievementId && x.CategoryIdentifier == categoryIdentifier))
                    TAchievementClassificationStore.Insert(new VAchievementClassification { AchievementIdentifier = achievementId, CategoryIdentifier = categoryIdentifier.Value });
            }

            if (!TAchievementOrganizationSearch.Exists(x => x.OrganizationIdentifier == organizationId && x.AchievementIdentifier == achievementId))
                TAchievementOrganizationStore.InsertOrganizationAchievement(organizationId, achievementId);
        }

        #endregion

        #region Helper methods

        protected static string GetFileUrl(Guid guid, string name)
        {
            return CmdsUploadProvider.GetFileRelativeUrl(guid, name);
        }

        #endregion
    }
}
