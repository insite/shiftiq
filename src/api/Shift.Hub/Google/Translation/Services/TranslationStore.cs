namespace Shift.Hub.Google
{
    public class TranslationStore
    {
        private readonly ISqlDatabase _db;

        public TranslationStore(ISqlDatabase db, DateTimeOffset? expiry = null)
        {
            _db = db;
            Expiry = expiry;
        }

        public DateTimeOffset? Expiry { get; set; }

        public async Task SaveAsync(string fromLanguage, string fromText, string toLanguage, string toText, Guid id)
        {
            var parameters = new Dictionary<string, object?>
            {
                { "@FromText", fromText },
                { "@ToText", toText },
                { "@TimestampExpired", Expiry },
                { "@TranslationIdentifier", id }
            };

            var query = $@"
update translation set modified_at = GETUTCDATE(), expired_at = @TimestampExpired, {fromLanguage} = @FromText, {toLanguage} = @ToText where translation_id = @TranslationIdentifier;
if @@ROWCOUNT = 0
  begin
    update translation set modified_at = GETUTCDATE(), expired_at = @TimestampExpired, {toLanguage} = @ToText where {fromLanguage} = @FromText;
    if @@ROWCOUNT = 0
      begin
        insert into translation ( {fromLanguage}, {toLanguage}, translation_id, expired_at ) values ( @FromText, @ToText, @TranslationIdentifier, @TimestampExpired );
      end;
  end;
";

            await _db.ExecuteQueryAsync(query, parameters);
        }
    }
}
