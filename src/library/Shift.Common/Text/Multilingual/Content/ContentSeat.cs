using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentSeat : MultilingualDictionary
    {
        public MultilingualString Title
        {
            get => AddOrGet(nameof(Title));
            set => this[nameof(Title)] = value;
        }

        public MultilingualString Summary
        {
            get => AddOrGet(nameof(Summary));
            set => this[nameof(Summary)] = value;
        }

        public MultilingualString Description
        {
            get => AddOrGet(nameof(Description));
            set => this[nameof(Description)] = value;
        }

        #region Construction

        public ContentSeat()
            : base()
        {

        }

        private ContentSeat(ContentSeat source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentSeat Clone() => new ContentSeat(this);

        public static new ContentSeat Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentSeat()
                : JsonConvert.DeserializeObject<ContentSeat>(json);
        }

        public string GetAgreementHtml()
        {
            var text = Get("Agreement")?.Default;

            if (string.IsNullOrEmpty(text))
                return "N/A";

            return Markdown.ToHtml(text);
        }

        #endregion
    }
}
