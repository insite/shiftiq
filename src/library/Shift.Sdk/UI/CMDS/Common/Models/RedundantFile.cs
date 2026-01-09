using System;

namespace Shift.Sdk.UI
{
    public class RedundantFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public string Organization { get; set; }
        public DateTimeOffset Uploaded { get; set; }
        public Guid Uploader { get; set; }
    }
}