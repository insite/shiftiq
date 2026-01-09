using System;
using System.Linq;

namespace Shift.Common
{
    public class SwiftSmsGatewayClient
    {
        public const string OkResponse = "Message queued successfully";

        private class Response
        {
            public string SendMessageWithReferenceResult { get; set; }
        }

        private readonly ApiClientSynchronous _client;

        public SwiftSmsGatewayClient(IHttpClientFactory factory, IJsonSerializerBase serializer)
        {
            _client = new ApiClientSynchronous(factory, serializer);
        }

        public string Send(string phoneNumber, string message)
        {
            var trimmedPhoneNumber = new string(phoneNumber.Where(x => char.IsDigit(x)).ToArray());

            var body = new { MessageBody = message };

            var result = _client.HttpPost<Response>(trimmedPhoneNumber, body);

            try
            {
                return result.Data.SendMessageWithReferenceResult;
            }
            catch (Exception ex)
            {
                var error = ex.GetFormattedMessages();

                return $"An unexpected error occurred sending message to {phoneNumber}: {error}";
            }
        }
    }
}