using System;

using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Sites.Pages
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid PageId => Guid.TryParse(Request["id"], out var asset) ? asset : Guid.Empty;

        private QPage Entity => _entity ?? (_entity = ServiceLocator.PageSearch.GetPage(PageId));

        private string EditorRelativePath
        {
            get
            {
                var editorRelativePath = "/ui/admin/sites/pages/outline";
                var parameters = GetEditorParameters();

                return HttpResponseHelper.BuildUrl(editorRelativePath, parameters);
            }
        }

        private string FinderRelativePath => "/ui/admin/sites/pages/search";

        #endregion

        #region Fields

        private QPage _entity;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Confirm1.AutoPostBack = true;
            Confirm1.CheckedChanged += (x, y) => { DeleteButton.Enabled = Confirm1.Checked; };

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = EditorRelativePath;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Entity == null || Entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect(FinderRelativePath);

            PageHelper.AutoBindHeader(this, null, Entity.Title);

            PageDetails.BindPage(Entity);

            // This function gets the requested page AND its downstream subpages.
            var data = ServiceLocator.PageSearch.GetDownstreamPages(Entity.PageIdentifier);
            // ... therefore we need to subtract one here.
            var containedPagesCount = data.Length - 1;
            ContainedPagesCount.Text = $"{containedPagesCount:n0}";
        }

        #endregion

        #region Event handlers

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Entity == null)
                HttpResponseHelper.Redirect(FinderRelativePath);

            var returnUrl = Entity.PageType == "Block" && Entity.ParentPageIdentifier.HasValue
                ? $"/ui/admin/sites/pages/outline?id={Entity.ParentPageIdentifier}&panel=content&tab=pageblocks"
                : FinderRelativePath;


            var commands = new PageCommandGenerator().
                DeletePageWithChildren(ServiceLocator.PageSearch.GetPageChildrenIds(Entity.PageIdentifier));

            foreach (var command in commands)
                ServiceLocator.SendCommand(command);

            HttpResponseHelper.Redirect(returnUrl);
        }

        #endregion

        #region Methods (helpers)

        private string GetEditorParameters()
        {
            return $"id={PageId}&panel=setup";
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? GetEditorParameters()
                : null;
        }

        #endregion
    }
}