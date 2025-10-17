using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Common
{
    public class JwtClient
    {
        private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IJsonSerializer _serializer;

        private readonly IJwtEncoder _encoder;

        private string _token = null;

        public JwtClient(IHttpClientFactory httpClientFactory, IJsonSerializer jsonSerializer, IJwtEncoder jwtEncoder)
        {
            _httpClientFactory = httpClientFactory;

            _serializer = jsonSerializer;

            _encoder = jwtEncoder;
        }

        public async Task<string> NewTokenAsync(Uri jwtServer, JwtRequest jwtRequest)
        {
            _token = null;

            return await GetTokenAsync(jwtServer, jwtRequest);
        }

        public async Task<string> GetTokenAsync(Uri jwtServer, JwtRequest jwtRequest)
        {
            if (ValidateCachedToken())
                return _token;

            return await GenerateNewToken(jwtServer, jwtRequest);
        }

        private async Task<string> GenerateNewToken(Uri jwtServer, JwtRequest jwtRequest)
        {
            _token = string.Empty;
            try
            {
                await Lock.WaitAsync();

                if (jwtRequest.Lifetime == null || jwtRequest.Lifetime <= 0)
                    jwtRequest.Lifetime = JwtRequest.DefaultLifetime;

                var jwtServerUrl = jwtServer.AbsoluteUri;

                var requestData = _serializer.Serialize(jwtRequest);
                var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");

                using (var httpClient = _httpClientFactory.Create())
                {
                    var responseMessage = await httpClient.PostAsync(jwtServerUrl, requestContent);
                    var responseContent = await responseMessage.Content.ReadAsStringAsync();

                    var status = responseMessage.StatusCode;
                    if (status != HttpStatusCode.OK)
                    {
                        var error = $"{jwtServerUrl} responded with HTTP {(int)status} {status}. {responseContent}";
                        throw new ApiException(error);
                    }

                    _token = responseContent;
                }
            }
            finally
            {
                Lock.Release();
            }
            return _token;
        }

        /// <remarks>
        /// Client-side validation of a JWT is simple: ensure the JWT can be decoded into a valid 
        /// object, and confirm it is not yet expired. Additional JWT validation (e.g., signature 
        /// verification) can be done on the server.
        /// </remarks>
        private bool ValidateCachedToken()
        {
            if (_token.IsEmpty())
                return false;

            try
            {
                var jwt = _encoder.Decode(_token);

                return !jwt.IsExpired();
            }
            catch
            {
                return false;
            }
        }
    }
}