using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ChartHistoryEvent : NcshaHistoryEvent
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CriteriaInfo
        {
            #region Properties

            public IReadOnlyList<CriteriaFieldInfo> Fields => _fields;

            [JsonProperty(PropertyName = "regions")]
            public string[] Regions { get; private set; }

            [JsonProperty(PropertyName = "year")]
            public CriteriaYearInfo Year { get; private set; }

            [JsonProperty(PropertyName = "aggregateFunc")]
            public string Func { get; private set; }

            [JsonProperty(PropertyName = "datasetType")]
            public string DatasetType { get; private set; }

            [JsonProperty(PropertyName = "axis")]
            public string AxisName { get; private set; }

            [JsonProperty(PropertyName = "unit")]
            public string AxisUnit { get; private set; }

            #endregion

            #region Fields

            [JsonProperty(PropertyName = "fields")]
            private List<CriteriaFieldInfo> _fields;

            #endregion

            #region Construction

            [JsonConstructor]
            private CriteriaInfo()
            {
                Regions = new string[0];
                Year = new CriteriaYearInfo(null, null);
                _fields = new List<CriteriaFieldInfo>();
            }

            public CriteriaInfo(string[] regions, int? fromYear, int? toYear, string func, string datasetType, string axis, string unit)
            {
                _fields = new List<CriteriaFieldInfo>();

                Regions = regions;
                Year = new CriteriaYearInfo(fromYear, toYear);
                Func = func;
                DatasetType = datasetType;
                AxisName = axis;
                AxisUnit = unit;
            }

            #endregion

            #region Methods

            public void AddField(string category, string name, string code)
            {
                var field = new CriteriaFieldInfo(category, name, code);

                _fields.Add(field);
            }

            #endregion

            #region Methods (JSON)

            public bool ShouldSerializeRegions() => Regions.IsNotEmpty();

            public bool ShouldSerializeYear() => Year.From.HasValue || Year.To.HasValue;

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CriteriaFieldInfo
        {
            #region Properties

            [JsonProperty(PropertyName = "category")]
            public string Category { get; private set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; private set; }

            [JsonProperty(PropertyName = "code")]
            public string Code { get; private set; }

            #endregion

            #region Construction

            public CriteriaFieldInfo(string category, string name, string code)
            {
                Category = category;
                Name = name;
                Code = code;
            }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CriteriaYearInfo
        {
            #region Properties

            [JsonProperty(PropertyName = "from", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? From { get; private set; }

            [JsonProperty(PropertyName = "to", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? To { get; private set; }

            #endregion

            #region Construction

            public CriteriaYearInfo(int? from, int? to)
            {
                From = from;
                To = to;
            }

            #endregion
        }

        #endregion

        #region Properties

        public IReadOnlyList<CriteriaInfo> Criteria => _criteria;

        #endregion

        #region Fields

        [JsonProperty(PropertyName = "criteria")]
        private List<CriteriaInfo> _criteria = new List<CriteriaInfo>();

        #endregion

        #region Methods

        public CriteriaInfo AddCriteria(string[] regions, int? fromYear, int? toYear, string func, string datasetType, string axis, string unit)
        {
            var criteria = new CriteriaInfo(regions, fromYear, toYear, func, datasetType, axis, unit);

            _criteria.Add(criteria);

            return criteria;
        }

        #endregion
    }
}
