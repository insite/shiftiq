using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class PageCreated : Change
    {
        public Guid? Site { get; set; }
        public Guid Tenant { get; set; }
        public Guid Author { get; set; }
        public Guid? ParentPage { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsHidden { get; set; }
        public bool IsNewTab { get; set; }

        public int Sequence { get; set; }

        public PageCreated(
            Guid? site,
            Guid? parentPage,
            Guid tenant, 
            Guid author,
            string title,
            string type,
            int sequence,
            bool isHidden,
            bool isNewTab)
        {
            Site = site;
            ParentPage = parentPage;
            Tenant = tenant;
            Author = author;
            Title = title;
            Type = type;
            Sequence = sequence;
            IsHidden = isHidden;
            IsNewTab = isNewTab;
        }
    }
}
