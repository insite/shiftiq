using System;

using InSite.Cmds.Infrastructure;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Portal.Uploads
{
    public partial class Search : AdminBasePage, ICmdsUserControl
    {
        private Guid OrganizationIdentifier =>
            Guid.TryParse(Request["id"], out var value) ? value : CurrentIdentityFactory.ActiveOrganizationIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Downloads.DataBinding += (s, a) =>
                Downloads.DataSource = UploadRepository.SelectWithCounters(OrganizationIdentifier, null, Downloads.PageIndex, Downloads.PageSize);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            ActiveCompanyName.Text = Organization.CompanyName;

            Downloads.VirtualItemCount = UploadRepository.Count(OrganizationIdentifier, null);
            Downloads.PageIndex = 0;
            Downloads.DataBind();
        }

        protected string GetUploadUrl(object typeObj, object guidObj, object nameObj)
        {
            return (string)typeObj == UploadType.CmdsFile
                ? CmdsUploadProvider.GetFileRelativeUrl((Guid)guidObj, (string)nameObj)
                : (string)nameObj;
        }
    }
}