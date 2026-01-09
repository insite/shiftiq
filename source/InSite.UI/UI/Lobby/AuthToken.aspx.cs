using System;
using System.Net.Http;
using System.Text;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Lobby
{
    public partial class AuthToken : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TestCase1.Click += TestValidationInAPI_Click;
        }

        private void TestValidationInAPI_Click(object sender, EventArgs e)
        {
            var uri = new Uri(TestUrl.Text);

            var client = new HttpClient();

            TaskRunner.RunSync(async () =>
            {
                var data = TestInput.Text;

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var http = await client.PostAsync(uri, content);

                var output = await http.Content.ReadAsStringAsync();

                TestOutput.Text = output;

                if (!http.IsSuccessStatusCode)
                {
                    TestSuccess.Visible = false;
                    TestFailure.Visible = true;
                    TestDetail.InnerHtml = output;
                }
                else
                {
                    TestSuccess.InnerText = $"HTTP {http.StatusCode} Success";
                    TestSuccess.Visible = true;
                    TestFailure.Visible = false;
                    TestDetail.InnerHtml = $"{uri} successfully validated the input token.";
                }
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var hostUrl = ServiceLocator.AppSettings.Shift.Api.Hosting.V2.BaseUrl.TrimEnd('/');

            TestUrl.Text = $"{hostUrl}/security/cookies/validate";

            var cookieSettings = ServiceLocator.AppSettings.Security.Cookie;

            CookieName.Text = cookieSettings.Name;

            var cookie = Request.Cookies[cookieSettings.Name];

            if (cookie == null)
            {
                SerializedCookieValue.Text = "(null)";
                DeserializedFailure.Visible = true;
            }
            else
            {
                var encrypt = ServiceLocator.AppSettings.Environment.Name != EnvironmentName.Local;
                var secret = ServiceLocator.AppSettings.Application.EncryptionKey;

                SerializedCookieValue.Text = cookie.Value;
                var encoder = new CookieTokenEncoder();
                var token = encoder.Deserialize(cookie.Value, encrypt, secret);
                DeserializedCookieValue.Text = JsonConvert.SerializeObject(token, Formatting.Indented);
                DeserializedSuccess.Visible = true;
                DeserializedDetail.InnerHtml = $"Cookie {(cookie.Expires != DateTime.MinValue && cookie.Expires < DateTime.UtcNow ? " expires " + cookie.Expires.ToString() : " is expired")} | Domain {cookie.Domain} | Path {cookie.Path}";
            }
        }
    }
}