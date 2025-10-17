using System;
using System.Net.Http;

using Shift.Common;

namespace Shift.Common.Scorm
{
    public class ScormClientFactory : IHttpClientFactory
    {
        public Uri ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ScormClientFactory(string apiUrl, string user, string password)
        {
            ApiUrl = new Uri(apiUrl);
            UserName = user;
            Password = password;
        }

        public HttpClient Create()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("UserName", UserName);
            client.DefaultRequestHeaders.Add("Password", Password);
            client.BaseAddress = ApiUrl;
            return client;
        }

        public Uri GetBaseAddress()
        {
            return ApiUrl;
        }

        public string GetSecret()
        {
            return Password;
        }

        public string GetAuthorizationParameter()
        {
            throw new NotImplementedException();
        }

        public void SetAuthenticationHeader(string scheme, string parameter)
        {
            throw new NotImplementedException();
        }

        public void AddAuthorizationHeader(string name, string value)
        {
            throw new NotImplementedException();
        }
    }
}