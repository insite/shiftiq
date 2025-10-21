using System;
using System.Web;
using System.Web.SessionState;

using InSite.Common.Web;
using InSite.UI.Admin.Records.Credentials.Utilities;

using Shift.Common;

namespace InSite.UI.Portal.Records.Certificates
{
    public partial class Badge : IHttpHandler, IRequiresSessionState
    {
        private HttpResponse Response;

        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            Response = context.Response;

            try
            {
                var credentialId = Guid.Parse(context.Request.QueryString["credential"]);

                var item = ServiceLocator.AchievementSearch.GetCredential(credentialId);

                if (item.HasBadgeImage != true || item.BadgeImageUrl.IsEmpty())
                    return;

                var svgBytes = BadgeHelper.GetBadgeSVGFile(item.BadgeImageUrl, item.CredentialIdentifier);
                if (svgBytes == null || svgBytes.Length == 0)
                    return;

                Response.SendFile("badge.svg", svgBytes, "image/svg+xml");

            }
            catch (Exception ex)
            {
                var action = Global.HandleException(ex);
                if (action.Type != ExceptionActionType.Ignore)
                {
                    if (action.Type == ExceptionActionType.Warning)
                        AppSentry.SentryWarning(ex);
                    else
                        AppSentry.SentryError(ex);
                }
            }
        }
    }
}