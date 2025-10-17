using System;

using Newtonsoft.Json;

namespace Shift.Common
{
    [Serializable]
    public class ContentExamQuestion : MultilingualDictionary
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

        public MultilingualString Rationale
        {
            get => AddOrGet(nameof(Rationale));
            set => this[nameof(Rationale)] = value;
        }

        public MultilingualString RationaleOnCorrectAnswer
        {
            get => AddOrGet(nameof(RationaleOnCorrectAnswer));
            set => this[nameof(RationaleOnCorrectAnswer)] = value;
        }

        public MultilingualString RationaleOnIncorrectAnswer
        {
            get => AddOrGet(nameof(RationaleOnIncorrectAnswer));
            set => this[nameof(RationaleOnIncorrectAnswer)] = value;
        }

        public MultilingualString Description
        {
            get => AddOrGet(nameof(Description));
            set => this[nameof(Description)] = value;
        }

        public MultilingualString Exemplar
        {
            get => AddOrGet(nameof(Exemplar));
            set => this[nameof(Exemplar)] = value;
        }

        #region Construction

        public ContentExamQuestion()
            : base()
        {

        }

        private ContentExamQuestion(ContentExamQuestion source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamQuestion Clone() => new ContentExamQuestion(this);

        public static new ContentExamQuestion Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamQuestion()
                : JsonConvert.DeserializeObject<ContentExamQuestion>(json);
        }

        #endregion
    }
}
