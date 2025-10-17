using System.Collections.Generic;
using System.Linq;

namespace Shift.Sdk.UI
{
    public class EventStatisticGroup
    {
        public string Name { get; set; }
        public int Sum => Items != null ? Items.Sum(x => x.Count) : 0;
        public IEnumerable<EventStatisticItem> Items { get; set; }
    }
}