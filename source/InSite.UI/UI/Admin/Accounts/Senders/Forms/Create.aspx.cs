using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Senders.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/senders/edit";
        private const string SearchUrl = "/ui/admin/accounts/senders/search";

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

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            Details.OrganizationIdentifier = Organization.Identifier;

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var entity = new TSender();

            Details.GetInputValues(entity);

            entity.SenderIdentifier = UniqueIdentifier.Create();

            TSenderStore.Insert(entity);

            TSenderOrganizationStore.Insert(Details.OrganizationIdentifier.Value, entity.SenderIdentifier);

            HttpResponseHelper.Redirect($"{EditUrl}?id={entity.SenderIdentifier}&status=saved");
        }
    }
}