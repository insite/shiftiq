using System.Diagnostics;
using System.Text;

using InSite.Application;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Integration.BCMail
{
    public class BCMailServer : IBCMailServer
    {
        public readonly object _mutex = new object();

        private string GetQuery(string request, bool isTest)
        {
            var test = isTest || _environment != EnvironmentName.Production;
            return $"JOB{(test ? "_TEST" : "")}/auth=itaex_distr;5717f5be2b10495a8655beb578e052a8/JSON/{request}";
        }

        private readonly EnvironmentName _environment;
        private readonly IIdentityService _identityService;
        private readonly IApiRequestLogger _apiRequestLogger;

        public BCMailServer(EnvironmentName environment, IIdentityService identityService, IApiRequestLogger apiRequestLogger)
        {
            _environment = environment;
            _identityService = identityService;
            _apiRequestLogger = apiRequestLogger;
        }

        public DistributionJob[] Status(string[] jobs, bool isTest)
        {
            var input = new DistributionStatusRequestInput { Jobs = jobs };

            var json = HttpPost(GetQuery("status", isTest), JsonConvert.SerializeObject(input));

            try
            {
                var output = JsonConvert.DeserializeObject<DistributionStatusRequestOutput>(json);
                return output.Jobs;
            }
            catch
            {
                return new DistributionJob[]
                {
                    new DistributionJob
                    {
                        Code = "Error",
                        Status = "Error",
                        Errors = json
                    }
                };
            }
        }

        public DistributionJob Create(DistributionRequest request, bool isTest)
        {
            lock (_mutex)
            {
                var input = new DistributionCreateRequestInput { Request = request };

                var json = HttpPost(GetQuery("create%3AITAEX-DISTR", isTest), input.Serialize());

                DistributionJob output;

                try
                {
                    output = JsonConvert.DeserializeObject<DistributionJob>(json);
                }
                catch
                {
                    output = new DistributionJob
                    {
                        Code = "Error",
                        Status = "Error",
                        Errors = json
                    };
                }

                return output;
            }
        }

        private string HttpPost(string query, string data)
        {
            var watch = new Stopwatch();
            watch.Start();

            var user = _identityService.GetCurrentUser();
            var organization = OrganizationIdentifiers.SkilledTradesBC;

            var hubClient = new IntegrationClient(HttpVerb.POST, IntegrationType.BCMail, user, organization, _apiRequestLogger);
            hubClient.RequestTimeoutMilliseconds = 500000;
            hubClient.RequestEncoding = Encoding.ASCII;

            return hubClient.Request(query, data).Content;
        }
    }
}