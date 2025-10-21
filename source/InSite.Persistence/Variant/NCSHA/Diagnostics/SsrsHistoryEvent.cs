using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class SsrsHistoryEvent : NcshaHistoryEvent
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CriteriaItem
        {
            #region Properties

            [JsonProperty(PropertyName = "name")]
            public string Name { get; private set; }

            [JsonProperty(PropertyName = "value")]
            public string Value { get; private set; }

            #endregion

            #region Construction

            public CriteriaItem(string name, string value)
            {
                Name = name;
                Value = value;
            }

            #endregion
        }

        #endregion

        #region Proprties

        [JsonProperty(PropertyName = "code")]
        public string Code { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        public IReadOnlyList<CriteriaItem> Criteria => _criteria;

        #endregion

        #region Fields

        [JsonProperty(PropertyName = "criteria")]
        private List<CriteriaItem> _criteria;

        #endregion

        #region Construction

        [JsonConstructor]
        protected SsrsHistoryEvent()
        {

        }

        public SsrsHistoryEvent(string code, string name)
        {
            Code = code;
            Name = name;

            _criteria = new List<CriteriaItem>();
        }

        #endregion

        #region Properties

        public void AddCriteria(string name, string value)
        {
            var item = new CriteriaItem(name, value);

            _criteria.Add(item);
        }

        #endregion
    }
}
