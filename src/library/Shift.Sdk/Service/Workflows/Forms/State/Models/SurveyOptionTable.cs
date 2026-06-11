using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    [Serializable]
    public class SurveyOptionTable
    {
        [JsonIgnore]
        public SurveyQuestion Question { get; set; }

        [JsonIgnore]
        private List<SurveyOptionList> _lists { get; set; }
        public List<SurveyOptionList> Lists => _lists;

        [JsonIgnore]
        public bool IsEmpty => _lists.IsEmpty();

        public SurveyOptionTable()
        {
            _lists = new List<SurveyOptionList>();
        }

        public bool ShouldSerializeLists() => !IsEmpty;

        public int IndexOf(SurveyOptionList list) => _lists.IndexOf(list);

        public void Add(SurveyOptionList list) => _lists.Add(list);

        public void Remove(SurveyOptionList list) => _lists.Remove(list);
    }
}