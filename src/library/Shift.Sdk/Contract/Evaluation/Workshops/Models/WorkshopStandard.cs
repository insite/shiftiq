using System;

namespace Shift.Contract
{
    public class WorkshopStandard
    {
        public Guid StandardId { get; set; }
        public Guid? ParentId { get; set; }
        public int AssetNumber { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }

        public WorkshopStandard Parent { get; set; }
    }
}