using InSite.Domain.Organizations;

using static InSite.UI.Layout.Lobby.Controls.SignInBasePage;

namespace InSite.Web.SignIn
{
    public static partial class SignInLogic
    {
        public sealed class GetUserOrganization_Result
        {
            public OrganizationState Organization { get; set; }
            public (SignInErrorCodes Code, bool HtmlEncode, string Message) Error { get; set; }
            public string RedirectUrl { get; set; }
        }
    }
}