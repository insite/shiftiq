using System;

namespace Shift.Contract
{
    public partial class SpecWorkshop
    {
        public class Set
        {
            public Guid? AreaId { get; set; }
            public WorkshopQuestion[] Questions { get; set; }
        }
    }
}
