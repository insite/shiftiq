using System;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Sites.Pages;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages
{
    public partial class ChangePageSetup : AdminBasePage, IHasParentLinkParameters
    {
        private Guid PageId => Guid.TryParse(Request["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var page = ServiceLocator.PageSearch.GetPage(PageId);
            if (page == null || page.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect($"/ui/admin/sites/pages/search", true);

            PageHelper.AutoBindHeader(this, null, page.Title);

            PageDetails.BindPage(page);

            if (page.PageType == "Block")
                PageType.IncludeItems = WebPageTypeComboBox.DataItem.Block;
            else
                PageType.ExcludeItems = WebPageTypeComboBox.DataItem.Block;

            PageType.RefreshData();

            TitleInput.Text = page.Title;
            PageType.Value = page.PageType;

            CancelButton.NavigateUrl = $"/ui/admin/sites/pages/outline?id={PageId}&panel=setup";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var commands = new PageCommandGenerator().
                GetDifferencePageSetupCommands(
                    GetEntityValues(),
                    GetInputValues()
                );

            foreach (var command in commands)
            {
                ServiceLocator.SendCommand(command);

                if (command is ChangePageTitle x)
                {
                    var changeContent = CreateCommandToChangePageContent(x.AggregateIdentifier, Language.Default, x.Title);

                    ServiceLocator.SendCommand(changeContent);
                }
            }

            HttpResponseHelper.Redirect($"/ui/admin/sites/pages/outline?id={PageId}&panel=setup");
        }

        private Application.Pages.Write.ChangePageContent CreateCommandToChangePageContent(Guid pageId, string language, string title)
        {
            var block = ServiceLocator.ContentSearch.GetBlock(pageId);

            block.SetText("Title", language, title);

            var command = new Application.Pages.Write.ChangePageContent(pageId, block);

            return command;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={PageId}&panel=setup"
                : null;
        }

        private PageState GetEntityValues()
        {
            var page = ServiceLocator.PageSearch.GetPage(PageId);
            return new PageState()
            {
                Identifier = page.PageIdentifier,
                Title = page.Title,
                Type = page.PageType
            };
        }

        private PageState GetInputValues()
        {
            return new PageState()
            {
                Title = TitleInput.Text,
                Type = PageType.Value
            };
        }
    }
}