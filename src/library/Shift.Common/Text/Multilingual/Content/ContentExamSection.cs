using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentExamSection : MultilingualDictionary
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

        #region Construction

        public ContentExamSection()
            : base()
        {

        }

        private ContentExamSection(ContentExamSection source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamSection Clone() => new ContentExamSection(this);

        public static new ContentExamSection Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamSection()
                : JsonConvert.DeserializeObject<ContentExamSection>(json);
        }

        #endregion
    }
}
