using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Standards.Collections.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid AssetID => Guid.TryParse(Request["asset"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/standards/collections/search";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!IsPostBack)
            {
                var entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == AssetID);
                if (entity == null || entity.StandardType != Shift.Constant.StandardType.Collection || entity.OrganizationIdentifier != Organization.Identifier)
                    HttpResponseHelper.Redirect("/ui/admin/standards/collections/search", true);

                var title = $"{entity.ContentTitle ?? entity.ContentName ?? "Untitled"} <span class='form-text'>{entity.StandardType} Asset #{entity.AssetNumber}</span>";
                
                PageHelper.AutoBindHeader(this, null, title);

                var competenciesCount = StandardContainmentSearch.Count(x => x.ParentStandardIdentifier == AssetID);

                CompetenciesCount.Text = $"{competenciesCount:n0}";

                if (competenciesCount > 0)
                    DeleteConfirmationCheckbox.Text = "Delete Collection and Competencies";

                TitleOutput.Text = $"<a href=\"/ui/admin/standards/collections/outline?asset={AssetID}\">{entity.ContentTitle}</a>" ;
                Label.Text = entity.StandardLabel;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            StandardStore.Delete(AssetID);

            HttpResponseHelper.Redirect("/ui/admin/standards/collections/search");
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={AssetID}"
                : null;
        }
    }
}