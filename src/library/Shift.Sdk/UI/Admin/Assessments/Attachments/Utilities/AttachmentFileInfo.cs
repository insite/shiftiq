using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class AttachmentFileInfo
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public int ContentLength { get; set; }
    }
}