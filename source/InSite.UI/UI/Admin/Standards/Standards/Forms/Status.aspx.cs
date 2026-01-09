using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Standards.Standards.Forms
{
    public partial class Status : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/standards/standards/search";
        private const string EditUrl = "/ui/admin/standards/edit";

        private Guid StandardId => Guid.TryParse(Request.QueryString["standard"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;

            StatusList.DataBinding += StatusList_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var entity = StandardSearch.Select(StandardId);
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var title = StringHelper.FirstValue(entity.ContentTitle, entity.ContentName, "Untitled");

            PageHelper.AutoBindHeader(this, null, $"{title} <span class='form-text'>{entity.StandardType}</span>");

            StatusList.DataBind();
            StatusList.SelectedValue = entity.StandardStatus.EmptyIfNull();

            StatusUpdatedBy.Text = UserSearch.GetFullName(entity.StandardStatusLastUpdateUser).NullIf(UserNames.Someone);
            StatusDateUpdated.Text = entity.StandardStatusLastUpdateTime.Format(User.TimeZone);

            CancelButton.NavigateUrl = GetEditLinkUrl();
        }

        private void StatusList_DataBinding(object sender, EventArgs e)
        {
            StatusList.DataTextField = nameof(TCollectionItem.ItemName);
            StatusList.DataValueField = nameof(TCollectionItem.ItemName);
            StatusList.DataSource = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Standards_Classification_Status
            });
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = ServiceLocator.StandardSearch.GetStandard(StandardId);

            entity.StandardStatus = StatusList.SelectedValue;
            entity.StandardStatusLastUpdateTime = DateTimeOffset.Now;
            entity.StandardStatusLastUpdateUser = User.Identifier;

            StandardStore.Update(entity);

            HttpResponseHelper.Redirect(GetEditLinkUrl());
        }

        public string GetParentLinkParameters(IWebRoute parent) =>
            parent.Name.EndsWith("/edit") ? GetEditLinkParameters() : null;

        private string GetEditLinkParameters() => $"id={StandardId}";

        private string GetEditLinkUrl() => EditUrl + "?" + GetEditLinkParameters();
    }
}