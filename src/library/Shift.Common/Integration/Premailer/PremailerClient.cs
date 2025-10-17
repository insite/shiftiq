using System.Net.Http;
using System.Text;

using Shift.Common;

namespace Shift.Common.Integration.Premailer
{
    public class PremailerClient
    {
        private readonly ApiSettings _settings;

        public PremailerClient(ApiSettings settings)
        {
            _settings = settings;
        }

        public string MoveCssInline(string htmlContent)
        {
            string baseUrl = _settings.BaseUrl;

            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            var url = $"{baseUrl}{Endpoints.MoveCssInline}";

            var content = new StringContent(htmlContent, Encoding.UTF8, "plain/text");

            var postResult = TaskRunner.RunSync(StaticHttpClient.Client.PostAsync, url, content);

            postResult.EnsureSuccessStatusCode();

            return TaskRunner.RunSync(postResult.Content.ReadAsStringAsync);
        }
    }
}