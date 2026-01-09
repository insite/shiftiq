using Newtonsoft.Json;

namespace Shift.Common
{
    public class ContentEventClass : MultilingualDictionary
    {
        public MultilingualString Title
        {
            get => AddOrGet(nameof(Title));
            set => this[nameof(Title)] = value;
        }

        public MultilingualString Description
        {
            get => AddOrGet(nameof(Description));
            set => this[nameof(Description)] = value;
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

        public MultilingualString ClassLink
        {
            get => AddOrGet(nameof(ClassLink));
            set => this[nameof(ClassLink)] = value;
        }

        #region Construction

        public ContentEventClass()
            : base()
        {

        }

        private ContentEventClass(ContentEventClass source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentEventClass Clone() => new ContentEventClass(this);

        public static new ContentEventClass Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentEventClass()
                : JsonConvert.DeserializeObject<ContentEventClass>(json);
        }

        #endregion
    }
}
