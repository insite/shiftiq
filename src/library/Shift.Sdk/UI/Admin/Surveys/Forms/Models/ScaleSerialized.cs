using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class ScaleSerialized
    {
        public string Category { get; set; }

        public List<ScaleItemSerialized> Items { get; set; }
    }
}