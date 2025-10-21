using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Standards.Documents.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid StandardID => Guid.TryParse(Request["asset"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/standards/documents/outline?asset={StandardID}"; ;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardID);
            if (entity == null || entity.StandardType != Shift.Constant.StandardType.Document || entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect("/ui/admin/standards/documents/search", true);

            var title = $"{entity.ContentTitle ?? entity.ContentName ?? "Untitled"} <span class='form-text'>{entity.StandardType} Asset #{entity.AssetNumber}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            SetInputValues(entity);

            var competenciesCount = StandardContainmentSearch.Count(x => x.ParentStandardIdentifier == StandardID);

            CompetenciesCount.Text = $"{competenciesCount:n0}";

            if (competenciesCount > 0)
                DeleteConfirmationCheckbox.Text = "Delete Document and Competencies";
        }

        private void SetInputValues(Standard document)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(document.StandardIdentifier);
            var settings = ContentSettings.Deserialize(document.ContentSettings);

            TitleLink.Text = content.Title.Text.Default.IfNullOrEmpty("None");
            TitleLink.NavigateUrl = $"/ui/admin/standards/documents/outline?asset={document.StandardIdentifier}";

            if (document.DocumentType != null)
            {
                var sections = SectionInfo.GetDocumentSections(document.DocumentType).Select(x => new
                {
                    x.ID,
                    Title = x.GetTitle(),
                    Content = content.GetHtml(x.ID),
                    IsLocked = settings.Locked.Contains(x.ID)
                })
                .Where(x => x.Content.IsNotEmpty())
                .ToList();

                SectionRepeater.DataSource = sections;
                SectionRepeater.DataBind();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            StandardStore.Delete(StandardID);

            HttpResponseHelper.Redirect("/ui/admin/standards/documents/search");
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={StandardID}"
                : null;
        }
    }
}