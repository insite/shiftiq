using System;
using System.Net.Http;

namespace Shift.Common
{
    public interface IHttpClientFactory
    {
        HttpClient Create();

        Uri GetBaseAddress();

        string GetSecret();

        string GetAuthorizationParameter();

        void SetAuthenticationHeader(string scheme, string parameter);

        void AddAuthorizationHeader(string name, string value);
    }
}