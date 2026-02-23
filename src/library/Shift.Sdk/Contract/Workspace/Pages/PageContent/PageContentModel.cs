using System;

using Shift.Common;

namespace Shift.Contract
{
    public class PageContentModel
    {
        public string Title { get; set; }
        public string[] ContentFields { get; set; }
        public ContentContainer Content { get; set; }
        public BlockContentModel[] Blocks { get; set; }
    }
}
