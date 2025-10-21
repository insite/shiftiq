using Newtonsoft.Json;

namespace InSite.UI.Admin.Integrations.Tests.Utilities
{
    internal interface ID365Method
    {
        void InitMethod();
        string GetUrl();
        string GetBody(Formatting jsonFormatting);
        D365Response SendRequest();
    }
}
