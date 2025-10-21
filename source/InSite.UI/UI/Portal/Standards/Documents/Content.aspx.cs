using System;
using System.Web.UI;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Standards.Documents
{
    public partial class Content : PortalBasePage
    {
        #region Properties

        private Guid? AssetID => Guid.TryParse(Request["standard"], out var value) ? value : (Guid?)null;
        private string PageAction => Request.QueryString["action"];

        private string Tab => Request["tab"];

        private string[] Fields
        {
            get => (string[])ViewState[nameof(Fields)];
            set => ViewState[nameof(Fields)] = value;
        }

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
                Open();

            RenderBreadcrumb($"?standard={AssetID}");
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
            if (entity == null
                || entity.StandardType != StandardType.Document
                || entity.OrganizationIdentifier != Organization.Identifier
                || entity.StandardPrivacyScope.IfNullOrEmpty("Tenant") == "User"
                    && entity.CreatedBy != User.UserIdentifier
                    && !CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards_Documents, PermissionOperation.Configure)
                )
            {
                RedirectToSearch();
            }

            var title = $"Change {entity.DocumentType} Content";

            SectionTitle.Text = $"{Translate(title)}";

            SetInputValues(entity);
        }

        private bool Save()
        {
            if (!Page.IsValid)
                return false;

            var standard = ServiceLocator.StandardSearch.GetStandard(AssetID.Value);
            if (standard == null)
                return true;

            var data = new ContentContainer();

            GetInputValues(standard, data);

            StandardStore.Update(standard);
            ServiceLocator.SendCommand(new ModifyStandardContent(standard.StandardIdentifier, data));

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Standard document)
        {
            PageHelper.AutoBindHeader(this);

            var master = (PortalMaster)Master;
            master.Breadcrumbs.BindTitleAndSubtitle(
                ServiceLocator.ContentSearch.GetText(document.StandardIdentifier, ContentLabel.Title, CurrentLanguage) ?? Translate("Untitled"),
                $"{Translate(document.StandardType)} {Translate("Asset")} #{document.AssetNumber}");

            if (ContentEditor.IsEmpty)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(document.StandardIdentifier);
                var tooltips = ServiceLocator.ContentSearch.GetTooltipBlock(document.OrganizationIdentifier);
                var settings = ContentSettings.Deserialize(document.ContentSettings);

                if (!settings.Locked.Contains("Title"))
                {
                    ContentEditor.Add(new LayoutContentSection.SingleLine("Title")
                    {
                        Title = Translate("Title"),
                        Value = content.Title.Text
                    });
                }

                var sections = SectionInfo.GetDocumentSections(document.DocumentType);
                foreach (var section in sections)
                {
                    if (settings.Locked.Contains(section.ID))
                        continue;

                    if (document.DocumentType.Equals("Customized Occupation Profile")
                        &&
                        (
                            section.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Acknowledgements]")
                            || section.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Standards Development Process]")
                            || section.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Certification Development Process]")
                        ))
                        continue;

                    var sectionTitle = section.GetTitle();

                    var tooltipLabel = section.LabelID + ".[Tooltip]";
                    if (tooltipLabel.Equals("[Standards/Document/Content].[Job Description].[Required Certifications, Licenses Credentials].[Tooltip]"))
                        tooltipLabel = "[Standards/Document/Content].[Job Description].[Required Certifications].[Tooltip]";

                    var tooltip = tooltips[tooltipLabel].Text;
                    var html = content[section.ID].Html;
                    var text = content[section.ID].Text;

                    if (!html.IsEmpty)
                    {
                        ContentEditor.Add(new LayoutContentSection.Html(section.ID)
                        {
                            Title = sectionTitle,
                            Label = sectionTitle,
                            Value = html,
                            Tooltip = tooltip.Get(CurrentLanguage)
                        });
                    }
                    else
                    {
                        ContentEditor.Add(new LayoutContentSection.Markdown(section.ID)
                        {
                            Title = sectionTitle,
                            Label = sectionTitle,
                            Value = text,
                            Tooltip = tooltip.Get(CurrentLanguage)
                        });
                    }
                }

                ContentEditor.SetLanguage(CurrentLanguage);
                ContentEditor.OpenTab(Tab);
            }
        }

        private void GetInputValues(QStandard entity, Shift.Common.ContentContainer data)
        {
            data.Title.Text = ContentEditor.GetValue(ContentSectionDefault.Title);

            var sections = SectionInfo.GetDocumentSections(entity.DocumentType);
            var settings = ContentSettings.Deserialize(entity.ContentSettings);

            foreach (var section in sections)
            {
                if (settings.Locked.Contains(section.ID))
                    continue;

                data[section.ID].Text = ContentEditor.GetValue(section.ID);
            }
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/portal/standards/documents/search", true);

        private void RedirectToParent()
        {
            if (!string.IsNullOrEmpty(PageAction))
                HttpResponseHelper.Redirect($"/ui/portal/standards/documents/outline?standard={AssetID}&action={PageAction}");
            HttpResponseHelper.Redirect($"/ui/portal/standards/documents/outline?standard={AssetID}");
        }


        #endregion
    }
}