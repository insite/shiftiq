using System.Linq;
using System.Web.UI;

using InSite.Domain.Foundations;
using InSite.Persistence.Content;
using InSite.UI.Layout.Common.Contents;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Sites
{
    public partial class PageArticle : UserControl, IPortalPage
    {
        public PortalPageModel Model { get; set; }

        public void BindModelToControls(PortalPageModel model, ISecurityFramework identity)
        {
            Model = model;

            CurrentSessionState.LastPortalPageUrl = Page.Request.RawUrl;

            var translator = ((PortalBasePage)Page).Translator;

            Model = LaunchCardBuilder.CreatePortalPage(Page.RouteData.Values, identity, translator, identity.Language);

            OutlineBody.Visible = Model.Body.HasValue();
            OutlineBody.InnerHtml = Model.Body;

            if (Model.Page.Children.Count > 0)
                LoadBodyContentBlocks();
        }

        private void LoadBodyContentBlocks()
        {
            foreach (var child in Model.Page.Children.OrderBy(y => y.Sequence))
            {
                if (child.PageType != "Block")
                    continue;

                try
                {
                    var info = child.ContentControl.IsNotEmpty()
                        ? ControlPath.GetPageBlock(child.ContentControl)
                        : null;

                    if (info == null)
                        continue;

                    var control = LoadControl(info.Path);
                    var childContent = ServiceLocator.ContentSearch.GetBlock(child.PageIdentifier);
                    ((IBlockControl)control).BindContent(childContent, child.Hook);

                    BodyContentBlocks.Controls.Add(control);
                }
                catch (BlockContentControlNotFound ex)
                {
                    DangerPanel.InnerText = ex.Message;
                    DangerPanel.Visible = true;
                }
            }
        }
    }
}