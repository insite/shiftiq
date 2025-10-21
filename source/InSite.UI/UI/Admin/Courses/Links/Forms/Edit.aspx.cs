using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Courses.Links.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/courses/links/search";
        private const string CreateUrl = "/ui/admin/courses/links/create";

        private Guid LinkIdentifier => Guid.TryParse(Request.QueryString["link"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void Open()
        {
            var link = LtiLinkSearch.Select(LinkIdentifier);

            if (link == null)
            {
                HttpResponseHelper.Redirect(CreateUrl, true);
                return;
            }

            PageHelper.AutoBindHeader(this, null, link.ResourceTitle);

            Detail.SetInputValues(link);
        }

        private void Save()
        {
            var entity = LtiLinkSearch.Select(LinkIdentifier);

            Detail.GetInputValues(entity);

            LtiLinkStore.Update(entity);
        }
    }
}
