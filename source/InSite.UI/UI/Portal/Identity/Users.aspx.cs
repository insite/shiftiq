using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Desktops.Design.Users
{
    // Retired route. User administration is now served by the consolidated person search at
    // /ui/portal/contacts/people/search. This shell issues a permanent redirect, preserving any
    // query string, so legacy links and saved deep links continue to work. The .aspx shell is
    // retained so the route continues to resolve. The redirect does not request a scope, so the
    // consolidated page picks its default like any other visit: the caller's own contacts when
    // they have some, otherwise the full organization where the E03 partition allows it.
    public partial class Search : SearchPage<PersonFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            Response.RedirectPermanent(
                "/ui/portal/contacts/people/search"
                + Request.Url.Query);
        }
    }
}
