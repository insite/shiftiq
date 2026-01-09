using System;

using InSite.Persistence;

using Newtonsoft.Json;

namespace InSite.Api.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonFileModel : IFile
    {
        public DateTimeOffset Date { get; set; }
        public int Size { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name => FileModel.GetName(Path);

        [JsonProperty(PropertyName = "ext")]
        public string Extension => FileModel.GetExtension(Path);

        [JsonProperty(PropertyName = "date")]
        public string FormatedDate => Date.ToString("MMM d, yyyy");

        [JsonProperty(PropertyName = "size")]
        public string FormatedSize
        {
            get
            {
                const int BytesInMB = 1048576;
                const int BytesInKB = 1024;

                if (Size > BytesInMB)
                    return $"{(double)Size / BytesInMB:n1} MB";

                if (Size > BytesInKB)
                    return $"{(double)Size / BytesInKB:n1} KB";

                return $"{Size:n0} B";
            }
        }

        public string Path { get; set; }
    }
}