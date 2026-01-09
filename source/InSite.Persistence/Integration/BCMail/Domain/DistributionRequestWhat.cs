using System.Collections.Generic;

namespace InSite.Persistence.Integration.BCMail
{
    public class DistributionRequestWhat
    {
        public int FormNumber { get; set; }
        public string FormNumberType { get; set; }

        public string FormType { get; set; }
        public string FormTitle { get; set; }
        public string FormLevel { get; set; }
        public string FormName { get; set; }

        public DistributionRequestWhatMaterials Materials { get; set; }
        public List<DistributionRequestWhatWho> Who { get; set; }

        public DistributionRequestWhat()
        {
            Materials = new DistributionRequestWhatMaterials();
            Who = new List<DistributionRequestWhatWho>();
        }
    }
}