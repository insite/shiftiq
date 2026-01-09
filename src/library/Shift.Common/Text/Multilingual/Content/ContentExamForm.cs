using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentExamForm : MultilingualDictionary
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

        public MultilingualString Introduction
        {
            get => AddOrGet(nameof(Introduction));
            set => this[nameof(Introduction)] = value;
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

        public ContentExamForm()
            : base()
        {

        }

        private ContentExamForm(ContentExamForm source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentExamForm Clone() => new ContentExamForm(this);

        public static new ContentExamForm Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentExamForm()
                : JsonConvert.DeserializeObject<ContentExamForm>(json);
        }

        #endregion
    }
}
