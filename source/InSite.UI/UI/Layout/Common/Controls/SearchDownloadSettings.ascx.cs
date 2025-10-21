using System;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class SearchDownloadSettings : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            TranslateControls();

            PlatformSearchDownloadMaximumRows.Text = ServiceLocator.AppSettings.Platform.Search.Download.MaximumRows.ToString("N0");

            var email = ServiceLocator.Partition.Email;

            SupportEmailLink.Text = $"<a target=_blank href='mailto:{email}'>{email}</a>";
        }

        private void TranslateControls()
        {
            foreach (ListItem item in OutputFileFormat.Items)
                item.Text = Translate(item.Text);

            foreach (ListItem item in IsShowHidden.Items)
                item.Text = Translate(item.Text);
        }

        internal void SetInputValues(Domain.Reports.IDownload settings)
        {
            var formatItem = OutputFileFormat.Items.FindByValue(settings.Format);
            if (formatItem != null)
                OutputFileFormat.SelectedValue = formatItem.Value;

            var showHiddenItem = IsShowHidden.Items.FindByValue(settings.ShowHidden.ToString());
            if (showHiddenItem != null)
                IsShowHidden.SelectedValue = showHiddenItem.Value;

            IsRemoveSpaces.Checked = settings.RemoveSpaces;
        }

        internal void GetInputValues(Domain.Reports.IDownload settings)
        {
            settings.Format = OutputFileFormat.SelectedValue;
            settings.ShowHidden = IsShowHidden.SelectedValue == bool.TrueString;
            settings.RemoveSpaces = IsRemoveSpaces.Checked;
        }
    }
}