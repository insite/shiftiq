using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public class DirectAccessClient : IDirectAccessClient
    {
        private readonly VariantSkilledTradesBC _servers;
        private readonly IApiRequestLogger _logger;
        private readonly IJsonSerializer _serializer;

        public DirectAccessClient(EnvironmentName environment, VariantSkilledTradesBC servers, IApiRequestLogger logger, IJsonSerializer serializer)
        {
            if (servers == null || servers.DirectAccess == null || servers.IntegrationHub == null)
                throw new ArgumentNullException(nameof(servers), "Server configuration settings are required for Direct Access and Integration Hub.");

            if (!servers.DirectAccess.IsValid() && !servers.IntegrationHub.IsValid())
                throw new ArgumentNullException(nameof(servers), "Valid configuration settings must be provided for Direct Access and/or Integration Hub.");

            _servers = servers;
            _logger = logger;
            _serializer = serializer;
        }

        #region IDirectAccessServer (implementation)

        public void VerifyActiveIndividual(Guid user, VerificationInputVariables inputs, VerificationDisplayVariables displays)
        {
            var server = ConfigureServers($"individual/verify/{inputs.CandidateCode}").First();

            var response = Shift.Common.TaskRunner.RunSync(HttpGetAsync, user, server);

            var output = int.TryParse(inputs.CandidateCode, out var code)
                ? JsonConvert.DeserializeObject<VerifyActiveIndividualOutput>(response.Content)
                : null;

            inputs.CandidateStatus = output != null && output.Verified ? "Verified" : "Not Verified";
            displays.CandidateStatusReason = output?.Reason;
        }

        public IndividualRequestOutput IndividualRequest(Guid user, IndividualRequestInput input)
        {
            var server = ConfigureServers($"individual/search").First();

            try
            {
                var response = Shift.Common.TaskRunner.RunSync(HttpPostAsync, user, server, input.Serialize());

                return IndividualRequestOutput.Deserialize(response.Content);
            }
            catch (Exception)
            {
                return new IndividualRequestOutput();
            }
        }

        public void VerifyCorrespondingRegistration(Guid user, VerificationInputVariables inputs, VerificationDisplayVariables displays)
        {
            var input = new VerifyCorrespondingRegistrationInput(inputs.CandidateCode, inputs.FormCode);

            var output = VerifyCorrespondingRegistration(user, input);

            inputs.TradeStatus = output.Verified ? "Verified" : "Not Verified";

            displays.TradeStatusReason = output.Reason;
        }

        private VerifyCorrespondingRegistrationOutput VerifyCorrespondingRegistration(Guid user, VerifyCorrespondingRegistrationInput input)
        {
            var server = ConfigureServers($"individual/verifyprogramreg/{input.IndividualId}/{input.ExamId}").First();

            var response = Shift.Common.TaskRunner.RunSync(HttpGetAsync, user, server);

            return VerifyCorrespondingRegistrationOutput.Deserialize(response.Content);
        }

        public ExamSubmissionResponse SubmitExamData(Guid user, ExamSubmissionRequest input)
        {
            ExamSubmissionResponse response = null;

            var servers = ConfigureServers($"exam");

            foreach (var server in servers)
            {
                var result = Shift.Common.TaskRunner.RunSync(HttpPostAsync, user, server, input.Serialize());

                if (response == null)
                    response = ExamSubmissionResponse.Deserialize(result.Content);
            }

            return response;
        }

        public ExamEventCandidateOutput ExamEventCandidate(Guid user, string eventId, string individualId, ExamEventCandidateInput input)
        {
            ExamEventCandidateOutput output = null;

            if (int.TryParse(eventId, out var e))
                input.EventNumber = e;

            if (int.TryParse(individualId, out var i))
                input.IndividualNumber = i;

            var servers = ConfigureServers($"exam/eventcandidate");

            foreach (var server in servers)
            {
                var response = Shift.Common.TaskRunner.RunSync(HttpPostAsync, user, server, input.Serialize());

                if (output == null)
                    output = response?.Content != null ? ExamEventCandidateOutput.Deserialize(response.Content) : null;
            }

            return output;
        }

        public ExamEventOutput ExamEvent(Guid user, int eventId, ExamEventInput input)
        {
            ExamEventOutput output = null;

            var servers = ConfigureServers($"exam/event");

            foreach (var server in servers)
            {
                var response = Shift.Common.TaskRunner.RunSync(HttpPostAsync, user, server, input.Serialize());

                if (output == null)
                    output = ExamEventOutput.Deserialize(response.Content);
            }

            return output;
        }

        public AdHocEventNotificationOutput AdHocEventNotification(Guid user, string eventId, AdHocEventNotificationInput input)
        {
            AdHocEventNotificationOutput output = null;

            var servers = ConfigureServers($"exam/eventnotification");

            foreach (var server in servers)
            {
                var response = Shift.Common.TaskRunner.RunSync(HttpPostAsync, user, server, input.Serialize());

                if (output == null)
                    output = AdHocEventNotificationOutput.Deserialize(response.Content);
            }

            return output;
        }

        #endregion

        #region Helpers

        async Task<DirectAccessApiResponse> HttpGetAsync(Guid user, DirectAccessServer api)
        {
            var request = CreateRequest(HttpMethod.Get, api, null);

            var key = LogRequest(user, request.Headers, "GET", api.ApiUrl, null);

            var http = await StaticHttpClient.Client.SendAsync(request);
            var content = await http.Content.ReadAsStringAsync();
            var status = (int)http.StatusCode;

            LogResponse(key, http, content);

            return CreateApiResponse(status, content);
        }

        async Task<DirectAccessApiResponse> HttpPostAsync(Guid user, DirectAccessServer api, string json)
        {
            DirectAccessApiResponse response = null;

            try
            {
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var request = CreateRequest(HttpMethod.Post, api, data);

                var key = LogRequest(user, request.Headers, "POST", api.ApiUrl, json);

                var http = await StaticHttpClient.Client.SendAsync(request);
                var content = await http.Content.ReadAsStringAsync();
                var status = (int)http.StatusCode;

                LogResponse(key, http, content);

                response = CreateApiResponse(status, content);
            }
            catch (Exception ex)
            {
                // An exception that occurs due to task cancellation can be ignored.
                var ignore = ex is TaskCanceledException || (ex.InnerException != null && ex.InnerException is TaskCanceledException);
                if (!ignore)
                    throw;
            }

            return response;
        }

        private Guid LogRequest(Guid user, HttpRequestHeaders headers, string method, string url, string content)
        {
            try
            {
                return _logger.Insert(user, OrganizationIdentifiers.SkilledTradesBC, headers, method, url, content);
            }
            catch { }
            return Guid.Empty;
        }

        private void LogResponse(Guid key, HttpResponseMessage response, string content)
        {
            try
            {
                _logger.Update(key, new IntegrationResponse(response, content));
            }
            catch { }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, DirectAccessServer api, HttpContent content)
        {
            var request = new HttpRequestMessage(method, api.ApiUrl);

            if (content != null)
                request.Content = content;

            var value = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{api.UserName}:{api.Password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", value);

            return request;
        }

        private List<DirectAccessServer> ConfigureServers(string endpoint)
        {
            var da = _servers.DirectAccess;
            var ih = _servers.IntegrationHub;

            var servers = new List<DirectAccessServer>();

            if (da.IsValid())
                servers.Add(new DirectAccessServer
                {
                    ApiUrl = $"{da.ApiUrl}/{endpoint}",
                    UserName = da.UserName,
                    Password = da.Password
                });

            if (ih.IsValid())
                servers.Add(new DirectAccessServer
                {
                    ApiUrl = $"{ih.ApiUrl}/{endpoint}",
                    UserName = ih.UserName,
                    Password = ih.Password
                });

            return servers;
        }

        private DirectAccessApiResponse CreateApiResponse(int status, string content)
        {
            var apiResponse = new DirectAccessApiResponse
            {
                Status = status,
                Content = content
            };

            if (500 <= status)
                throw new Exception($"Direct Access API call failed with status code {status}: {content}");
            else if (480 <= status && status < 500)
                apiResponse.Error = _serializer.Deserialize<DirectAccessApiError>(content);
            else if (400 <= status && status < 480)
                throw new Exception($"Direct Access API call failed with status code {status}: {content}");

            return apiResponse;
        }

        #endregion
    }
}