using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contents.Read;
using InSite.Application.Files.Read;
using InSite.Application.Records.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class PublicationSetup : System.Web.UI.UserControl
    {
        #region Properties

        private Guid ProgramIdentifier
        {
            get => (Guid)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        public bool IsPublished
        {
            get => (bool)ViewState[nameof(IsPublished)];
            set => ViewState[nameof(IsPublished)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WebSiteIdentifier.AutoPostBack = true;
            WebSiteIdentifier.ValueChanged += WebSiteIdentifier_ValueChanged;

            WebFolderIdentifier.AutoPostBack = true;
            WebFolderIdentifier.ValueChanged += WebFolderIdentifier_ValueChanged;

            WebPageIdentifier.AutoPostBack = true;
            WebPageIdentifier.ValueChanged += WebPageIdentifier_ValueChanged;
            WebPageIdentifierAdd.Click += WebPageIdentifierAdd_Click;

            UpdatePanel.Request += UpdatePanel_Request;

            ReorderButton.Click += ReorderButton_Click;
            SaveButton.Click += SaveButton_Click;
            DeleteImage.Click += DeleteImage_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            CommandButtons.Visible = true;

            ReorderButton.OnClientClick = $"inSite.common.gridReorderHelper.startReorder('{UniqueID}'); return false;";

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdatePanel.ChildrenAsTriggers = true;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(PublicationSetup),
                $"register_reorder_{UniqueID}",
                $@"
                inSite.common.gridReorderHelper.registerReorder({{
                    id:'{UniqueID}',
                    selector:'#{SectionControl.ClientID} div.res-cont-list > table:first',
                    items:'tbody > tr',
                    updatePanelId:'{UpdatePanel.ClientID}'
                }});", true);

            base.OnPreRender(e);
        }

        #endregion

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var program = ProgramSearch.GetProgram(ProgramIdentifier);
            if(program == null) 
                return; 

            program.ProgramSlug = ProgramSlug.Text;
            program.ProgramIcon = ProgramIcon.Text;

            SaveV2Image(program);

            if (WebPageIdentifier.ValueAsGuid.HasValue)
                SavePage(WebPageIdentifier.ValueAsGuid.Value, program);

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);

            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "tab", "publication"));
        }

        private void SaveV2Image(TProgram program)
        {
            var oldUrl = program.ProgramImage;

            var newUrl = ProgramImageUploadV2.AdjustImageSaveAndGetUrl(
                program.ProgramIdentifier,
                FileObjectType.Program,
                300,
                200);

            if (string.IsNullOrWhiteSpace(newUrl))
                return;

            if (!string.IsNullOrWhiteSpace(oldUrl) && !string.Equals(oldUrl, newUrl, StringComparison.OrdinalIgnoreCase))
                FileUploadV2.DeleteFileByUrl(oldUrl);

            program.ProgramImage = newUrl;
        }

        public void LoadData(TProgram program)
        {
            ProgramIdentifier = program.ProgramIdentifier;

            BindModelToControls(program);
            BindRepeater();
        }

        public void Refresh()
        {
            BindRepeater();
        }

        public void BindModelToControls(TProgram program)
        {
            var page = ServiceLocator.PageSearch.BindFirst(x => x, x => x.ObjectType == "Program" && x.ObjectIdentifier == program.ProgramIdentifier);

            WebPagePanel.Visible = page != null;
            WebPageIdentifier.ValueAsGuid = page?.PageIdentifier;
            OnWebPageSelected(page?.PageIdentifier);

            ProgramSlug.Text = program.ProgramSlug ?? StringHelper.Sanitize(program.ProgramName, '-');
            ProgramIcon.Text = program.ProgramIcon;
            ProgramIconPreview.Visible = !string.IsNullOrEmpty(program.ProgramIcon);
            ProgramIconPreview.InnerHtml = $"<i class='{program.ProgramIcon}'></i>";

            ProgramImage.ImageUrl = program.ProgramImage;
            ProgramImageField.Visible = !string.IsNullOrEmpty(program.ProgramImage);
            ProgramImageUrl.Text = program.ProgramImage;
        }

        private void DeleteImage_Click(object sender, EventArgs e)
        {
            var program = ProgramSearch.GetProgram(ProgramIdentifier);

            if (program == null)
                return;

            if (program.ProgramImage == null)
                return;

            var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(program.ProgramImage);
            if (fileIdentifier != null)
                ServiceLocator.StorageService.Delete(fileIdentifier.Value);

            program.ProgramImage = null;

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);
            Response.StatusCode = (int)HttpStatusCode.OK;
            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "tab", "publication"));
        }

        private void SavePage(Guid pageId, TProgram program)
        {
            var page = ServiceLocator.PageSearch.Select(pageId);
            page.ObjectType = "Program";
            page.ObjectIdentifier = program.ProgramIdentifier;
            page.ContentControl = "Program";
            page.IsHidden = !PublicationStatus.Checked;
            page.PageIcon = program.ProgramIcon;
            page.PageSlug = program.ProgramSlug;

            if (program.ProgramImage != null)
            {
                if (page.ContentLabels == null)
                    page.ContentLabels = "ImageURL";
                else if (!page.ContentLabels.Contains("ImageURL"))
                    page.ContentLabels += ", ImageURL";

                var pageContent = ServiceLocator.ContentSearch.SelectContainer(page.PageIdentifier, "ImageURL", "en")
                    ?? new TContent
                    {
                        ContentIdentifier = UniqueIdentifier.Create(),
                        ContainerType = "Web Page",
                        ContainerIdentifier = page.PageIdentifier,
                        ContentLabel = "ImageURL",
                        ContentLanguage = "en",
                        OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier
                    };

                pageContent.ContentText = program.ProgramImage;
                ServiceLocator.ContentStore.Save(pageContent);
            }

            var original = ServiceLocator.PageSearch.Select(pageId);

            var commands = new PageCommandGenerator().GetDifferencePageSetupCommands(original, page);

            ServiceLocator.SendCommands(commands);
        }

        #region Publication

        private void WebSiteIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebSiteSelected();

        private void OnWebSiteSelected()
        {
            WebFolderIdentifier.ClearSelection();
            WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

            if (WebSiteIdentifier.ValueAsGuid.HasValue)
                WebFolderIdentifier.ListFilter.SiteIdentifier = WebSiteIdentifier.ValueAsGuid;

            WebFolderIdentifier.RefreshData();
            OnWebFolderSelected();
        }

        private void WebFolderIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebFolderSelected();

        private void OnWebFolderSelected()
        {
            WebPageIdentifier.ClearSelection();
            WebPageIdentifier.ListFilter.FolderIdentifier = Guid.Empty;

            WebPagePanel.Visible = WebFolderIdentifier.ValueAsGuid.HasValue;
            if (WebPagePanel.Visible)
                WebPageIdentifier.ListFilter.FolderIdentifier = WebFolderIdentifier.ValueAsGuid;

            WebPageIdentifier.RefreshData();
            OnWebPageSelected(WebPageIdentifier.ValueAsGuid);
        }

        private void WebPageIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebPageSelected(WebPageIdentifier.ValueAsGuid);

        private void OnWebPageSelected(Guid? id)
        {
            IsPublished = id.HasValue;
            if (IsPublished)
            {
                var page = SitemapSearch.Get(id.Value);

                if (page != null)
                {
                    WebPageIdentifier.ListFilter.FolderIdentifier = page.FolderIdentifier;
                    WebFolderIdentifier.ListFilter.SiteIdentifier = page.SiteIdentifier;
                    WebFolderIdentifier.ValueAsGuid = page.FolderIdentifier;
                    WebSiteIdentifier.ValueAsGuid = page.SiteIdentifier;

                    PublicationStatus.Checked = !page.PageIsHidden;

                    WebPageIdentifierEdit.HRef = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}";
                    WebPageIdentifierView.HRef = ServiceLocator.PageSearch.GetPagePath(page.PageIdentifier, false);
                    WebPageHelp.InnerHtml = $"{WebPageIdentifierView.HRef}";

                    WebPageIdentifierAdd.Visible = false;
                    WebPageIdentifierLinks.Visible = true;
                }
            }
            else
            {
                WebPageIdentifier.ListFilter.FolderIdentifier = Guid.Empty;
                WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

                WebPageIdentifierAdd.Visible = true;
                WebPageIdentifierLinks.Visible = false;
            }
        }

        private void WebPageIdentifierAdd_Click(object sender, EventArgs e)
        {
            var program = ProgramSearch.GetProgram(ProgramIdentifier);
            var page = new QPage
            {
                ContentControl = "Program",
                ObjectType = "Program",
                ObjectIdentifier = program.ProgramIdentifier,
                Title = program.ProgramName,
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                ParentPageIdentifier = WebFolderIdentifier.ValueAsGuid.Value,
                PageSlug = program.ProgramSlug.HasValue() ? program.ProgramSlug : ProgramSlug.Text,
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = "Page",
                SiteIdentifier = WebSiteIdentifier.ValueAsGuid.Value,
                AuthorDate = DateTimeOffset.Now,
                AuthorName = CurrentSessionState.Identity.User.FullName
            };

            var commands = new PageCommandGenerator().GetCommands(page);

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            WebPageIdentifier.ListFilter.FolderIdentifier = page.ParentPageIdentifier;
            WebPageIdentifier.RefreshData();
            WebPageIdentifier.Value = page.PageIdentifier.ToString();
            OnWebPageSelected(page.PageIdentifier);
        }

        #endregion

        #region Task Ordering

        private void BindRepeater()
        {
            var items = GetTaskItems();
            var hasItems = items.Count > 0;

            NoItemsMessage.Visible = !hasItems;
            CommandButtons.Visible = hasItems;

            DataRepeater.Visible = hasItems;

            DataRepeater.DataSource = items;
            DataRepeater.DataBind();
        }

        private void Reorder(TaskInfo[] toReorder)
        {
            int sequence = 0;

            foreach (var c in toReorder)
            {
                c.Sequence = ++sequence;
                TaskStore.UpdateTaskSequence(c.TaskIdentifier.Value, sequence);
            }

        }

        private List<TaskInfo> GetTaskItems()
        {
            var Organization = CurrentSessionState.Identity.Organization;
            var taskObjectData = ProgramHelper.GetTaskObjectData(Organization.OrganizationIdentifier, Organization.ParentOrganizationIdentifier);

            var tasks = ProgramSearch1.GetProgramTasks(new TTaskFilter
            {
                ProgramIdentifier = ProgramIdentifier,
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ExcludeObjectTypes = new string[] { "Assessment" }
            });

            var taskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            return taskInfoContainer.OrderBy(x => x.Sequence).ToList();
        }

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            OnReorderCommand(Request.Form["__EVENTARGUMENT"]);
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            OnReorderCommand(e.Value);
        }

        public void OnReorderCommand(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return;

            var args = argument.Split('&');
            var command = args[0];
            var value = args.Length > 1 ? args[1] : null;

            if (command == "save-reorder")
            {
                var changes = JavaScriptHelper.GridReorder.Parse(value);
                var items = GetTaskItems();
                var reorder = new TaskInfo[items.Count];

                for (int i = 0; i < items.Count; i++)
                    reorder[i] = items[i];

                foreach (var c in changes)
                    reorder[c.Destination.ItemIndex] = items[c.Source.ItemIndex];

                Reorder(reorder);

                BindRepeater();
            }
        }

        #endregion

        #region Helper Functions

        protected string GetDisplayTextType(string type)
            => !string.IsNullOrEmpty(type) ? Shift.Common.Humanizer.TitleCase(type) : null;

        #endregion
    }
}