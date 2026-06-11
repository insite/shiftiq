namespace Shift.Hub.Google
{
    public class TranslationSearch
    {
        private readonly ISqlDatabase _db;

        public TranslationSearch(ISqlDatabase db)
        {
            _db = db;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@TranslationIdentifier", id }
                };

                var translations = await _db.CountAsync("select count(*) from translation where translation_id = @TranslationIdentifier", parameters);
                return translations > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string fromText, string fromLanguage)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@FromText", fromText }
                };

                var translations = await _db.CountAsync($"select count(*) from translation where {fromLanguage} = @FromText", parameters);
                return translations > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetAsync(string fromText, string fromLanguage, string toLanguage)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@FromText", fromText }
                };

                var translations = await _db.SelectAsync<string>($"select {toLanguage} from translation where {fromLanguage} = @FromText", parameters);
                return translations.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
