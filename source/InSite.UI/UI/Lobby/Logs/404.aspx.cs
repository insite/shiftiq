using System;
using System.Data.Entity.Core.Objects;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.UI.Lobby.Controls;

using Shift.Common;

namespace InSite.UI.Lobby.Logs
{
    public partial class _404 : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ErrorBody.ContentCreated += ErrorBody_ContentCreated;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Request.IsAuthenticated)
                Server.Transfer("~/UI/Lobby/Logs/410.aspx");

            Response.StatusCode = 404;

            base.OnLoad(e);
        }

        private void ErrorBody_ContentCreated(object sender, ErrorPageBody.ContentEventArgs e)
        {
            var heading = (HtmlGenericControl)e.Container.FindControl("ErrorHeading");
            heading.InnerHtml = "Page Not Found";

            var body = (HtmlGenericControl)e.Container.FindControl("ErrorBody");
            body.InnerHtml = "It seems we can't find the page you are looking for.";

            var path = Request.QueryString["path"];
            if (path.IsEmpty())
                return;

            path = QueryStringSanitizer.SanitizeQueryString(path);

            if (path.IsEmpty())
            {
                Response.StatusCode = 400;
                return;
            }

            var href = (Literal)e.Container.FindControl("ErrorHref");
            href.Text = "<p class='pb-1 text-danger'><i class='fas fa-unlink me-1'></i> " + path + "</p>";
        }
    }

    public class QueryStringSanitizer
    {
        /// <summary>
        /// Sanitizes a query string parameter to ensure it is safe for use.
        /// </summary>
        /// <param name="input">The raw query string parameter value.</param>
        /// <returns>The sanitized query string parameter value.</returns>
        public static string SanitizeQueryString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Remove dangerous characters
            string sanitized = Regex.Replace(input, @"[<>""'%;()&+]", string.Empty);

            // Replace any encoded dangerous characters
            sanitized = Regex.Replace(sanitized, @"%[0-9a-fA-F]{2}", string.Empty);

            // Trim whitespace and ensure it's not overly long
            sanitized = sanitized.Trim();
            if (sanitized.Length > 256)
            {
                sanitized = sanitized.Substring(0, 256);
            }

            return IsValidInput(sanitized) ? sanitized : null;
        }

        /// <summary>
        /// Validates the input against a custom rule (e.g., alphanumeric only).
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>True if valid; otherwise false.</returns>
        private static bool IsValidInput(string input)
        {
            // Example: Allow only alphanumeric and dashes/underscores
            return Regex.IsMatch(input, @"^[a-zA-Z0-9/_-]*$");
        }
    }
}