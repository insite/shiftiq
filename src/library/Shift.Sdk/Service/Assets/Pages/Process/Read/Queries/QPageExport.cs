using System;
using System.Collections.Generic;

using Shift.Common;
namespace InSite.Application.Sites.Read
{
    [Serializable]
    public class QPageExport
    {
        public string PageType { get; set; }
        public string PageSlug { get; set; }
        public string Title { get; set; }
        public string ContentControl { get; set; }
        public string NavigateUrl { get; set; }
        public bool IsNewTab { get; set; }
        public string Site { get; set; }
        public bool IsHidden { get; set; }
        public string ContentLabels { get; set; }
        public string Icon { get; set; }
        public string Hook { get; set; }
        public string[] Groups { get; set; }

        public ContentContainer Content { get; set; }
        public List<QPageExport> Children { get; set; }
    }
}
