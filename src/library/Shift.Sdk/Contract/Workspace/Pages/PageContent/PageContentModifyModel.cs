using System;

using Shift.Common;

namespace Shift.Contract
{
    public class PageContentModifyModel
    {
        public ContentContainer Content { get; set; }
        public BlockContentModel[] Blocks { get; set; }
        public Guid[] DeletedBlockIds { get; set; }
    }
}
