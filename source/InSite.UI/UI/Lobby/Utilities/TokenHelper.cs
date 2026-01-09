using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby.Utilities
{
    public static class TokenHelper
    {
        public static string GenerateToken(JwtRequest request, string baseAddressUrl)
            => TaskRunner.RunSyncWithConfig(GenerateTokenAsync, false, request, baseAddressUrl);

        private static async Task<string> GenerateTokenAsync(JwtRequest request, string baseAddressUrl)
        {
            var uri = new Uri(baseAddressUrl);
            var baseAddress = new Uri($"{uri.Scheme}://{uri.Host}");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/token", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseBody = ServiceLocator.Serializer.Deserialize<JwtResponse>(responseContent);
                    return responseBody.AccessToken;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string errorDetails = TaskRunner.RunSync(response.Content.ReadAsStringAsync);
                    ServiceLocator.Logger.Information($"Bad Request: {errorDetails}");
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string errorDetails = TaskRunner.RunSync(response.Content.ReadAsStringAsync);
                    ServiceLocator.Logger.Information($"Unauthorized: {errorDetails}");
                    return null;
                }
                else
                {
                    ServiceLocator.Logger.Information($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
        }

        public static QPersonSecret GetClientSecret(Guid personId, bool refresh, int? lifetimeInMinutes = 20)
        {
            var type = SecretType.Authentication;

            var name = SecretName.ShiftClientSecret;

            RemoveDuplicateClientSecrets(name);

            var secret = ServiceLocator.PersonSecretSearch.GetByPerson(personId, name);

            if (secret != null)
            {
                if (!refresh)
                    return secret;

                var remove = new RemovePersonSecret(personId, secret.SecretIdentifier);

                ServiceLocator.SendCommand(remove);
            }

            var secretId = UniqueIdentifier.Create();

            if (lifetimeInMinutes == null)
                lifetimeInMinutes = 129600; // 129,600 minutes = 36 hours

            var value = Secret.CreateValue();

            var expiry = DateTimeOffset.Now.AddMonths(3);

            var add = new AddPersonSecret(personId, secretId, type, name, value, expiry, lifetimeInMinutes);

            ServiceLocator.SendCommand(add);

            secret = ServiceLocator.PersonSecretSearch.GetSecret(secretId);

            return secret;
        }

        /// <summary>
        /// Delete any duplicate client secrets to avoid confusion
        /// </summary>
        /// <param name="name"></param>
        private static void RemoveDuplicateClientSecrets(string name)
        {
            var secrets = ServiceLocator.PersonSecretSearch.Bind(x => x, new QPersonSecretFilter());

            var duplicates = secrets
                .OrderByDescending(x => x.SecretExpiry)
                .GroupBy(x => new { x.SecretName, x.PersonIdentifier })
                .Where(group => group.Count() > 1);

            foreach (var group in duplicates)
            {
                // Keep the most recent and delete the rest

                var secretsToDelete = group.Skip(1);

                foreach (var secret in secretsToDelete)
                {
                    var remove = new RemovePersonSecret(group.Key.PersonIdentifier, secret.SecretIdentifier);

                    ServiceLocator.SendCommand(remove);
                }
            }
        }

        public static string DecodeReturnBackUrl(string returnBackUrl)
        {
            try
            {
                string base64String = returnBackUrl.Replace('-', '+').Replace('_', '/').Replace('~', '=');

                switch (base64String.Length % 4)
                {
                    case 2: base64String += "=="; break;
                    case 3: base64String += "="; break;
                }

                if (!IsBase64String(base64String))
                    throw new FormatException("The input is not a valid Base-64 string.");

                byte[] byteArray = Convert.FromBase64String(base64String);
                return Encoding.UTF8.GetString(byteArray);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static bool IsBase64String(string base64)
        {
            return base64.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=');
        }
    }
}