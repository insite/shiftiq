using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Integrations.Tests.Utilities
{
    public class D365Response
    {
        public string Status { get; private set; }
        public string Body { get; private set; }

        private static string GetApiClientSecret()
        {
            var identity = CurrentSessionState.Identity;

            var userId = identity.User.Identifier;

            var organizationId = identity.Organization.Identifier;

            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId);

            if (person == null)
                throw new Exception($"Person not found for user {userId} in organization {organizationId}");

            var personId = person.PersonIdentifier;

            var secret = ServiceLocator.PersonSecretSearch.GetByPerson(personId, SecretName.ShiftClientSecret)?.SecretValue;

            if (secret == null)
                throw new Exception($"API Client Secret not found for person {personId}");

            return secret;
        }

        private D365Response() { }

        public static D365Response Get(string url, string method, string contentType, string body)
        {
            if (url.IsEmpty())
                return new D365Response
                {
                    Status = "N/A",
                    Body = "N/A"
                };

            var secret = GetApiClientSecret();

            url = "https://" + HttpContext.Current.Request.Url.Host + url;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 60 * 1000;
            request.Method = method;
            request.ContentType = contentType;
            request.Headers.Add("Authorization", $"Bearer {secret}");

            if (body.IsNotEmpty())
            {
                var bytes = Encoding.UTF8.GetBytes(body);
                request.ContentLength = bytes.Length;
                using (var requestStream = request.GetRequestStream())
                    requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = request.GetResponseNoException())
                {
                    var result = new D365Response
                    {
                        Status = $"{(int)response.StatusCode} {response.StatusCode.GetStatusDescription()}"
                    };

                    using (var stream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(stream))
                            result.Body = streamReader.ReadToEnd();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new D365Response
                {
                    Status = "ERROR",
                    Body = ex.ToString()
                };
            }
        }
    }
}