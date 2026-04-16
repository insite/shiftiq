using System;

using Shift.Common;

namespace Shift.Contract
{
    public class BlockContentModel
    {
        public Guid BlockId { get; set; }
        public int? BlockIdNumber { get; set; }
        public string BlockType { get; set; }
        public string Title { get; set; }
        public string Hook { get; set; }
        public ContentContainer Content { get; set; }
    }
}
