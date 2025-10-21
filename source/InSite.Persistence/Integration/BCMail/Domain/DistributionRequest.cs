using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    public class DistributionRequest
    {
        public DistributionRequestWhen When { get; set; }
        public DistributionRequestWhere Where { get; set; }
        public List<DistributionRequestWhat> What { get; set; }

        [JsonIgnore]
        public int WhatCount { get { return What.Count; } }

        [JsonIgnore]
        public int WhoCount { get { return What.SelectMany(x => x.Who).Count(); } }

        public DistributionRequest()
        {
            When = new DistributionRequestWhen();
            Where = new DistributionRequestWhere();
            What = new List<DistributionRequestWhat>();
        }
    }
}