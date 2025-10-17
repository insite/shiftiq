using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Courses
{
    [Serializable]
    public class Module
    {
        public static class Defaults
        {
            public const bool IsAdaptive = false;
        }

        [JsonIgnore]
        public Unit Unit { get; set; }

        public Guid Identifier { get; set; }
        public ContentContainer Content { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        public List<Activity> Activities { get; set; }

        public int ModuleAsset { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleImage { get; set; }
        public int ModuleSequence { get; set; }
        public Guid? SourceIdentifier { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool ModuleIsAdaptive { get; set; }
        public string PrerequisiteDeterminer { get; set; }
    }
}
