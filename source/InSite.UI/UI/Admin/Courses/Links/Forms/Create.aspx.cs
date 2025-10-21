using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Courses.Links.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/courses/links/edit";
        private const string SearchUrl = "/ui/admin/courses/links/search";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var link = new TLtiLink();

            Detail.GetInputValues(link);

            link.OrganizationIdentifier = Organization.OrganizationIdentifier;
            link.AssetNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            link.LinkIdentifier = UniqueIdentifier.Create();

            LtiLinkStore.Insert(link);

            HttpResponseHelper.Redirect(EditUrl + $"?link={link.LinkIdentifier}");
        }
    }
}
