using System.Collections.Generic;
using System.Web.UI;

using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.Persistence.Content;
using InSite.UI.Portal.Sites;

namespace InSite.UI.Portal.Content.Sites.Controls
{
    public partial class Catalog : UserControl, IPortalPage
    {
        public PortalPageModel Model { get; set; }

        public List<LaunchCard> Cards { get; set; }

        public void BindModelToControls(PortalPageModel model, ISecurityFramework identity)
        {
            Model = model;

            Cards = CourseSearch.GetCatalogCourseList(Model.Page.ObjectIdentifier, identity, LaunchCardBuilder.GetProgress);

            Repeater.BindModelToControls(Cards, identity);
        }
    }
}