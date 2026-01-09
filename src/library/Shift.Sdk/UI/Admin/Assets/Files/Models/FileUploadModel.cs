using System;
using System.Linq;

namespace Shift.Sdk.UI
{
    public class FileUploadModel
    {
        public string Label { get; set; }
        public string Input { get; set; }

        public string SelectedFileNames { get; set; }
        public string SelectButton { get; set; }
        public string SelectFiles { get; set; }
        public string UploadButton { get; set; }
        public string UploadProgress { get; set; }

        public string ContainerType { get; set; }
        public Guid ContainerIdentifier { get; set; }

        public int? MaxFileSize { get; set; }
        public string[] AllowedExtensions { get; set; }

        public string MaxFileSizeJS => MaxFileSize.HasValue ? MaxFileSize.ToString() : "null";

        // /\.(gif|jpg|jpeg|png)$/gi
        public string AllowedExtensionsJS => AllowedExtensions != null && AllowedExtensions.Length > 0
            ? $"/\\.({string.Join("|", AllowedExtensions.Select(x => x.Substring(1)))})$/gi"
            : "null";
    }
}