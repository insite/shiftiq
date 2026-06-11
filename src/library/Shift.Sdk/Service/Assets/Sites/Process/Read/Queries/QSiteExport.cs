using System;
using System.Collections.Generic;

using Shift.Common;
namespace InSite.Application.Sites.Read
{
    [Serializable]
    public class QSiteExport
    {
        public string Name { get; set; }
        public string Title { get; set; }

        public ContentContainer Content { get; set; }
        public List<QPageExport> Pages { get; set; }
    }
}