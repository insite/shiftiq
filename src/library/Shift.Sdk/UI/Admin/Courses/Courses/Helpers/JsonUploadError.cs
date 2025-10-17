using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    internal class JsonUploadError : IJsonUploadResult
    {
        public JsonUploadError()
        {
            Type = "ERROR";
            Messages = new List<string>();
        }

        public List<string> Messages { get; }
        public string Type { get; }
    }
}
