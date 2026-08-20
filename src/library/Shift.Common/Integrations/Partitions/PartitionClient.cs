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
        // Registration is idempotent, so a 503 from the Hub is safe to repeat. The Hub answers 503
        // only when another registration for the same partition holds the lock, which clears in
        // well under a second. Attempts and delays are kept small on purpose: Register() runs
        // synchronously inside Application_Start, so every retry holds up the app coming online.

        private const int MaxAttempts = 3;

        private static readonly int[] RetryDelaysMilliseconds = { 500, 1500 };

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
            var url = $"{GetApiRoot()}partitions";

            var json = JsonConvert.SerializeObject(partition);

            for (var attempt = 1; ; attempt++)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    AddApiKey(request);

                    var result = await StaticHttpClient.Client.SendAsync(request);

                    if (HttpStatusCode.OK == result.StatusCode)
                        return;

                    var body = await ReadBodyAsync(result);

                    var busy = HttpStatusCode.ServiceUnavailable == result.StatusCode;

                    var retryable = busy && attempt < MaxAttempts;

                    if (!retryable)
                        throw new InvalidOperationException(
                            $"Partition registration failed after {attempt} of {MaxAttempts} attempts: the Hub API returned HTTP {(int)result.StatusCode} {result.StatusCode} ({result.ReasonPhrase}). {body}");
                }

                await Task.Delay(RetryDelaysMilliseconds[attempt - 1]);
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
            var url = $"{GetApiRoot()}partitions";

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

        // Engine:Api:BaseUrl is the API root shared by every Engine client (Google, Premailer,
        // ImageMagick, partitions), and each client appends only its own segment. The mount point
        // belongs to the URL, not the client: https://dev-hub.shiftiq.com/ for a hub at a host
        // root, https://test.cmds.app/api/ for one mounted as an IIS child application. This
        // client used to append an extra api/ segment of its own, which forced the servers to
        // answer at api/partitions and (behind an /api mount) api/api/partitions.

        private string GetApiRoot()
        {
            var baseUrl = _api?.BaseUrl;

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("Cannot register the partition: Engine.Api.BaseUrl is not configured.");

            return baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
        }
    }
}
