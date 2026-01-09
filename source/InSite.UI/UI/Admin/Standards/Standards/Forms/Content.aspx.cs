using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? ContainerId => Guid.TryParse(Request["container"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

        private string[] Fields
        {
            get => (string[])ViewState[nameof(Fields)];
            set => ViewState[nameof(Fields)] = value;
        }

        #endregion

        #region Security

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CurrentSessionState.Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            TitleRequiredValidator.ServerValidate += TitleRequiredValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write))
                    RedirectToSearch();

                Open();
            }
        }
        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Save())
                RedirectToParent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToParent();
        }

        private void TitleRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var text = ContentEditor.GetValue("Title", ContentSectionDefault.BodyText);

            args.IsValid = text.Exists(MultilingualString.DefaultLanguage) && !text.Default.IsEmpty();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var standard = ContainerId.HasValue
                ? StandardSearch.Select(ContainerId.Value)
                : null;

            if (standard == null)
                RedirectToSearch();

            if (standard.DocumentType != null)
                RedirectToParent();

            Fields = Organization.GetStandardContentLabels();
            if (Fields.Length == 0)
                RedirectToParent();

            SetInputValues(standard);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            if (!ContainerId.HasValue)
                return true;

            var data = new Shift.Common.ContentContainer();

            GetInputValues(data);

            ServiceLocator.SendCommand(new ModifyStandardContent(ContainerId.Value, data));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Standard standard)
        {
            PageHelper.AutoBindHeader(this, null, standard.ContentTitle ?? standard.ContentName);

            var content = ServiceLocator.ContentSearch.GetBlock(standard.StandardIdentifier, labels: Fields);

            foreach (var name in Fields)
            {
                var item = content[name];

                ContentEditor.Add(new AssetContentSection.MarkdownAndHtml(name)
                {
                    Title = name,
                    HtmlValue = item.Html,
                    MarkdownValue = item.Text,
                    IsMultiValue = true
                });
            }

            ContentEditor.SetLanguage(standard.Language ?? CurrentSessionState.Identity.Language);
            ContentEditor.OpenTab(Tab);
        }

        private void GetInputValues(Shift.Common.ContentContainer data)
        {
            foreach (var name in Fields)
            {
                var item = data[name];

                item.Html = ContentEditor.GetValue(name, ContentSectionDefault.BodyHtml);
                item.Text = ContentEditor.GetValue(name, ContentSectionDefault.BodyText);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/standards/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/edit?id={ContainerId}&panel=content&tab={Tab}");

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"asset={ContainerId}"
                : null;
        }

        #endregion
    }
}
