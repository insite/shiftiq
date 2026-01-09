namespace Engine.Api.Translation
{
    public class TTranslation
    {
        public Guid TranslationIdentifier { get; set; }

        public string? ar { get; set; }
        public string? de { get; set; }
        public string? en { get; set; }
        public string? eo { get; set; }
        public string? es { get; set; }
        public string? fr { get; set; }
        public string? he { get; set; }
        public string? it { get; set; }
        public string? ja { get; set; }
        public string? ko { get; set; }
        public string? la { get; set; }
        public string? nl { get; set; }
        public string? no { get; set; }
        public string? pa { get; set; }
        public string? pl { get; set; }
        public string? pt { get; set; }
        public string? ru { get; set; }
        public string? sv { get; set; }
        public string? uk { get; set; }
        public string? zh { get; set; }

        public DateTimeOffset TimestampCreated { get; set; }
        public DateTimeOffset? TimestampExpired { get; set; }
        public DateTimeOffset TimestampModified { get; set; }
    }
}
