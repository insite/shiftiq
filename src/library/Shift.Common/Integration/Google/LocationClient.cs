using System;
using System.Net;
using System.Threading;

using Newtonsoft.Json;

namespace Shift.Common.Integration.Google
{
    public class LocationClient
    {
        public Country[] CollectCountries(string host)
        {
            return GetData<Country>(host, Endpoints.Location.Countries);
        }

        public Province[] CollectProvinces(string host, string country)
        {
            var endpoint = Endpoints.Location.Provinces.Replace("{country}", country);
            return GetData<Province>(host, endpoint);
        }

        private string BuildEndpointUrl(string host, string path)
        {
            if (!host.EndsWith("/"))
                host += "/";

            if (path.StartsWith("/"))
                path = path.Substring(1);

            return host + path;
        }

        private T[] GetData<T>(string host, string endpointUrl)
        {
            return Get<T[]>(host, endpointUrl);
        }

        private T Get<T>(string host, string endpoint, int retryCount = 3, int retryDelayMs = 500)
        {
            Exception lastException = null;

            var url = BuildEndpointUrl(host, endpoint);

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                try
                {
                    var response = Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.GetAsync, url);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = Shift.Common.TaskRunner.RunSync(response.Content.ReadAsStringAsync);

                        return JsonConvert.DeserializeObject<T>(content);
                    }

                    var warning = $"Attempt {attempt}: API call to {url} returned status code {response.StatusCode}";

                    System.Diagnostics.Trace.TraceWarning(warning);
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    var error = $"Attempt {attempt}: API call to {url} threw exception: {ex.Message}";

                    System.Diagnostics.Trace.TraceError(error);
                }

                if (attempt < retryCount)
                    Thread.Sleep(retryDelayMs);
            }

            var fatal = $"API call to {url} failed after {retryCount} attempts.";

            throw new ApplicationException(fatal, lastException);
        }
    }
}