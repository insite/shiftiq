using System;
using System.Data.Entity;

namespace InSite.Persistence
{
    public static class JsonFunctions
    {
        [DbFunction("dbo", "JSON_VALUE")]
        public static string JsonValue(string expression, string path)
        {
            throw new NotSupportedException();
        }
    }
}
