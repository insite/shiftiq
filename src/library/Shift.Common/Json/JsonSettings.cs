using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;

namespace Shift.Common.Json
{
    public static class JsonSettings
    {
        public const string SchemaLicenseKey = "5476-PLkMZedjxmqEfmNNUppRMt6AiVTJZ3nT9QHqo7oXAfsu2ZlT1gB6U7bVopJpOeeifbIUfCNzDyjBko4z7Tkyu1uQpDFMErLfnL5u/QNScNxRuTFry7L+uQLDuyTjvk+wlzKUims/Dpc801CaIMXO/Ty1pSmwmuUf9zwryNora3Z7IklkIjo1NDc2LCJFeHBpcnlEYXRlIjoiMjAyNS0xMC0xN1QwMDoyMjoyOS4xMDY3MjI3WiIsIlR5cGUiOiJKc29uU2NoZW1hSW5kaWUifQ==";

        public static JsonSerializerSettings Default()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore // SMELLS: This is dangerous, but I am afraid to touch it
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        public static void Register()
        {
            JsonConvert.DefaultSettings = Default;

            License.RegisterLicense(SchemaLicenseKey);
        }
    }
}
