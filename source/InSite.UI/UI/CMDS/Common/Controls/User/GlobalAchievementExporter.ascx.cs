using System;
using System.Web.UI;

using InSite.Application.Achievements.Write;
using InSite.Cmds.Infrastructure;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Contacts.Companies.Files
{
    public partial class GlobalAchievementExporter : UserControl
    {
        #region Delegates

        public delegate void AchievementCopiedHandler(GlobalAchievementExporter sender, ExporterEventArgs args);

        #endregion

        #region Public methods

        public void LoadData(string subType, string disableFunctionName, string enableFunctionName)
        {
            if (string.IsNullOrEmpty(subType))
            {
                AchievementPanel.Visible = false;
                return;
            }

            DisableFunctionName = disableFunctionName;
            EnableFunctionName = enableFunctionName;
            SubType = subType;

            var organization = CurrentSessionState.Identity.Organization.Code;
            AchievementTypeName1.Text = Shift.Common.AchievementTypes.Pluralize(subType, organization);
            AchievementTypeName2.Text = AchievementTypeName1.Text;

            var filter = new VCmdsAchievementFilter();
            filter.AchievementVisibility = AccountScopes.Enterprise;
            filter.AchievementType = subType;

            var table = VCmdsAchievementSearch.SelectByFilter(filter);

            Achievements.Items.Clear();

            foreach (var row in table)
            {
                var item = new System.Web.UI.WebControls.ListItem(row.AchievementTitle, row.AchievementIdentifier.ToString());

                Achievements.Items.Add(item);
            }

            AchievementPanel.Visible = true;
            NoResultPanel.Visible = table.Count == 0;
            ResultPanel.Visible = table.Count > 0;
            OtherAchievementsWarning.Visible = subType == Shift.Common.AchievementTypes.OtherAchievement;
        }

        #endregion

        #region Event handlers

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Achievements.SelectedValue))
                return;

            var args = CopyGlobalAchievement();

            OnAchievementCopied(args);
        }

        #endregion

        #region ExporterEventArgs

        public class ExporterEventArgs
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public bool HasDownloads { get; set; }
            public bool Success { get; set; }
        }

        #endregion

        #region Events

        public event AchievementCopiedHandler AchievementCopied;

        private void OnAchievementCopied(ExporterEventArgs args)
        {
            if (AchievementCopied != null)
                AchievementCopied(this, args);
        }

        #endregion

        #region Properties

        public string DisableFunctionName
        {
            get => (string)ViewState[nameof(DisableFunctionName)];
            set => ViewState[nameof(DisableFunctionName)] = value;
        }

        public string EnableFunctionName
        {
            get => (string)ViewState[nameof(EnableFunctionName)];
            set => ViewState[nameof(EnableFunctionName)] = value;
        }

        public Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        public string SubType
        {
            get => (string)ViewState[nameof(SubType)];
            set => ViewState[nameof(SubType)] = value;
        }

        public Guid? CategoryIdentifier
        {
            get => ViewState[nameof(CategoryIdentifier)] as Guid?;
            set => ViewState[nameof(CategoryIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CopyButton.Click += CopyButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            CopyButton.OnClientClick = string.Format("if($get('{0}').value == '') {{ alert('Achievement is not selected.'); return false; }} return confirm('Are you sure you want to copy the selected achievement(s)?');", Achievements.ClientID);
            UnselectButton.OnClientClick = string.Format("UnselectButton_onclick(this, '{0}', '{1}'); return false;", Achievements.ClientID, EnableFunctionName);
            Achievements.Attributes["onchange"] = string.Format("Achievements_onclick(this, '{0}', '{1}')", UnselectButton.ClientID, DisableFunctionName);
        }

        #endregion

        #region Copy global Achievement

        private ExporterEventArgs CopyGlobalAchievement()
        {
            bool hasDownloads;

            var inserted = InsertCompanyAchievement(OrganizationIdentifier, out var copy);

            if (!inserted)
                return new ExporterEventArgs { AchievementIdentifier = copy.AchievementIdentifier, AchievementTitle = copy.AchievementTitle, Success = false };

            hasDownloads = CopyAchievementDownloads(copy.AchievementIdentifier);

            LoadData(SubType, DisableFunctionName, EnableFunctionName);

            return new ExporterEventArgs { AchievementIdentifier = copy.AchievementIdentifier, AchievementTitle = copy.AchievementTitle, HasDownloads = hasDownloads, Success = true };
        }

        private bool InsertCompanyAchievement(Guid organizationId, out VCmdsAchievement achievement)
        {
            var companyName = CurrentSessionState.Identity.Organization.CompanyName;

            var originalAchievementID = Guid.Parse(Achievements.SelectedValue);

            var original = VCmdsAchievementSearch.Select(originalAchievementID);

            var title = $"{original.AchievementTitle} ({companyName})";

            var existent = VCmdsAchievementSearch.SelectFirst(x => x.AchievementTitle == title && x.AchievementLabel == original.AchievementLabel && x.OrganizationIdentifier == OrganizationIdentifier);

            if (existent != null)
            {
                achievement = existent;
                return false;
            }

            var achievementIdentifier = UniqueIdentifier.Create();
            var command = new CreateAchievement(achievementIdentifier, organizationId, original.AchievementLabel, title, null, null);

            command.OriginOrganization = organizationId;

            ServiceLocator.SendCommand(command);

            achievement = VCmdsAchievementSearch.Select(achievementIdentifier);

            return true;
        }

        private bool CopyAchievementDownloads(Guid copyAchievementId)
        {
            var originalAchievementId = Guid.Parse(Achievements.SelectedValue);
            var originalUploads = UploadRepository.SelectAchievementUploads(originalAchievementId);

            if (originalUploads.Count == 0)
                return false;

            foreach (var upload in originalUploads)
            {
                if (UploadSearch.Exists(OrganizationIdentifier, upload.Name))
                    continue;

                Guid uploadId;

                if (upload.UploadType == UploadType.Link)
                {
                    var entity = UploadStore.InsertLink(new Upload
                    {
                        ContainerType = UploadContainerType.Oganization,
                        ContainerIdentifier = OrganizationIdentifier,
                        OrganizationIdentifier = OrganizationIdentifier,

                        Name = upload.Name,
                        Title = upload.Title,
                        Description = upload.Description,

                        Uploader = CurrentSessionState.Identity.User.UserIdentifier,
                        Uploaded = DateTimeOffset.UtcNow
                    });

                    uploadId = entity.UploadIdentifier;
                }
                else
                {
                    var model = CmdsUploadProvider.Current.Copy(upload.ContainerIdentifier, upload.Name, OrganizationIdentifier, upload.Name);
                    if (model == null)
                        continue;

                    uploadId = model.UploadID;
                }

                UploadRepository.AttachAchievement(uploadId, copyAchievementId);
            }

            return true;
        }

        #endregion
    }
}