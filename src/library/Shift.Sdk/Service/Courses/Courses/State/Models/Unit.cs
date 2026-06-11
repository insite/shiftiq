using System;
using System.Collections.Generic;

using Shift.Common;
namespace InSite.Domain.Courses
{
    [Serializable]
    public class Unit
    {
        public static class Defaults
        {
            public const bool IsAdaptive = false;
        }

        public Guid Identifier { get; set; }
        public ContentContainer Content { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        public List<Module> Modules { get; set; }

        public int UnitAsset { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public int UnitSequence { get; set; }
        public Guid? SourceIdentifier { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool UnitIsAdaptive { get; set; }
        public string PrerequisiteDeterminer { get; set; }
    }
}
