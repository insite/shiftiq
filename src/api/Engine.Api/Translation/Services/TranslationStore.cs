using Engine.Api.Internal;

namespace Engine.Api.Translation
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
update content.TTranslation set TimestampModified = GETUTCDATE(), TimestampExpired = @TimestampExpired, {fromLanguage} = @FromText, {toLanguage} = @ToText where TranslationIdentifier = @TranslationIdentifier;
if @@ROWCOUNT = 0
  begin
    update content.TTranslation set TimestampModified = GETUTCDATE(), TimestampExpired = @TimestampExpired, {toLanguage} = @ToText where {fromLanguage} = @FromText;
    if @@ROWCOUNT = 0
      begin
        insert into content.TTranslation ( {fromLanguage}, {toLanguage}, TranslationIdentifier, TimestampExpired ) values ( @FromText, @ToText, @TranslationIdentifier, @TimestampExpired );
      end;
  end;
";

            await _db.ExecuteQueryAsync(query, parameters);
        }
    }
}