using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using InSite.Domain.Integration;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Integration
{
    public class ApiRequestLogger : IApiRequestLogger
    {
        private static readonly List<string> _buffer = new List<string>();

        private readonly Action<string, string> _error;

        public ApiRequestLogger(Action<string, string> error)
        {
            _error = error;
        }

        public void Error(string error, string source)
        {
            _error?.Invoke(error, source);
        }

        public Guid Insert(Guid? userIdentifier, Guid? organizationIdentifier, HttpWebRequest request, string content)
        {
            var apiRequest = new ApiRequest { RequestStarted = DateTimeOffset.Now };

            apiRequest.RequestIdentifier = UniqueIdentifier.Create();
            apiRequest.RequestStatus = "Proxy Started";
            apiRequest.RequestDirection = "Out";
            apiRequest.ValidationStatus = "Proxy Valid";
            apiRequest.UserIdentifier = userIdentifier;
            apiRequest.OrganizationIdentifier = organizationIdentifier;

            // Extract properties from the request for the log entry.
            apiRequest.RequestUri = request.RequestUri.ToString();
            apiRequest.RequestMethod = request.Method;

            if (request.Headers != null)
                apiRequest.RequestHeaders = request.Headers.ToString();

            apiRequest.RequestContentData = content;
            apiRequest.RequestContentType = request.ContentType;

            ApiRequestStore.Insert(apiRequest);

            return apiRequest.RequestIdentifier;
        }

        public Guid Insert(Guid? userIdentifier, Guid? organizationIdentifier, HttpRequestHeaders headers, string requestMethod, string requestUrl, string content)
        {
            var apiRequest = new ApiRequest { RequestStarted = DateTimeOffset.Now };

            apiRequest.RequestIdentifier = UniqueIdentifier.Create();
            apiRequest.RequestStatus = "Proxy Started";
            apiRequest.RequestDirection = "Out";
            apiRequest.ValidationStatus = "Proxy Valid";
            apiRequest.UserIdentifier = userIdentifier;
            apiRequest.OrganizationIdentifier = organizationIdentifier;

            // Extract properties from the request for the log entry.
            apiRequest.RequestUri = requestUrl;
            apiRequest.RequestMethod = requestMethod;

            if (headers != null)
                apiRequest.RequestHeaders = JsonConvert.SerializeObject(headers);

            apiRequest.RequestContentData = content;

            ApiRequestStore.Insert(apiRequest);

            return apiRequest.RequestIdentifier;
        }

        public async Task<Guid> InsertAsync(ApiDeveloper developer, Guid? organization, HttpRequestMessage request, string direction, string status, string ipAddress, Guid? responseLogId)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var method = request.Method ?? throw new ArgumentNullException("request.Method");
            var requestUri = request.RequestUri ?? throw new ArgumentNullException("request.RequestUri");

            var apiRequest = new ApiRequest
            {
                RequestDirection = direction,
                RequestIdentifier = UniqueIdentifier.Create(),
                RequestMethod = method.ToString(),
                RequestStarted = DateTimeOffset.Now,
                RequestStatus = "Requested",
                RequestUri = requestUri.ToString(),

                ValidationStatus = status,

                DeveloperEmail = developer?.Email,
                DeveloperName = developer?.Name,
                DeveloperHostAddress = ipAddress,

                UserIdentifier = developer?.User?.UserIdentifier,
                OrganizationIdentifier = organization,
                ResponseLogIdentifier = responseLogId
            };

            if (request.Headers != null)
                apiRequest.RequestHeaders = JsonConvert.SerializeObject(request.Headers);

            if (request.Content != null)
                apiRequest.RequestContentData = await request.Content.ReadAsStringAsync();

            await ApiRequestStore.InsertAsync(apiRequest);

            return apiRequest.RequestIdentifier;
        }

        public void Update(Guid requestKey, IntegrationResponse response)
        {
            var apiRequest = ApiRequestSearch.Select(requestKey);

            apiRequest.ResponseCompleted = DateTimeOffset.Now;
            apiRequest.ResponseTime = (int)(apiRequest.ResponseCompleted.Value - apiRequest.RequestStarted).TotalMilliseconds;
            apiRequest.RequestStatus = "Proxy Completed";
            apiRequest.ResponseContentType = response.ContentType;
            apiRequest.ResponseContentData = response.Content;
            apiRequest.ResponseStatusName = response.StatusDescription;
            apiRequest.ResponseStatusNumber = (int)response.StatusCode;

            ApiRequestStore.Update(apiRequest);
        }

        public void Update(Guid requestKey, string destination, Exception ex)
        {
            var errors = CsvConverter.ListToStringList(ex.GetMessages(), ". ");

            var apiRequest = ApiRequestSearch.Select(requestKey);
            apiRequest.RequestStatus = "Proxy Failed";
            apiRequest.ExecutionErrors = errors;

            ApiRequestStore.Update(apiRequest);

            var source = typeof(IntegrationClient).FullName + "." + nameof(Update);
            var error = $"An unexpected error occurred for this HTTP request ({destination}).\r\n" + errors;

            if (!IsSubmitted(source, error))
                Error(error, source);
        }

        /// <summary>
        /// Returns true if the same source and message have been submitted before. It is OK if this resets when the application restarts. The 
        /// purpose here is only to decrease the number of duplicate log entries submitted to Sentry; some duplicates are acceptable.
        /// </summary>
        private static bool IsSubmitted(string source, string error)
        {
            lock (_buffer)
            {
                if (_buffer.Contains(source + error))
                    return true;

                _buffer.Add(source + error);
            }

            return false;
        }
    }
}
