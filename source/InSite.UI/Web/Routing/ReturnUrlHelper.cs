namespace InSite
{
    public static class ReturnUrlHelper
    {
        public static string GetRedirectUrl(string redirectUrl, string returnQuery = null)
        {
            return new ReturnUrl().GetRedirectUrl(redirectUrl, returnQuery);
        }
    }
}