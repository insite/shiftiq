using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentExamBank : MultilingualDictionary
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

        public MultilingualString MaterialsForParticipation
        {
            get => AddOrGet(nameof(MaterialsForParticipation));
            set => this[nameof(MaterialsForParticipation)] = value;
        }

        public MultilingualString MaterialsForDistribution
        {
            get => AddOrGet(nameof(MaterialsForDistribution));
            set => this[nameof(MaterialsForDistribution)] = value;
        }

        public MultilingualString InstructionsForOnline
        {
            get => AddOrGet(nameof(InstructionsForOnline));
            set => this[nameof(InstructionsForOnline)] = value;
        }

        public MultilingualString InstructionsForPaper
        {
            get => AddOrGet(nameof(InstructionsForPaper));
            set => this[nameof(InstructionsForPaper)] = value;
        }

        #region Construction

        public ContentExamBank()
            : base()
        {

        }

        protected ContentExamBank(ContentExamBank source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamBank Clone() => new ContentExamBank(this);

        public static new ContentExamBank Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamBank()
                : JsonConvert.DeserializeObject<ContentExamBank>(json);
        }

        #endregion
    }
}
