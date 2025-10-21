using System;
using System.Web;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin
{
    public partial class TestEquation : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                MarkdownText.Text = @"
**This is my test formula:** [eq]y=x^2+a\cdot\left(x+1\right)[eq],

and this is too: [eq]y=x^2[eq]
";

                MarkdownResult.Text = Markdown.ToHtml(MarkdownText.Text);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrintButton.Click += PrintButton_Click;
            RenderButton.Click += RenderButton_Click;
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            var requestUrl = HttpContext.Current.Request.Url;
            var url = $"{requestUrl.Scheme}://{requestUrl.Host}/UI/Admin/testequation.aspx";

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = PageOrientationType.Portrait,
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                MarginLeft = 22,
                MarginTop = 22,
                HeaderUrl = "",
                HeaderSpacing = 7,
            };

            var pdf = HtmlConverter.UrlToPdf(settings, url);

            Response.SendFile($"Test", "pdf", pdf);
        }

        private void RenderButton_Click(object sender, EventArgs e)
        {
            MarkdownResult.Text = Markdown.ToHtml(MarkdownText.Text);
        }
    }
}