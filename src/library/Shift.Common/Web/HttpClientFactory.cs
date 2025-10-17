using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Shift.Common
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly Uri _baseAddress;

        private readonly string _secret;

        private string _authorizationScheme = "Bearer";

        private string _authorizationParameter;

        private Dictionary<string, string> _authorizationHeaders = new Dictionary<string, string>();

        public HttpClientFactory(Uri baseAddress, string secret)
        {
            _baseAddress = baseAddress;

            _secret = secret;
        }

        public HttpClient Create()
        {
            var client = new HttpClient()
            {
                BaseAddress = _baseAddress
            };

            if (_authorizationParameter.IsNotEmpty())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authorizationScheme, _authorizationParameter);
            }

            foreach (var header in _authorizationHeaders)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

            return client;
        }

        public Uri GetBaseAddress()
        {
            return _baseAddress;
        }

        public string GetSecret()
        {
            return _secret;
        }

        public string GetAuthorizationParameter()
        {
            return _authorizationParameter;
        }

        public void SetAuthenticationHeader(string scheme, string parameter)
        {
            _authorizationScheme = scheme;
            _authorizationParameter = parameter;
        }

        public void AddAuthorizationHeader(string name, string value)
        {
            if (!_authorizationHeaders.ContainsKey(name))
                _authorizationHeaders.Add(name, value);
        }
    }
}