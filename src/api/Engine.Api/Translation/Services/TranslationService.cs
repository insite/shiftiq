using Engine.Api.Internal;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

using Newtonsoft.Json;

namespace Engine.Api.Translation
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string fromText, string fromLanguage, string toLanguage);
        Task<string> TranslateAsync(string fromText, string fromLanguage, string toLanguage, Guid id);

    }
    public class TranslationService : ITranslationService
    {
        private readonly TranslationSearch _search;
        private readonly TranslationStore _store;
        private readonly HashSet<string> _languagesSupported;
        private readonly GoogleServiceAccount _accountObj;
        private readonly string _accountJson;

        public TranslationService(ISqlDatabase db, GoogleSettings google, DateTimeOffset? expiry = null)
        {
            _search = new TranslationSearch(db);
            _store = new TranslationStore(db, expiry);
            _languagesSupported = google.Translation.Languages.ToHashSet(StringComparer.OrdinalIgnoreCase);
            _accountObj = google.Translation.Account;
            _accountJson = JsonConvert.SerializeObject(_accountObj);
        }

        public bool IsLanguageSupported(string language) =>
            _languagesSupported.Contains(language);

        public async Task<string> TranslateAsync(string fromText, string fromLanguage, string toLanguage)
            => await TranslateAsync(fromText, fromLanguage, toLanguage, Guid.NewGuid());

        public async Task<string> TranslateAsync(string fromText, string fromLanguage, string toLanguage, Guid id)
        {
            if (!IsLanguageSupported(toLanguage))
                throw new LanguageNotSupportedException(toLanguage);

            var previous = await _search.GetAsync(fromText, fromLanguage, toLanguage);
            if (previous != null)
                return previous;

            var client = await new TranslationServiceClientBuilder { JsonCredentials = _accountJson }.BuildAsync();
            var request = new TranslateTextRequest
            {
                SourceLanguageCode = fromLanguage,
                Contents = { fromText },
                MimeType = "text/plain",
                TargetLanguageCode = toLanguage,
                Parent = new ProjectName(_accountObj.ProjectId).ToString(),
            };

            var response = await client.TranslateTextAsync(request);
            var translation = response.Translations[0];
            var toText = translation.TranslatedText;

            await _store.SaveAsync(fromLanguage, fromText, toLanguage, toText, id);

            return toText;
        }
    }
}