using System.Net.Http;
using System.Text;

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

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(htmlContent, Encoding.UTF8, "plain/text")
            };

            HubApiKey.Apply(request, _settings.ApiKey);

            var postResult = TaskRunner.RunSync(() => StaticHttpClient.Client.SendAsync(request));

            postResult.EnsureSuccessStatusCode();

            return TaskRunner.RunSync(postResult.Content.ReadAsStringAsync);
        }
    }
}
