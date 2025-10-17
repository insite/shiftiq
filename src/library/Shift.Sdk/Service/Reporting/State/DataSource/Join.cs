using System;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class Join
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Expression { get; set; }
    }
}
