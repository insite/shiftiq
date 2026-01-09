using System;
using System.Collections.Generic;

using InSite.Application.Contents.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Utilities.Labels.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/assets/labels/search";
        private const string EditUrl = "/ui/admin/assets/labels/edit";

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
                if (!CanEdit)
                    HttpResponseHelper.Redirect(SearchUrl);

                PageHelper.AutoBindHeader(this);

                CancelButton.NavigateUrl = SearchUrl;

                Detail.SetDefaultValues(true);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var labelName = Save();
            if (labelName == null)
                return;

            var url = GetEditUrl(labelName);

            HttpResponseHelper.Redirect(url);
        }

        private string Save()
        {
            var contents = new List<TContent>();

            Detail.GetInputValues(contents);

            string labelName;

            if (contents.Count == 0)
                return null;

            labelName = contents[0].ContentLabel;

            var exists = ServiceLocator.ContentSearch.Exists(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == labelName && x.ContainerType == ContentContainerType.Application);

            if (exists)
            {
                var url = GetEditUrl(labelName);
                CreatorStatus.AddMessage(AlertType.Error, $"Another label with the same placeholder name <a href='{url}'>already exists</a>.");
                return null;
            }

            foreach (var content in contents)
                ServiceLocator.ContentStore.Save(content);

            LabelSearch.Refresh();

            return labelName;
        }

        private static string GetEditUrl(string labelName)
            => EditUrl + $"?label={labelName}";
    }
}