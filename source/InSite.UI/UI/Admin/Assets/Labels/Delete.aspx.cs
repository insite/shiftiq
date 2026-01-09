using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Assets.Labels
{
    public partial class Delete : AdminBasePage
    {
        private string LabelIdentifier => Request["label"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = "/ui/admin/assets/labels/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var contents = ServiceLocator.ContentSearch.SelectContainers(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == LabelIdentifier && x.ContainerType == ContentContainerType.Application);

            if (contents.Length == 0)
            {
                HttpResponseHelper.Redirect("/ui/admin/assets/labels/search");
                return;
            }

            var defaultContent = contents.FirstOrDefault(x => string.Equals(x.ContentLanguage, "en", StringComparison.OrdinalIgnoreCase)) ?? contents[0];
            PlaceHolderName.Text = defaultContent.ContentLabel;
            DefaultText.Text = defaultContent.ContentSnip;

            PageHelper.AutoBindHeader(this, null, defaultContent.ContentText);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.ContentStore.DeleteContainerByType(LabelSearch.ContainerIdentifier, LabelIdentifier, ContentContainerType.Application);

            LabelSearch.Refresh();

            HttpResponseHelper.Redirect("/ui/admin/assets/labels/search");
        }

    }
}