using System;

using InSite.Admin.Standards.Occupations.Controls;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Standards.Collections.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Standard _entity;
        private bool _isEntityLoaded;

        private string FinderRelativePath => "/ui/admin/standards/collections/search";

        private Guid StandardIdentifier => Guid.TryParse(Request["asset"], out var value) ? value : Guid.Empty;

        protected Standard Entity
        {
            get
            {
                if (_isEntityLoaded)
                    return _entity;

                _entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier);
                _isEntityLoaded = true;

                return _entity;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack)
                Open();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            _isEntityLoaded = false;

            if (Entity == null)
                HttpResponseHelper.Redirect(FinderRelativePath);

            var title = $"{Entity.ContentTitle ?? Entity.ContentName ?? "Untitled"}";

            PageHelper.AutoBindHeader(this, null, title);

            PrintDocumentButton.NavigateUrl = $"/ui/admin/standards/collections/print?asset={Entity.StandardIdentifier}";

            SetInputValues(Entity);

            CompetenciesPanel.SetAssetType("Collection");

            DeleteLink.NavigateUrl = $"/admin/standards/collections/delete?asset={Entity.StandardIdentifier}";
        }

        private void SetInputValues(Standard entity)
        {
            Detail.SetInputValues(entity);
            DuplicateButton.NavigateUrl = new ReturnUrl($"asset={Entity.StandardIdentifier}")
                    .GetRedirectUrl($"/ui/admin/standards/collections/create?action=duplicate&asset={Entity.StandardIdentifier}");

            NewClassLink.NavigateUrl = new ReturnUrl($"asset={Entity.StandardIdentifier}")
                    .GetRedirectUrl($"/ui/admin/standards/collections/create");

            DownloadLink.NavigateUrl = $"/ui/admin/standards/collections/download?asset={Entity.StandardIdentifier}";
            DuplicateButton.Visible = CanEdit;
            CompetenciesPanel.SetInputValues(entity);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveClicked();
        }

        private void SaveClicked()
        {
            if (!Page.IsValid)
                return;           

            Open();
        }
    }
}