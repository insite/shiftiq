using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.UI.Admin.Integrations.ApiRequests
{
    public partial class Search : SearchPage<ApiRequestFilter>
    {
        public override string EntityName => "API Request";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnSearching(ApiRequestFilter filter)
        {
            if (filter.RequestStartedSince == null)
            {
                SearchAlert.AddMessage(AlertType.Error, "Please enter a start date in your search criteria");
                return;
            }

            base.OnSearching(filter);
        }
    }
}