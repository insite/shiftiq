using System;
using System.Data.Entity;

namespace InSite.Persistence
{
    public static class CoreFunctions
    {
        [DbFunction("contents", "GetContentHtmlEn")]
        public static string GetContentHtmlEn(Guid id, string label) { throw new NotSupportedException("Direct calls are not supported."); }

        [DbFunction("contents", "GetContentText")]
        public static string GetContentText(Guid id, string label, string language) { throw new NotSupportedException("Direct calls are not supported."); }

        [DbFunction("contents", "GetContentTextEn")]
        public static string GetContentTextEn(Guid id, string label) { throw new NotSupportedException("Direct calls are not supported."); }

        [DbFunction("contents", "IsContentContains")]
        public static bool IsContentContains(Guid id, string label, string language, string keyword) { throw new NotSupportedException("Direct calls are not supported."); }

        [DbFunction("contents", "IsContentTranslated")]
        public static bool IsContentTranslated(Guid id, string label) { throw new NotSupportedException("Direct calls are not supported."); }
    }
}
