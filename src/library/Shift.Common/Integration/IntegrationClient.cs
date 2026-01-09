using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using Shift.Constant;

namespace Shift.Common
{
    public class IntegrationClient
    {
        private const int DefaultRequestTimeoutMilliseconds = 15 * 60 * 1000;

        public static EnvironmentName Environment { get; set; }

        private static Dictionary<Guid, OrganizationIntegrations> _organizationIntegrations;

        private readonly IntegrationType _integrationType;
        private readonly Guid? _userIdentifier;
        private readonly Guid _organizationIdentifier;
        private readonly IApiRequestLogger _logger;

        public HttpVerb Method { get; set; }
        public string ContentType { get; set; } = "application/json";
        public int RequestTimeoutMilliseconds { get; set; } = DefaultRequestTimeoutMilliseconds;
        public Encoding RequestEncoding { get; set; } = Encoding.UTF8;
        public Encoding ResponseEncoding { get; set; } = Encoding.ASCII;

        public static void Init(
            EnvironmentName environment,
            Dictionary<Guid, OrganizationIntegrations> organizationIntegrations
            )
        {
            Environment = environment;
            _organizationIntegrations = organizationIntegrations;
        }

        public IntegrationClient(
            HttpVerb method,
            IntegrationType type,
            Guid? userIdentifier,
            Guid organizationIdentifier,
            IApiRequestLogger logger
            )
        {
            Method = method;

            _integrationType = type;
            _userIdentifier = userIdentifier;
            _organizationIdentifier = organizationIdentifier;
            _logger = logger;
        }

        public IntegrationResponse Request(string appUrl, string postData, string postDataToSave = null)
        {
            var requestTuple = CreateWebRequest(appUrl, postData);
            var request = requestTuple.Item1;
            var requestContentToSave = postDataToSave ?? requestTuple.Item2;

            var requestKey = _logger.Insert(_userIdentifier, _organizationIdentifier, request, requestContentToSave);

            IntegrationResponse integrationResponse;

            try
            {
                using (var response = request.GetResponseNoException())
                {
                    string responseContent = null;

                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (var streamReader = new StreamReader(responseStream, ResponseEncoding))
                            responseContent = streamReader.ReadToEnd();
                    }

                    integrationResponse = new IntegrationResponse(response, responseContent);
                }
            }
            catch (Exception ex)
            {
                var destination = GetEndpointPath(request.RequestUri);
                _logger.Update(requestKey, destination, ex);
                throw;
            }

            _logger.Update(requestKey, integrationResponse);

            if (string.IsNullOrEmpty(integrationResponse.Content))
                throw new WebException("Failed to get a response for the HTTP request");

            return integrationResponse;
        }

        public string RequestString(string appUrl, string postData = null)
        {
            return Request(appUrl, postData).Content;
        }

        private Tuple<HttpWebRequest, string> CreateWebRequest(string appUrl, string postData)
        {
            var baseEndpoint = GetBaseEndpoint();

            // Create an HTTP web request to the web service method
            var requestUri = GetRequestUrl(appUrl, baseEndpoint);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            SetHeaders(baseEndpoint, request);

            request.Timeout = RequestTimeoutMilliseconds;
            request.Method = Method.ToString();
            request.ContentType = ContentType;

            var content = Method != HttpVerb.GET
                ? CreateContent(postData, baseEndpoint, request)
                : null;

            return new Tuple<HttpWebRequest, string>(request, content);
        }

        private void SetHeaders(BaseEndpoint baseEndpoint, HttpWebRequest request)
        {
            request.Headers.Clear();

            var headers = baseEndpoint.GetHeaders();
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers.Add(header.Name, header.Value);
            }
        }

        private string CreateContent(string postData, BaseEndpoint baseEndpoint, HttpWebRequest request)
        {
            var content = postData;

            var parameters = baseEndpoint.GetParameters();
            if (parameters.IsNotEmpty() && string.Equals(ContentType, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                var result = new StringBuilder(content);

                foreach (var param in parameters)
                {
                    if (result.Length > 0)
                        result.Append("&");

                    result.Append(param.Name);
                    result.Append("=");
                    result.Append(HttpUtility.UrlEncode(param.Value));
                }

                content = result.ToString();
            }

            if (string.IsNullOrEmpty(content))
                return null;

            var bytes = RequestEncoding.GetBytes(content);

            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
                requestStream.Write(bytes, 0, bytes.Length);

            return content;
        }

        private Uri GetRequestUrl(string appUrl, BaseEndpoint baseEndpoint)
        {
            string appPath, appQuery;

            if (appUrl.Contains("?"))
            {
                var parts = appUrl.Split(new char[] { '?' });
                appPath = parts[0];
                appQuery = parts[1];
            }
            else
            {
                appPath = appUrl;
                appQuery = null;
            }

            if (appPath.StartsWith("/"))
                appPath = appPath.Substring(1);

            var requestPath = !string.IsNullOrEmpty(appPath)
                ? (baseEndpoint?.Endpoint ?? string.Empty) + @"/" + appPath
                : (baseEndpoint?.Endpoint ?? string.Empty);

            var requestQuery = GetRequestQuery(baseEndpoint, appQuery);

            var builder = new UriBuilder
            {
                Scheme = "https",
                Host = baseEndpoint.HostName,
                Port = 443,
                Path = requestPath,
                Query = requestQuery
            };


            return builder.Uri;
        }

        private string GetRequestQuery(BaseEndpoint baseEndpoint, string appQuery)
        {
            if (string.Equals(ContentType, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                return appQuery;

            var parameters = baseEndpoint.GetParameters();

            if (parameters.IsEmpty())
                return appQuery;

            var result = new StringBuilder();

            foreach (var param in parameters)
            {
                if (result.Length > 0)
                    result.Append("&");

                result.Append(param.Name);
                result.Append("=");
                result.Append(HttpUtility.UrlEncode(param.Value));
            }

            if (!string.IsNullOrEmpty(appQuery))
            {
                result.Append("&");
                result.Append(appQuery);
            }

            return result.ToString();
        }

        private BaseEndpoint GetBaseEndpoint()
        {
            var endpoint = GetBaseEndpoint(_organizationIdentifier) ?? GetBaseEndpoint(OrganizationIdentifiers.Global);

            if (endpoint == null)
                throw new Exception($"API Endpoint Not Found: Environment = {Environment}; Organization = {_organizationIdentifier}; Integration = {_integrationType} ");

            return endpoint;

            BaseEndpoint GetBaseEndpoint(Guid exactOrganization)
            {
                if (!_organizationIntegrations.TryGetValue(exactOrganization, out var integrations))
                    return null;

                switch (_integrationType)
                {
                    case IntegrationType.Bambora:
                        return integrations.Bambora?.Get(Environment);
                    case IntegrationType.Recaptcha:
                        return integrations.Recaptcha?.Get(Environment);
                    case IntegrationType.BCMail:
                        return integrations.BCMail?.Get(Environment);
                }

                return null;
            }
        }

        private string GetEndpointPath(Uri uri)
            => $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }
}
