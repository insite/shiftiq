using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using InSite.Application.Integrations.Prometric;
using InSite.Application.Registrations.Read;
using InSite.Domain.Integrations.Prometric;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Integration.Prometric
{
    public class PrometricApi : IPrometricApi
    {
        private readonly GeeParser _parser = new GeeParser();

        private readonly Func<Guid> _user;
        private readonly Func<Guid> _organization;

        private readonly IApiRequestLogger _logger;

        public PrometricOptions Prometric { get; set; }

        public PrometricApi(
            EnvironmentName environment,
            PrometricEnvironments prometricEnvironments,
            Func<Guid> organization,
            Func<Guid> user,
            IApiRequestLogger logger)
        {
            _user = user;
            _organization = organization;

            _logger = logger;

            Prometric = environment == EnvironmentName.Production
                ? prometricEnvironments.Production
                : prometricEnvironments.Test;
        }

        public bool EligibilityExists(GetEligibilityInput input)
            => GetEligibility(input).Success;

        public bool EligibilityExists(SaveEligibilityInput input)
            => GetEligibility(CreateGetEligibilityInput(input)).Success;

        public bool EligibilityExists(QRegistration registration)
            => GetEligibility(registration).Success;

        public PrometricEndpoint GetPrometricEndpoint()
        {
            var org = OrganizationSearch.Select(_organization());
            return org?.Integrations?.Prometric;
        }

        public string GetToken()
        {
            var prometric = GetPrometricEndpoint();
            var prometricUser = prometric?.UserName;
            var prometricPassword = prometric?.Password;

            if (prometricUser == null || prometricPassword == null)
                throw new Exception("Prometric API credentials not found.");

            var data = new Dictionary<string, string>
            {
                { "strSystemID", prometricUser },
                { "strPassword", prometricPassword }
            };

            var response = HttpPost(_user(), _organization(), $"{Prometric.ApiUrl}/GetToken", data);

            return _parser.ParseGetTokenOutput(response.Content);
        }

        public GetEligibilityOutput GetEligibility(GetEligibilityInput input)
        {
            var token = GetToken();

            var prometric = GetPrometricEndpoint();
            var prometricUser = prometric?.UserName;

            var data = new Dictionary<string, string>
            {
                { "strToken", token },
                { "strSRClientCode", prometricUser },
                { "strClientEligibilityCode", input.ClientEligibilityCode },
                { "strSRProgramCode", input.SRProgramCode },
                { "strSRExamCode", input.SRExamCode},
                { "strSRAppointmentID", input.SRAppointmentID}
            };

            var response = HttpPost(_user(), _organization(), $"{Prometric.ApiUrl}/RequestInfo", data);

            var xml = HttpUtility.HtmlDecode(response.Content);
            return _parser.ParseGetEligibilityOutput(xml);
        }

        public SaveEligibilityOutput SaveEligibility(SaveEligibilityInput input)
            => EligibilityExists(input) ? UpdateEligibility(input) : InsertEligibility(input);

        public SaveEligibilityOutput InsertEligibility(SaveEligibilityInput eligibility)
        {
            eligibility.Action = "add";
            return StoreEligibility(eligibility);
        }

        public SaveEligibilityOutput UpdateEligibility(SaveEligibilityInput eligibility)
        {
            eligibility.Action = "update";
            return StoreEligibility(eligibility);
        }

        public SaveEligibilityOutput DeleteEligibility(SaveEligibilityInput eligibility)
        {
            if (!EligibilityExists(eligibility))
                return new SaveEligibilityOutput { Code = 0, Message = "Eligibility Record Not Found" };

            eligibility.Action = "delete";
            return StoreEligibility(eligibility);
        }

        private SaveEligibilityOutput StoreEligibility(SaveEligibilityInput eligibility)
        {
            var token = GetToken();

            var prometric = GetPrometricEndpoint();
            var prometricUser = prometric?.UserName;

            eligibility.Client = prometricUser;

            var data = new Dictionary<string, string>
            {
                { "strToken", token },
                { "strXELIG", _parser.Base64Encode(eligibility) }
            };

            var response = HttpPost(_user(), _organization(), $"{Prometric.ApiUrl}/SubmitXELIG", data);

            var xml = HttpUtility.HtmlDecode(response.Content);
            return _parser.ParseSaveEligibilityOutput(xml);
        }

        public GetEligibilityOutput GetEligibility(QRegistration registration)
            => GetEligibility(CreateGetEligibilityInput(registration));

        public SaveEligibilityOutput SaveEligibility(QRegistration registration)
            => SaveEligibility(CreateSaveEligibilityInput(registration));

        public SaveEligibilityOutput DeleteEligibility(QRegistration registration)
            => DeleteEligibility(CreateSaveEligibilityInput(registration));

        #region API Helpers

        async Task<GeeResponse> HttpPostAsync(Guid user, Guid organization, string apiUrl, Dictionary<string, string> data)
        {
            var json = JsonConvert.SerializeObject(data);
            var key = LogRequest(user, organization, "POST", apiUrl, json);

            var content = new FormUrlEncodedContent(data);
            var http = await StaticHttpClient.Client.PostAsync(apiUrl, content);
            var result = await http.Content.ReadAsStringAsync();
            var status = (int)http.StatusCode;

            LogResponse(key, http, result);

            return new GeeResponse(status, result);
        }

        GeeResponse HttpPost(Guid user, Guid organization, string apiUrl, Dictionary<string, string> data)
        {
            var json = JsonConvert.SerializeObject(data);
            var key = LogRequest(user, organization, "POST", apiUrl, json);

            var content = new FormUrlEncodedContent(data);

            var http = Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.PostAsync, apiUrl, content);

            var result = Shift.Common.TaskRunner.RunSync(http.Content.ReadAsStringAsync);
            var status = (int)http.StatusCode;

            LogResponse(key, http, result);

            return new GeeResponse(status, result);
        }

        private Guid LogRequest(Guid user, Guid organization, string method, string url, string content)
        {
            try { return _logger.Insert(user, organization, null, method, url, content); }
            catch { }
            return Guid.Empty;
        }

        private void LogResponse(Guid key, HttpResponseMessage response, string content)
        {
            try { _logger.Update(key, new IntegrationResponse(response, content)); }
            catch { }
        }

        #endregion

        #region Database Helpers

        private GetEligibilityInput CreateGetEligibilityInput(SaveEligibilityInput save)
        {
            var prometric = GetPrometricEndpoint();

            var get = new GetEligibilityInput();

            get.SRClientCode = prometric.ClientCode;
            get.SRProgramCode = prometric.ClientCode;

            get.ClientEligibilityCode = save.LearnerCode;
            get.SRExamCode = $"{save.AssessmentFormCode}";

            return get;
        }

        private static readonly Regex AccommodationTypePattern = new Regex("(?<Name>.+?)(?<Note>\\(.+?\\))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private SaveEligibilityInput CreateSaveEligibilityInput(QRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var registrationId = registration.RegistrationIdentifier;

            if (registration.Candidate?.User == null)
                throw new Exception($"Registration {registrationId} is not assigned to a learner.");

            if (registration.ExamFormIdentifier == null)
                throw new Exception($"Registration {registrationId} is not assigned to an assessment form.");

            var prometric = GetPrometricEndpoint();

            var input = new SaveEligibilityInput
            {
                Client = prometric.ClientCode,
                AssessmentFormProgram = prometric.ClientCode,

                LearnerFirstName = registration.Candidate.User.UserFirstName,
                LearnerLastName = registration.Candidate.User.UserLastName,

                LearnerCode = registration.Candidate.PersonCode,
                LearnerEmail = registration.Candidate.User.UserEmail,

                LearnerIdentifier = registration.Candidate.User.UserIdentifier,

                ExamEventPassword = registration.RegistrationPassword,

                AssessmentFormCode = registration.Form?.FormCode,
                AssessmentFormIdentifier = registration.ExamFormIdentifier.Value
            };

            var timeLimit = (decimal?)registration.Form.FormTimeLimit;

            input.Accommodations = registration.Accommodations
                .EmptyIfNull()
                .OrderBy(x => x.AccommodationType)
                .Select(x =>
                {
                    var nameMatch = AccommodationTypePattern.Match(x.AccommodationType);

                    var name = nameMatch.Success ? nameMatch.Groups["Name"].Value.Trim() : x.AccommodationType;

                    var multiplier = -1m;

                    if (timeLimit.HasValue && timeLimit.Value != 0 && (x.TimeExtension ?? 0) > 0)
                        multiplier = 1m + (x.TimeExtension.Value / timeLimit.Value);

                    return new SaveEligibilityAccommodationItem
                    {
                        Name = "Exam " + name,
                        Multiplier = multiplier
                    };
                })
                .ToArray();

            return input;
        }

        private GetEligibilityInput CreateGetEligibilityInput(QRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            var registrationId = registration.RegistrationIdentifier;

            if (registration.Candidate?.User == null)
                throw new Exception($"Registration {registrationId} is not assigned to a learner.");

            if (registration.ExamFormIdentifier == null)
                throw new Exception($"Registration {registrationId} is not assigned to an assessment form.");

            var prometric = GetPrometricEndpoint();

            var input = new GetEligibilityInput();

            input.SRClientCode = prometric.ClientCode;
            input.SRProgramCode = prometric.ClientCode;

            input.ClientEligibilityCode = registration.Candidate.PersonCode;
            input.SRExamCode = $"{registration.Form.FormCode}";

            return input;
        }

        #endregion
    }
}