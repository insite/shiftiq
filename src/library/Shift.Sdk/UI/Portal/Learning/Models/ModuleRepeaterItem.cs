using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI
{
    public class ModuleRepeaterItem
    {
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Asset { get; set; }
        public bool IsAdaptive { get; set; }
        public bool IsLocked { get; set; }

        public List<ActivityRepeaterItem> Activities { get; set; }

        public bool IsActive => !IsLocked && Activities.Count > 0 && Activities.Any(x => x.IsActive);
    }
}