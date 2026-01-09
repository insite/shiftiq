using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentExamSpecification : MultilingualDictionary
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

        public MultilingualString Introduction
        {
            get => AddOrGet(nameof(Introduction));
            set => this[nameof(Introduction)] = value;
        }

        public MultilingualString Conclusion
        {
            get => AddOrGet(nameof(Conclusion));
            set => this[nameof(Conclusion)] = value;
        }

        #region Construction

        public ContentExamSpecification()
                : base()
        {

        }

        private ContentExamSpecification(ContentExamSpecification source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamSpecification Clone() => new ContentExamSpecification(this);

        public static new ContentExamSpecification Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamSpecification()
                : JsonConvert.DeserializeObject<ContentExamSpecification>(json);
        }

        #endregion
    }
}
