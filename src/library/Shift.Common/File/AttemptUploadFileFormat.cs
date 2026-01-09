using System;
using System.Collections.Generic;

namespace Shift.Common.File
{
    public class AttemptUploadFileFormat
    {
        public string ID { get; }
        public string Title { get; }
        public string UploadLabel { get; }
        public string TypeLabel { get; }
        public AttemptUploadFileType[] TypeItems { get; }
        public ICollection<string> Extensions { get; }
        public AttemptUploadFileParser.ParseFunc Parse { get; }
        
        public AttemptUploadFileFormat(string id, string title, string uploadLabel, string typeLabel, AttemptUploadFileType[] typeItems, string[] exts, AttemptUploadFileParser.ParseFunc parser)
        {
            ID = id;
            Title = title;
            UploadLabel = uploadLabel;
            TypeLabel = typeLabel;
            TypeItems = typeItems;
            Extensions = new HashSet<string>(exts, StringComparer.OrdinalIgnoreCase);
            Parse = parser;
        }
    }
}
