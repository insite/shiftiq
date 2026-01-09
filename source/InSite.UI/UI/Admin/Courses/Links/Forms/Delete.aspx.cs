using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Courses.Links.Forms
{
    public partial class Delete : AdminBasePage
    {
        private Guid LinkID => Guid.TryParse(Request["link"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = "/ui/admin/courses/links/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var entity = LtiLinkSearch.Select(LinkID);
                if (entity == null)
                {
                    HttpResponseHelper.Redirect("/ui/admin/courses/links/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null, entity.ResourceTitle);

                TitleOutput.Text = $"<a href=\"/ui/admin/courses/links/edit?link={entity.LinkIdentifier}\">{entity.ResourceTitle}</a>";
                Description.Text = !string.IsNullOrEmpty(entity.ResourceSummary) ? entity.ResourceSummary.Replace("\n", "<br>") : "None";
                Url.Text = $"<a href=\"{ entity.ToolProviderUrl}\">{entity.ToolProviderUrl}</a>";
                Code.Text = entity.ResourceCode;
                Publisher.Text = entity.ToolProviderName;
                Subtype.Text = entity.ToolProviderType;
                Location.Text = !string.IsNullOrEmpty(entity.ResourceName) ? entity.ResourceName : "None";
                Key.Text = entity.ToolConsumerKey;
                Secret.Text = entity.ToolConsumerSecret;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            LtiLinkStore.Delete(LinkID);

            HttpResponseHelper.Redirect("/ui/admin/courses/links/search");
        }
    }
}