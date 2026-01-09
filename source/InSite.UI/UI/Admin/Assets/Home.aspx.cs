using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assets
{
    public class HomeModel
    {
        public int ContentCount { get; set; }
        public int GlossaryCount { get; set; }
        public int LabelCount { get; set; }
        public int UploadCount { get; set; }
    }

    public partial class Dashboard : AdminBasePage
    {
        protected HomeModel CreateHomeModel()
        {
            return new HomeModel
            {
                ContentCount = ServiceLocator.ContentSearch.Count(x => x.OrganizationIdentifier == Identity.Organization.Identifier),
                GlossaryCount = ServiceLocator.GlossarySearch.CountTerms(new GlossaryTermFilter { GlossaryIdentifier = GlossaryHelper.GlossaryIdentifier }),
                LabelCount = LabelSearch.Search(new LabelFilter()).Count,
                UploadCount = ServiceLocator.UploadSearch.Count(x => x.OrganizationIdentifier == Identity.Organization.Identifier)
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindModelToControls(CreateHomeModel());
        }

        protected void BindModelToControls(HomeModel model)
        {
            PageHelper.AutoBindHeader(this);

            LoadCounter(ContentCard, ContentCount, ContentLink, model.ContentCount, PermissionIdentifiers.Admin_Assets, "/ui/admin/assets/contents/search");
            ContentCard.Visible = ContentCard.Visible && Identity.IsOperator;

            LoadCounter(GlossaryCard, GlossaryCount, GlossaryLink, model.GlossaryCount, PermissionIdentifiers.Admin_Assets, "/ui/admin/assets/glossaries/search");
            LoadCounter(LabelCard, LabelCount, LabelLink, model.LabelCount, PermissionIdentifiers.Admin_Assets, "/ui/admin/assets/labels/search");
            LoadCounter(UploadCard, UploadCount, UploadLink, model.UploadCount, PermissionIdentifiers.Admin_Assets_Uploads, "/ui/admin/assets/uploads/browse");

            FileBrowserLink.Visible = Identity.IsOperator;
            FilesSection.Visible = Identity.IsOperator || UploadCard.Visible;
        }

        public void LoadCounter(HtmlGenericControl card, Literal counter, HtmlAnchor link, int count, Guid toolkit, string url)
        {
            counter.Text = $@"{count:n0}";
            card.Visible = Identity.IsGranted(toolkit);
            link.HRef = url;
        }
    }
}