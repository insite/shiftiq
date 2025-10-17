using System;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Shift.Constant;

namespace Shift.Common
{
    public class SmsSender
    {
        public const string OkResponse = "Message queued successfully";

        private class Response
        {
            public string SendMessageWithReferenceResult { get; set; }
        }

        private Guid _user;
        private Guid _organization;
        private IApiRequestLogger _apiRequestLogger;

        public SmsSender(Guid user, Guid organization, IApiRequestLogger apiRequestLogger)
        {
            _user = user;
            _organization = organization;
            _apiRequestLogger = apiRequestLogger;
        }

        public string Send(string phoneNumber, string message)
        {
            var trimmedPhoneNumber = new string(phoneNumber.Where(x => char.IsDigit(x)).ToArray());

            var body = new { MessageBody = message };
            var bodyJson = JsonConvert.SerializeObject(body);

            var client = new IntegrationClient(HttpVerb.POST, IntegrationType.SmsGateway, _user, _organization, _apiRequestLogger);
            client.ResponseEncoding = Encoding.UTF8;

            var responseText = client.RequestString(trimmedPhoneNumber, bodyJson);

            try
            {
                var response = JsonConvert.DeserializeObject<Response>(responseText);
                return response.SendMessageWithReferenceResult;
            }
            catch
            {
                return responseText;
            }
        }
    }
}