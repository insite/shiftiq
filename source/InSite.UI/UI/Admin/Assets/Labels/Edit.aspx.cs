using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contents.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Labels.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/labels/search";

        private string LabelName => Request.QueryString["label"];

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

            SaveButton.Enabled = CanEdit;

            Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            if (Save())
                SetStatus(EditorStatus, StatusType.Saved);
        }

        private void Open()
        {
            var contents = ServiceLocator.ContentSearch.SelectContainers(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == LabelName && x.ContainerType == ContentContainerType.Application);

            if (contents.Length == 0)
            {
                HttpResponseHelper.Redirect("/ui/admin/assets/labels/search");
                return;
            }

            PageHelper.AutoBindHeader(this);

            Detail.SetInputValues(contents);

            var en = contents.FirstOrDefault(x => x.ContentLanguage == "en");
            var hasRefs = en != null && en.ReferenceCount > 0;
            ReferenceSection.Visible = hasRefs;
            ReferenceLiteral.Text = hasRefs ? Markdown.ToHtml(en.ReferenceFiles) : string.Empty;

            CancelButton.NavigateUrl = SearchUrl;
        }

        private bool Save()
        {
            var contents = new List<TContent>();

            Detail.GetInputValues(contents);

            var existing = ServiceLocator.ContentSearch.SelectContainers(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == LabelName && x.ContainerType == ContentContainerType.Application);

            var update = new List<TContent>();
            var delete = new List<TContent>();

            foreach (var content in contents)
            {
                var existingContent = existing.FirstOrDefault(x => x.ContentLanguage == content.ContentLanguage && x.OrganizationIdentifier == content.OrganizationIdentifier);

                if (existingContent == null)
                {
                    update.Add(content);
                }
                else if (existingContent.ContentText != content.ContentText)
                {
                    existingContent.ContentText = content.ContentText;
                    update.Add(existingContent);
                }
            }

            foreach (var existingContent in existing)
            {
                if (!contents.Any(x => x.ContentLanguage == existingContent.ContentLanguage && x.OrganizationIdentifier == existingContent.OrganizationIdentifier))
                    delete.Add(existingContent);
            }

            ServiceLocator.ContentStore.Save(update);

            foreach (var content in delete)
                ServiceLocator.ContentStore.Delete(content.ContentIdentifier);

            if (update.Count > 0 || delete.Count > 0)
                LabelSearch.Refresh();

            return true;
        }
    }
}
