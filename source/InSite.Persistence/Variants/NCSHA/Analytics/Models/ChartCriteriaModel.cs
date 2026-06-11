using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartCriteriaModel
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class RegionInfo
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "exclude")]
            public IEnumerable<int> ExcludeYears { get; set; }

            public RegionInfo(string name, IEnumerable<int> excludeYears)
            {
                Name = name;
                ExcludeYears = excludeYears;
            }
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "regions")]
        public IEnumerable<RegionInfo> Regions { get; private set; }

        [JsonProperty(PropertyName = "years")]
        public IEnumerable<int> Years { get; private set; }

        #endregion

        #region Construction

        private ChartCriteriaModel()
        {

        }

        #endregion

        #region Initialization

        public static ChartCriteriaModel Create(IEnumerable<string> codes)
        {
            var data = CounterRepository.Distinct(x => new { x.Scope, x.Year }, x => codes.Contains(x.Code), "Scope");
            var years = data.Select(x => x.Year).Distinct().OrderBy(x => x).ToArray();
            var regions = data.GroupBy(x => x.Scope)
                .Select(x => new RegionInfo(x.Key, x.Select(y => y.Year).ToArray()))
                .ToArray();

            foreach (var region in regions)
                region.ExcludeYears = years.Where(x => !region.ExcludeYears.Contains(x)).ToArray();

            return new ChartCriteriaModel
            {
                Regions = regions,
                Years = years
            };
        }

        #endregion
    }
}