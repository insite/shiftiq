using System.Collections.Generic;

namespace Shift.Common
{
    public class SentinelsSettings
    {
        public Sentinel Maintenance { get; set; }
        public Sentinel Root { get; set; }
        public Sentinel Someone { get; set; }
        public Sentinel Test { get; set; }

        public Sentinel[] ToArray()
        {
            var list = new List<Sentinel>();

            if (Maintenance != null)
                list.Add(Maintenance);

            if (Root != null)
                list.Add(Root);

            if (Someone != null)
                list.Add(Someone);

            if (Test != null)
                list.Add(Test);

            return list.ToArray();
        }
    }

    public class Sentinel : Actor
    {
        public string[] Roles { get; set; }
    }
}
