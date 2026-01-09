using System;
using System.IO;
using System.Text;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class ReviewPrint : SubmissionSessionControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack || !Current.IsValid)
                HttpResponseHelper.SendHttp400();

            if (Current.Survey.UserFeedback == UserFeedbackType.Disabled)
                return;

            UserFeedbackType userFeedback;
            switch (Request.QueryString["print"])
            {
                case "summary":
                    userFeedback = UserFeedbackType.Summary;
                    break;
                case "answered":
                    userFeedback = UserFeedbackType.Answered;
                    break;
                default:
                    userFeedback = UserFeedbackType.Detailed;
                    break;
            }

            Details.LoadData(Current, Navigator, userFeedback, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var htmlBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(htmlBuilder))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    Page.Master.RenderControl(htmlWriter);
            }

            var htmlString = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, htmlBuilder);

            var type = Request.QueryString["type"];
            if (type == "html")
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                Response.Write(htmlString);

                Response.End();
            }
            else
            {
                var data = HtmlConverter.HtmlToPdf(htmlString, new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Portrait,
                    Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                    MarginLeft = 22,
                    MarginTop = 22,
                    HeaderSpacing = 7
                });

                Response.SendFile(Current.SessionIdentifier.ToString(), "pdf", data);
            }
        }
    }
}