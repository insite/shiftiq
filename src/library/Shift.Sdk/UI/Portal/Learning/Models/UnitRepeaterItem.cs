using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI
{
    public class UnitRepeaterItem
    {
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Asset { get; set; }
        public bool IsAdaptive { get; set; }
        public bool IsLocked { get; set; }

        public List<ModuleRepeaterItem> Modules { get; set; }

        public bool IsActive => !IsLocked && Modules.Count > 0 && Modules.Any(x => x.IsActive);
    }
}