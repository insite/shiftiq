using System;
using System.Linq;

using InSite.Admin.Sites.Pages.Controls;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Pages;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages
{
    public partial class ChangePageContent : AdminBasePage, IHasParentLinkParameters
    {
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        private QPage Entity
        {
            get
            {
                if (_isEntityLoaded)
                    return _entity;

                _entity = ServiceLocator.PageSearch.Select(PageId, new System.Linq.Expressions.Expression<Func<QPage, object>>[] { x => x.Site, x => x.Parent });
                _isEntityLoaded = true;

                return _entity;
            }
        }

        private string[] ContentFields
        {
            get => (string[])ViewState[nameof(ContentFields)];
            set => ViewState[nameof(ContentFields)] = value;
        }

        #region Fields

        private QPage _entity;
        private bool _isEntityLoaded;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += (x, y) => RedirectToParent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ContentEditor.GetSection("PageBlocks", true) is ContentBlocksManager blocksManager)
            {
                blocksManager.StatusUpdated += BlocksManager_Alert;
                blocksManager.BlockInserted += BlocksManager_BlockInserted;
                blocksManager.BlockDeleted += BlocksManager_BlockDeleted;
            }

            if (!IsPostBack)
            {
                var page = ServiceLocator.PageSearch.GetPage(PageId);

                if (page == null || page.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, page.Title);

                SetInputValues(page);
            }
        }

        private void RedirectToParent()
        {
            if (ContentEditor.GetSection("PageBlocks", true) is ContentBlocksManager blocksManager)
            {
                var navBack = blocksManager.GetNavTitle();
                if (!string.IsNullOrEmpty(navBack))
                    HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=content&tab={ContentEditor.GetCurrentTab()}&nav={navBack}", true);
            }
            HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=content&tab={ContentEditor.GetCurrentTab()}", true);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var page = ServiceLocator.PageSearch.GetPage(PageId);
            if (page == null)
            {
                HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search");
                return;
            }

            var content = new ContentContainer();

            GetInputValues(content);

            string navBack = "";

            if (ContentEditor.GetSection("PageBlocks", true) is ContentBlocksManager blocksManager)
            {
                blocksManager.Save();
                navBack = blocksManager.GetNavTitle();
            }

            ServiceLocator.SendCommand(new InSite.Application.Pages.Write.ChangePageContent(page.PageIdentifier, content));

            var title = content["Title"]?.Text?.Default;

            if (title != null)
            {
                var commands = new PageCommandGenerator().
                GetDifferencePageSetupCommands(
                    GetEntityValues(),
                    GetInputValues(title)
                );

                foreach (var command in commands)
                {
                    ServiceLocator.SendCommand(command);
                }
            }

            if (string.IsNullOrEmpty(navBack))
            {
                HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=content&tab="
                    + ContentEditor.GetCurrentTab()?.ToLower());
            }
            else
            {
                HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=content&tab="
                    + ContentEditor.GetCurrentTab()?.ToLower() + $"&nav={navBack}");
            }
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup"
                : null;
        }

        public void GetInputValues(ContentContainer data)
        {
            foreach (var name in ContentFields)
            {
                var item = data[name];

                if (name == "HtmlHead")
                {
                    item.Html = ContentEditor.GetValue(name);
                }
                else if (name == "PageBlocks")
                {

                }
                else
                {
                    item.Html = ContentEditor.GetValue(name, ContentSectionDefault.BodyHtml);
                    item.Text = ContentEditor.GetValue(name, ContentSectionDefault.BodyText);
                }
            }
        }

        public void SetInputValues(QPage entity)
        {
            {
                ContentFields = (entity.ContentLabels ?? string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.IsNotEmpty() && (entity.PageType != "Folder" || !x.Equals("PageBlocks", StringComparison.OrdinalIgnoreCase)))
                    .ToArray();

                var selectedTab = ContentEditor.GetCurrentTab();
                var selectedNavigation = ContentEditor.GetCurrentSubTab();

                var selectedSection = (ContentEditor.GetSection("PageBlocks", true) as ContentBlocksManager)?.GetSectionItem();

                var uploadFolderPath = $"/sites/{Organization.OrganizationCode}/{entity.PageSlug}";

                ContentEditor.Clear();

                var content = ServiceLocator.ContentSearch.GetBlock(entity.PageIdentifier, labels: ContentFields);

                foreach (var name in ContentFields)
                {
                    var item = content[name];

                    if (name == "HtmlHead")
                    {
                        ContentEditor.Add(new ContentSectionHtmlCode.SectionSettings(name)
                        {
                            Title = "&lt;HEAD&gt;",
                            Value = item.Html,
                            IsRequired = false
                        });
                    }
                    else if (name == "PageBlocks")
                    {
                        ContentEditor.Add(new ContentBlocksManager.SectionSettings(name)
                        {
                            ParentPage = entity,
                            Title = "Blocks"
                        });
                    }
                    else
                    {
                        ContentEditor.Add(new AssetContentSection.MarkdownAndHtml(name)
                        {
                            Title = name,
                            HtmlValue = item.Html,
                            MarkdownValue = item.Text,
                            IsMultiValue = true,
                            AllowUpload = true,
                            UploadFolderPath = uploadFolderPath
                        });
                    }
                }

                ContentEditor.SetLanguage(CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);

                if (selectedTab != null)
                    ContentEditor.OpenTab(selectedTab);

                if (selectedSection.HasValue)
                    (ContentEditor.GetSection("PageBlocks", true) as ContentBlocksManager)?.OpenSectionItem(selectedSection.Value);
            }
        }

        private PageState GetEntityValues()
        {
            var page = ServiceLocator.PageSearch.GetPage(PageId);
            return new PageState()
            {
                Identifier = page.PageIdentifier,
                Title = page.Title,
            };
        }

        private PageState GetInputValues(string title)
        {
            return new PageState()
            {
                Title = title,
            };
        }
        private void BlocksManager_Alert(object sender, AlertArgs args)
        {
            EditorStatus.AddMessage(args.Type, args.Text);
        }

        private void BlocksManager_BlockInserted(object sender, EventArgs e)
        {
            SetInputValues(Entity);
        }

        private void BlocksManager_BlockDeleted(object sender, EventArgs e)
        {
            SetInputValues(Entity);
        }
    }
}