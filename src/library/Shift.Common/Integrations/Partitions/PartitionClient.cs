using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shift.Common.Integration.Partitions
{
    public class AccountRegistration
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public DateTimeOffset? OpenedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }

    public class OrganizationRegistration
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public Guid Identifier { get; set; }
        public string WebsiteUrl { get; set; }
        public string LogoUrl { get; set; }

        public AccountRegistration Account { get; set; } = new AccountRegistration();
    }

    public class PartitionRegistration
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Theme { get; set; }
        public string Domain { get; set; }
        public string Email { get; set; }
        public string Slug { get; set; }
        public Guid Identifier { get; set; }
        public List<string> Whitelist { get; set; } = new List<string>();
        public string HelpUrl { get; set; }
        public string LogoUrl { get; set; }

        public List<OrganizationRegistration> Organizations { get; set; } = new List<OrganizationRegistration>();
    }

    public interface IPartitionClient
    {
        Task RegisterAsync(PartitionRegistration partition);
        void Register(PartitionRegistration partition);
        Task<List<PartitionRegistration>> GetPartitionsAsync();
        List<PartitionRegistration> GetPartitions();
    }

    public class PartitionClient : IPartitionClient
    {
        private readonly EngineApiSettings _api;

        public PartitionClient(EngineSettings engine)
        {
            _api = engine.Api;
        }

        public void Register(PartitionRegistration partition)
        {
            TaskRunner.RunSync(() => RegisterAsync(partition));
        }

        public async Task RegisterAsync(PartitionRegistration partition)
        {
            var url = $"{GetHubRoot()}api/partitions";

            var json = JsonConvert.SerializeObject(partition);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                AddApiKey(request);

                var result = await StaticHttpClient.Client.SendAsync(request);

                if (HttpStatusCode.OK != result.StatusCode)
                {
                    var body = await ReadBodyAsync(result);
                    throw new InvalidOperationException(
                        $"Partition registration failed: the Hub API returned HTTP {(int)result.StatusCode} {result.StatusCode} ({result.ReasonPhrase}). {body}");
                }
            }
        }

        private static async Task<string> ReadBodyAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return string.Empty;

            try
            {
                var body = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(body) ? string.Empty : body.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<PartitionRegistration> GetPartitions()
        {
            return TaskRunner.RunSync(GetPartitionsAsync);
        }

        public async Task<List<PartitionRegistration>> GetPartitionsAsync()
        {
            var url = $"{GetHubRoot()}api/partitions";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                AddApiKey(request);

                var result = await StaticHttpClient.Client.SendAsync(request);

                if (HttpStatusCode.OK != result.StatusCode)
                    throw new InvalidOperationException($"Partition query failed: the Hub API returned HTTP {result.StatusCode}. {result.ReasonPhrase}");

                var json = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<PartitionRegistration>>(json) ?? new List<PartitionRegistration>();
            }
        }

        // The shared HttpClient is a process-wide singleton, so the key is set per-request rather
        // than on DefaultRequestHeaders to avoid leaking it onto unrelated callers.

        private void AddApiKey(HttpRequestMessage request)
        {
            var apiKey = _api?.ApiKey;

            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add("X-Api-Key", apiKey);
        }

        // The API endpoints to create and query partitions lives at the Engine (Hub) root, which is
        // exactly the single configured Engine API base URL.

        private string GetHubRoot()
        {
            var baseUrl = _api?.BaseUrl;

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("Cannot register the partition: Engine.Api.BaseUrl is not configured.");

            return baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
        }
    }
}
