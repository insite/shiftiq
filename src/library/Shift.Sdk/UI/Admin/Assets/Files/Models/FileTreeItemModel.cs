using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class FileTreeItemModel
    {
        public string FileName { get; set; }
        public bool IsCollapsed { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRoot { get; set; }
        public List<FileTreeItemModel> Children { get; set; }
    }
}