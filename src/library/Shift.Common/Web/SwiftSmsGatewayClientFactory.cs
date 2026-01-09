using System;
using System.Net.Http;

namespace Shift.Common
{
    public class SwiftSmsGatewayClientFactory : IHttpClientFactory
    {
        private readonly Uri _baseAddress;

        public SwiftSmsGatewayClientFactory(Uri baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public void AddAuthorizationHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public HttpClient Create()
        {
            var client = new HttpClient()
            {
                BaseAddress = _baseAddress
            };

            return client;
        }

        public string GetAuthorizationParameter()
        {
            throw new NotImplementedException();
        }

        public Uri GetBaseAddress()
        {
            return _baseAddress;
        }

        public string GetSecret()
        {
            throw new NotImplementedException();
        }

        public void SetAuthenticationHeader(string scheme, string parameter)
        {
            throw new NotImplementedException();
        }
    }
}