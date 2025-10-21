using System;

using Humanizer;

using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Surveys
{
    public partial class RespondSearch : SearchPage<QResponseSessionFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Linker.Results.Searched += Results_Searched;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            LoadBreadcrumbs();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                SideContent.BindControlsToModel(null);
        }

        private void Results_Searched(object sender, EventArgs e)
        {
            ResponseCount.Text = Linker.Results.RowCount > 0
                ? Translate($"You have {"survey response".ToQuantity(Linker.Results.RowCount)}.")
                : Translate($"You have no survey responses.");
        }

        private void LoadBreadcrumbs()
        {
            if (!(Page is PortalBasePage portalPage))
                return;

            portalPage.AddBreadcrumb(Translate("Home"), RelativeUrl.PortalHomeUrl);
            portalPage.AddBreadcrumb(Translate("Surveys"), "/ui/survey/respond/search");

            ResultsTitle.InnerText = Translate("Applications and Survey Responses");
        }
    }
}