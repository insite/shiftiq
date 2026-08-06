using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentExamCriterion : MultilingualDictionary
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

        public ContentExamCriterion()
            : base()
        {

        }

        private ContentExamCriterion(ContentExamCriterion source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamCriterion Clone() => new ContentExamCriterion(this);

        public static new ContentExamCriterion Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamCriterion()
                : JsonConvert.DeserializeObject<ContentExamCriterion>(json);
        }

        #endregion
    }
}
