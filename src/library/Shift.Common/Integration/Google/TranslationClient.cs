using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Shift.Common.Integration.Google
{
    public interface ITranslationClient
    {
        void Translate(string from, string to, IEnumerable<MultilingualString> values);
        void Translate(string from, string to, MultilingualString value);
        string Translate(string from, string to, string value);
        string[] Translate(string from, string to, string[] inputs);
        Task<string[]> TranslateAsync(string from, string to, string[] inputs);
    }

    public class TranslationClient : ITranslationClient
    {
        private readonly ApiSettings _settings;

        public TranslationClient(EngineSettings engine)
        {
            _settings = engine.Api.Google;
        }

        public void Translate(string from, string to, IEnumerable<MultilingualString> values)
        {
            var results = Translate(from, to, values.Select(x => x[from]).ToArray());
            var index = 0;
            foreach (var value in values)
                value[to] = results[index++];
        }

        public void Translate(string from, string to, MultilingualString value)
        {
            value[to] = Translate(from, to, value[from]);
        }

        public string Translate(string from, string to, string value)
        {
            return Translate(from, to, new[] { value })[0];
        }

        public string[] Translate(string from, string to, string[] inputs)
        {
            return Shift.Common.TaskRunner.RunSync(TranslateAsync, from, to, inputs);
        }

        public async Task<string[]> TranslateAsync(string from, string to, string[] inputs)
        {
            string baseUrl = _settings.BaseUrl;

            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            var url = $"{baseUrl}{Endpoints.Translation.Translate}?from={from}&to={to}";

            var content = new StringContent(JsonConvert.SerializeObject(inputs), Encoding.UTF8, "application/json");

            var postResult = await StaticHttpClient.Client.PostAsync(url, content);

            if (HttpStatusCode.OK != postResult.StatusCode)
                throw new Exception($"Translation Failed: The Engine API returned HTTP {postResult.StatusCode}. {postResult.ReasonPhrase}");

            var apiResponseContent = await postResult.Content.ReadAsStringAsync();

            var outputs = JsonConvert.DeserializeObject<string[]>(apiResponseContent);

            for (var i = 0; i < outputs.Length; i++)
            {
                var text = outputs[i];
                outputs[i] = !string.IsNullOrEmpty(text) ? text.Replace("! [", "![").Replace("] (", "](") : text;
            }

            if (inputs.Length != outputs.Length)
                throw new Exception($"Translation Failed: The input array contains {inputs.Length} items, but the output array contains {outputs.Length} items.");

            return outputs;
        }
    }
}