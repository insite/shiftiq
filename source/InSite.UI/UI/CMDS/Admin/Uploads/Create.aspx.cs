using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Controls.Contacts.Companies.Files;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using Path = System.IO.Path;

namespace InSite.Cmds.Admin.Uploads.Forms
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int MaxFileSize = 20 * 1024 * 1024;

        #endregion

        #region Init & Load data

        private void InitData()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier);

            PageHelper.AutoBindHeader(this, null, organization.CompanyName);

            AchievementType.ExcludeSubType = "Module";
            AchievementType.RefreshData();

            AchievementCategorySelector.ListFilter.OrganizationIdentifier = organization.OrganizationIdentifier;
            AchievementCategorySelector.ListFilter.AchievementLabel = "NA";
            AchievementCategorySelector.RefreshData();

            AchievementSelector.Filter.AchievementType = "NA";
            AchievementSelector.Filter.OrganizationIdentifier = OrganizationIdentifier;
            AchievementSelector.Value = null;

            SetFileType();
        }

        #endregion

        #region Properties

        private Guid OrganizationIdentifier => CurrentIdentityFactory.ActiveOrganizationIdentifier;

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadType.AutoPostBack = true;
            UploadType.ValueChanged += UploadType_ValueChanged;

            FileUpload.MaxFileSize = MaxFileSize;

            AchievementCategorySelector.AutoPostBack = true;
            AchievementCategorySelector.ValueChanged += AchievementCategorySelector_ValueChanged;

            AchievementType.AutoPostBack = true;
            AchievementType.ValueChanged += AchievementType_ValueChanged;

            SubmitButton.Click += SubmitButton_Click;

            GlobalAchievementExporter.AchievementCopied += GlobalAchievementExporter_ResourceCopied;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                InitData();
            }

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(GlobalAchievementExporter);
        }

        #endregion

        #region Event handlers

        private void UploadType_ValueChanged(object sender, EventArgs e)
        {
            SetFileType();
        }

        private void GlobalAchievementExporter_ResourceCopied(GlobalAchievementExporter sender,
            GlobalAchievementExporter.ExporterEventArgs args)
        {
            if (args.Success)
            {
                AchievementSelector.Value = null;
                AchievementText.Text = null;

                if (args.HasDownloads)
                    EditorStatus.AddMessage(AlertType.Success,
                        string.Format("Global achievement '{0}' has been successfully copied.",
                            args.AchievementTitle));
                else
                    EditorStatus.AddMessage(AlertType.Information,
                        string.Format(
                            "Global achievement '{0}' has been copied, so it is now a organization-specific achievement, but no files or links have been uploaded for it.",
                            args.AchievementTitle));
            }
            else
            {
                EditorStatus.AddMessage(AlertType.Error,
                    string.Format("Organization-specific achievement '{0}' already exists.", args.AchievementTitle));
            }
        }

        private void AchievementCategorySelector_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var categoryId = AchievementCategorySelector.ValueAsGuid;

            AchievementSelector.Filter.CategoryIdentifier = categoryId;
            AchievementSelector.Value = null;
            AchievementText.Text = null;

            GlobalAchievementExporter.CategoryIdentifier = categoryId;
        }

        private void AchievementType_ValueChanged(object sender, EventArgs e)
        {
            AchievementCategorySelector.ListFilter.AchievementLabel = AchievementType.Value.IfNullOrEmpty("NA");
            AchievementCategorySelector.RefreshData();
            AchievementCategorySelectorView.IsActive = true;
            AchievementCategoryText.Text = null;

            AchievementSelector.Filter.AchievementType = AchievementType.Value.IfNullOrEmpty("NA");
            AchievementSelector.Filter.CategoryIdentifier = null;
            AchievementSelector.Value = null;
            AchievementText.Text = null;

            GlobalAchievementExporter.OrganizationIdentifier = OrganizationIdentifier;
            GlobalAchievementExporter.CategoryIdentifier = AchievementCategorySelector.ValueAsGuid;

            GlobalAchievementExporter.LoadData(AchievementType.Value, "disableUploadAchievement", "enableUploadAchievement");
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var uploadID = UploadType.Value == "File" ? SaveFile() : SaveLink();

            if (uploadID.HasValue)
                HttpResponseHelper.Redirect($"/ui/cmds/admin/uploads/edit?id={uploadID}");
        }

        private void Competencies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var isIndirectAssignment = (bool)row["IsIndirectAssignment"];

            var numberFormat = isIndirectAssignment
                ? "<span class='indirect-competency-assignment'>{0}</span>"
                : "<span class='direct-competency-assignment'>{0}</span>";

            var competencyLabel = (ITextControl)e.Item.FindControl("Competency");
            competencyLabel.Text = string.Format(numberFormat, row["Number"]);
        }

        #endregion

        #region Save

        private Guid? SaveLink()
        {
            var uploadEntity = UploadSearch.Select(OrganizationIdentifier, NavigationUrl.Text)
                ?? UploadStore.InsertLink(new Upload
                {
                    OrganizationIdentifier = OrganizationIdentifier,
                    ContainerIdentifier = OrganizationIdentifier,
                    ContainerType = UploadContainerType.Oganization,
                    UploadPrivacyScope = "Platform",
                    Name = NavigationUrl.Text,
                    Title = UrlTitle.Text,

                    Uploader = User.UserIdentifier,
                    Uploaded = DateTimeOffset.UtcNow
                });

            var categoryIdentifier = GetCategoryIdentifier(AchievementType.Value);
            var achievementId = GetAchievementID(OrganizationIdentifier, AchievementType.Value, categoryIdentifier);

            Save(uploadEntity.UploadIdentifier, achievementId, Competency.GetValues(), AchievementOption.SelectedValue);

            return uploadEntity.UploadIdentifier;
        }

        private Guid? SaveFile()
        {
            var metadata = FileUpload.Metadata;
            if (metadata == null)
                return null;

            var file = new FileInfo(metadata.FilePath);
            if (file.Length > MaxFileSize)
            {
                EditorStatus.AddMessage(AlertType.Error,
                    "You cannot upload a file larger than 20MB. Please reduce the size of your file before you upload it. If the size of your file cannot be reduced then please contact your administrator.");
                return null;
            }

            var fileName = CmdsUploadProvider.AdjustFileName(metadata.FileName);

            if (!OverwriteFile.Checked && CmdsUploadProvider.Current.GetInfo(OrganizationIdentifier, fileName) != null)
            {
                EditorStatus.AddMessage(
                    AlertType.Error,
                    string.Format("This file already exists: {0}", Path.GetFileName(fileName)));

                return null;
            }

            var uploadModel = CmdsUploadProvider.Current.Update(UploadContainerType.Oganization, OrganizationIdentifier, fileName, model =>
            {
                model.Title = metadata.FileName;

                using (var stream = new FileStream(metadata.FilePath, FileMode.Open))
                    model.Write(stream);
            });

            var categoryIdentifier = GetCategoryIdentifier(AchievementType.Value);
            var achievementID = GetAchievementID(OrganizationIdentifier, AchievementType.Value, categoryIdentifier);

            Save(uploadModel.UploadID, achievementID, Competency.GetValues(), AchievementOption.SelectedValue);

            return uploadModel.UploadID;
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
                {
                    achievementCompetencies.Add(new TAchievementStandard
                    {
                        StandardIdentifier = competencyId,
                        AchievementIdentifier = achievement
                    });
                }
            }

            TAchievementStandardStore.Insert(achievementCompetencies);
        }

        private Guid? GetCategoryIdentifier(string subType)
        {
            if (AchievementCategorySelectorView.IsActive)
                return AchievementCategorySelector.ValueAsGuid;

            var category = new VAchievementCategory
            {
                OrganizationIdentifier = OrganizationSearch.Select(OrganizationIdentifier).OrganizationIdentifier,
                CategoryIdentifier = UniqueIdentifier.Create(),
                AchievementLabel = subType,
                CategoryName = AchievementCategoryText.Text
            };

            TAchievementCategoryStore.Insert(category);

            return category.CategoryIdentifier;
        }

        private Guid GetAchievementID(Guid organizationId, string subType, Guid? categoryIdentifier)
        {
            Guid achievementID;

            if (AchievementSelectorView.IsActive)
            {
                achievementID = AchievementSelector.Value.Value;
            }
            else
            {
                var text = AchievementText.Text;
                var list = VCmdsAchievementSearch.Select(x => x.AchievementTitle == text && x.AchievementLabel == subType);

                if (list.Count > 0)
                {
                    achievementID = list[0].AchievementIdentifier;
                }
                else
                {
                    achievementID = UniqueIdentifier.Create();

                    ServiceLocator.SendCommand(new CreateAchievement(achievementID, Organization.Identifier, subType, text, null, null));
                }
            }

            if (categoryIdentifier.HasValue)
            {
                if (!TAchievementClassificationSearch.Exists(x => x.AchievementIdentifier == achievementID && x.CategoryIdentifier == categoryIdentifier))
                    TAchievementClassificationStore.Insert(new VAchievementClassification
                    {
                        AchievementIdentifier = achievementID,
                        CategoryIdentifier = categoryIdentifier.Value
                    });
            }

            if (!TAchievementOrganizationSearch.Exists(x => x.OrganizationIdentifier == organizationId && x.AchievementIdentifier == achievementID))
                TAchievementOrganizationStore.InsertOrganizationAchievement(organizationId, achievementID);

            return achievementID;
        }

        private void SetFileType()
        {
            var isFile = UploadType.Value == "File";

            FileUploadField.Visible = isFile;
            UrlField.Visible = !isFile;
            UrlTitleField.Visible = !isFile;
            OverwriteFileField.Visible = isFile;

            AchievementOption.Items.FindByValue("AchievementOnly").Text = isFile
                ? "Attach this file to the achievement only"
                : "Attach this URL to the achievement only";
            AchievementOption.Items.FindByValue("Both").Text = isFile
                ? "Attach this file to the achievement and to the competency"
                : "Attach this URL to the achievement and to the competency";
        }

        #endregion
    }
}
