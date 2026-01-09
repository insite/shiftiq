using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class CreatePage : Command
    {
        public Guid? Site { get; set; }
        public Guid Tenant { get; set; }
        public Guid Author { get; set; }
        public Guid? ParentPage { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }

        public bool IsHidden { get; set; }
        public bool IsNewTab { get; set; }

        public int Sequence { get; set; }

        public CreatePage(Guid page, Guid? site, Guid? parentPage, Guid tenant, Guid author, string title, string type, int sequence, bool isHidden, bool isNewTab)
        {
            AggregateIdentifier = page;
            Site = site;
            ParentPage = parentPage;

            Tenant = tenant;
            Author = author;

            Type = type;
            Title = title;

            Sequence = sequence;
            
            IsNewTab = isNewTab;
            IsHidden = isHidden;
        }
    }
}
