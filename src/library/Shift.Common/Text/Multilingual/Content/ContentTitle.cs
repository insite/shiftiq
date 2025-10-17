using System;

using Newtonsoft.Json;

namespace Shift.Common
{
    [Serializable]
    public class ContentTitle : MultilingualDictionary
    {
        public MultilingualString Title
        {
            get => AddOrGet(nameof(Title));
            set => this[nameof(Title)] = value;
        }

        #region Construction

        public ContentTitle()
            : base()
        {

        }

        private ContentTitle(ContentTitle source)
            : base(source)
        {

        }

        #endregion

        #region Methods (helpers)

        public new ContentTitle Clone() => new ContentTitle(this);

        public static new ContentTitle Deserialize(string json)
        {
            return string.IsNullOrEmpty(json)
                ? new ContentTitle()
                : JsonConvert.DeserializeObject<ContentTitle>(json);
        }

        public static string GetJson(string title)
        {
            var content = new ContentTitle();
            content.Title.Default = title;
            return content.Serialize();
        }

        public static string GetJson(string json, string title)
        {
            var content = Deserialize(json);
            content.Title.Default = title;
            return content.Serialize();
        }

        public static string GetJson(string json, string title, string language)
        {
            var content = Deserialize(json);
            content.Title[language] = title;
            return content.Serialize();
        }

        #endregion
    }
}
