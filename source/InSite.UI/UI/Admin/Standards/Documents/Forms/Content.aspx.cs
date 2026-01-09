using System;
using System.Web.UI;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

using ContentSectionDefault = Shift.Constant.ContentSectionDefault;
using StandardType = Shift.Constant.StandardType;

namespace InSite.Admin.Standards.Documents.Forms
{
    public partial class Content : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? AssetID => Guid.TryParse(Request["asset"], out var value) ? value : (Guid?)null;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanEdit)
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

        #endregion

        #region Database operations

        private void Open()
        {
            var entity = AssetID.HasValue ? StandardSearch.Select(AssetID.Value) : null;
            if (entity == null || entity.StandardType != StandardType.Document || entity.OrganizationIdentifier != Organization.Identifier)
                RedirectToSearch();

            SetInputValues(entity);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var standard = ServiceLocator.StandardSearch.GetStandard(AssetID.Value);
            if (standard == null)
                return true;

            var data = new Shift.Common.ContentContainer();
            GetInputValues(standard, data);

            StandardStore.Update(standard);
            ServiceLocator.SendCommand(new ModifyStandardContent(standard.StandardIdentifier, data));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Standard document)
        {
            PageHelper.AutoBindHeader(this, null, document.ContentTitle ?? document.ContentName);

            if (ContentEditor.IsEmpty)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(document.StandardIdentifier);
                var settings = ContentSettings.Deserialize(document.ContentSettings);

                if (!settings.Locked.Contains("Title"))
                {
                    ContentEditor.Add(new AssetContentSection.SingleLine("Title")
                    {
                        Title = "Title",
                        Value = content.Title.Text
                    });
                }

                if (document.DocumentType != null)
                {
                    var sections = SectionInfo.GetDocumentSections(document.DocumentType);
                    foreach (var section in sections)
                    {
                        if (settings.Locked.Contains(section.ID))
                            continue;

                        var sectionTitle = section.GetTitle();

                        var html = content[section.ID].Html;
                        var text = content[section.ID].Text;

                        if (!html.IsEmpty)
                            ContentEditor.Add(new AssetContentSection.Html(section.ID)
                            {
                                Title = sectionTitle,
                                Label = sectionTitle,
                                Value = html
                            });
                        else
                            ContentEditor.Add(new AssetContentSection.Markdown(section.ID)
                            {
                                Title = sectionTitle,
                                Label = sectionTitle,
                                Value = text
                            });
                    }
                }

                ContentEditor.SetLanguage(document.Language ?? CurrentSessionState.Identity.Language);
                ContentEditor.OpenTab(Request["tab"]);
            }
        }

        private void GetInputValues(QStandard entity, Shift.Common.ContentContainer data)
        {
            data.Title.Text = ContentEditor.GetValue(ContentSectionDefault.Title);

            var settings = ContentSettings.Deserialize(entity.ContentSettings);

            if (entity.DocumentType != null)
            {
                var sections = SectionInfo.GetDocumentSections(entity.DocumentType);

                foreach (var section in sections)
                {
                    if (settings.Locked.Contains(section.ID))
                        continue;

                    data[section.ID].Text = ContentEditor.GetValue(section.ID);
                }
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/documents/search", true);

        private void RedirectToParent() =>
            HttpResponseHelper.Redirect($"/ui/admin/standards/documents/outline?asset={AssetID}", true);

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={AssetID}"
                : null;
        }

        #endregion
    }
}