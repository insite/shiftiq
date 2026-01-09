using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Contents.Read;
using InSite.Application.Files.Read;
using InSite.Application.Records.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifyPublication : AdminBasePage, IHasParentLinkParameters
    {
        #region Navigation

        public const string NavigateUrl = "/ui/admin/learning/programs/modify-publication";

        public static string GetNavigateUrl(Guid programId, string tab = null)
        {
            var url = NavigateUrl + "?id=" + programId;

            if (tab.IsNotEmpty())
                url += "&tab=" + tab;

            return url;
        }

        public static void Redirect(Guid programId, string tab = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(programId, tab));

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion

        #region Properties

        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private string ReturnUrl
        {
            get => (string)ViewState[nameof(ReturnUrl)];
            set => ViewState[nameof(ReturnUrl)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WebSiteIdentifier.AutoPostBack = true;
            WebSiteIdentifier.ValueChanged += (s, a) => OnWebSiteSelected();

            WebFolderIdentifier.AutoPostBack = true;
            WebFolderIdentifier.ValueChanged += (s, a) => OnWebFolderSelected();

            WebPageIdentifier.AutoPostBack = true;
            WebPageIdentifier.ValueChanged += (s, a) => OnWebPageSelected(WebPageIdentifier.ValueAsGuid);

            WebPageIdentifierAdd.Click += (s, a) => OnWebPageAdd();

            DeleteImage.Click += (s, a) => OnDeleteImage();

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var url = new WebUrl(Request.RawUrl);
            var tab = url.QueryString["tab"];

            ReturnUrl = Outline.GetNavigateUrl(ProgramId.Value, tab: "publication", subtab: tab);
            PublicationTab.IsSelected = tab == "publication";
            TasksTab.IsSelected = tab == "tasks";

            Open();
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, program.ProgramName);

            var page = ServiceLocator.PageSearch.BindFirst(x => x, x => x.ObjectType == "Program" && x.ObjectIdentifier == program.ProgramIdentifier);

            WebPagePanel.Visible = page != null;
            WebPageIdentifier.ValueAsGuid = page?.PageIdentifier;
            OnWebPageSelected(page?.PageIdentifier);

            ProgramSlug.Text = program.ProgramSlug ?? StringHelper.Sanitize(program.ProgramName, '-');
            ProgramIcon.Text = program.ProgramIcon;
            ProgramIconPreview.Visible = !string.IsNullOrEmpty(program.ProgramIcon);
            ProgramIconPreview.InnerHtml = $"<i class='{program.ProgramIcon}'></i>";

            SetProgramImage(program.ProgramImage);
            BindTaskRepeater(program.ProgramIdentifier);

            CancelButton.NavigateUrl = ReturnUrl;
        }

        private void SetProgramImage(string url)
        {
            ProgramImage.ImageUrl = url;
            ProgramImageField.Visible = url.IsNotEmpty();
            ProgramImageUrl.Text = url;
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                return;

            program.ProgramSlug = ProgramSlug.Text;
            program.ProgramIcon = ProgramIcon.Text;

            SaveV2Image(program);

            if (WebPageIdentifier.ValueAsGuid.HasValue)
                SavePage(WebPageIdentifier.ValueAsGuid.Value, program);

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);

            SaveTasks(program.ProgramIdentifier);

            HttpResponseHelper.Redirect(ReturnUrl);
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

        #endregion

        #region Program Publication

        private void OnWebSiteSelected()
        {
            WebFolderIdentifier.ClearSelection();
            WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

            if (WebSiteIdentifier.ValueAsGuid.HasValue)
                WebFolderIdentifier.ListFilter.SiteIdentifier = WebSiteIdentifier.ValueAsGuid;

            WebFolderIdentifier.RefreshData();
            OnWebFolderSelected();
        }

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

        private void OnWebPageSelected(Guid? id)
        {
            if (!id.HasValue)
            {
                WebPageIdentifier.ListFilter.FolderIdentifier = Guid.Empty;
                WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

                WebPageIdentifierAdd.Visible = true;
                WebPageIdentifierLinks.Visible = false;

                return;
            }

            var page = SitemapSearch.Get(id.Value);
            if (page == null)
                return;

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

        private void OnWebPageAdd()
        {
            var program = ProgramSearch.GetProgram(ProgramId.Value);
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

        private void OnDeleteImage()
        {
            var program = ProgramSearch.GetProgram(ProgramId.Value);
            if (program == null)
                return;

            if (program.ProgramImage == null)
                return;

            var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(program.ProgramImage);
            if (fileIdentifier != null)
                ServiceLocator.StorageService.Delete(fileIdentifier.Value);

            program.ProgramImage = null;

            ProgramStore.Update(program, CurrentSessionState.Identity.User.UserIdentifier);

            SetProgramImage(program.ProgramImage);
        }

        #endregion

        #region Task Ordering

        private void BindTaskRepeater(Guid programId)
        {
            var items = GetTaskItems(programId);
            var hasItems = items.Count > 0;

            NoTaskItemsMessage.Visible = !hasItems;

            TaskItemRepeater.Visible = hasItems;
            TaskItemRepeater.DataSource = items;
            TaskItemRepeater.DataBind();
        }

        private List<TaskInfo> GetTaskItems(Guid programId)
        {
            var organization = CurrentSessionState.Identity.Organization;

            var taskObjectData = ProgramHelper.GetTaskObjectData(organization.OrganizationIdentifier, organization.ParentOrganizationIdentifier);

            var filter = new TTaskFilter
            {
                ProgramIdentifier = programId,
                ExcludeObjectTypes = new string[] { "Assessment" }
            };

            filter.OrganizationIdentifiers.Add(organization.OrganizationIdentifier);

            var tasks = ProgramSearch1.GetProgramTasks(filter);

            var taskInfoContainer = ProgramHelper.BindTaskInfo(taskObjectData, tasks);

            return taskInfoContainer.OrderBy(x => x.Sequence).ToList();
        }

        private void SaveTasks(Guid programId)
        {
            if (TasksValues.Value.IsEmpty())
                return;

            var ids = new List<Guid>();
            foreach (var value in TasksValues.Value.Split(';'))
            {
                if (Guid.TryParse(value, out var id))
                    ids.Add(id);
                else
                    return;
            }

            var tasks = GetTaskItems(programId);
            if (ids.Count != tasks.Count)
                return;

            var tasksOrdered = new TaskInfo[ids.Count];
            for (var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                var task = tasks.FirstOrDefault(x => x.TaskIdentifier.Value == id);
                if (task == null)
                    return;

                tasksOrdered[i] = task;
            }

            for (var i = 0; i < tasksOrdered.Length; i++)
            {
                var task = tasksOrdered[i];
                var sequence = i + 1;

                if (task.Sequence != sequence)
                    TaskStore.UpdateTaskSequence(task.TaskIdentifier.Value, sequence);
            }
        }

        #endregion

        #region Helper Functions

        protected string GetDisplayTextType(string type)
            => !string.IsNullOrEmpty(type) ? Shift.Common.Humanizer.TitleCase(type) : null;

        #endregion
    }
}