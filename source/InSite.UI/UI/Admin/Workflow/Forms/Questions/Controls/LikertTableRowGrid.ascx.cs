using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class LikertTableRowGrid : BaseUserControl
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateData
        {
            [JsonProperty(PropertyName = "lang")]
            public string Language { get; set; }

            [JsonProperty(PropertyName = "items")]
            public List<DataItem> DataItems { get; private set; } = new List<DataItem>();

            public StateData(string language)
            {
                Language = language;
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class DataItem
        {
            [JsonProperty(PropertyName = "id")]
            public int Index { get; set; }

            [JsonProperty(PropertyName = "title")]
            public MultilingualString Title { get; set; }

            [JsonProperty(PropertyName = "category")]
            public string Category { get; set; }

            public DataItem()
            {

            }

            public DataItem(int index, SurveyOptionList list)
                : this()
            {
                Index = index;
                Title = list.Content?.Title?.Text.Clone() ?? new MultilingualString();
                Category = list.Category;
            }

            public bool ShouldSerializeCategory() => !Category.IsEmpty();
        }

        #endregion

        #region Properties

        public bool HasItems => (GetStateData()?.DataItems?.Count ?? 0) > 0;

        private List<Guid> Identifiers
        {
            get => (List<Guid>)ViewState[nameof(Identifiers)];
            set => ViewState[nameof(Identifiers)] = value;
        }

        #endregion

        #region Fields

        private StateData _stateData = null;

        #endregion

        #region Initialization

        protected override void OnPreRender(EventArgs e)
        {
            if (_stateData != null)
                StateInput.Value = JsonConvert.SerializeObject(_stateData);

            base.OnPreRender(e);
        }

        #endregion

        #region Methods (data binding)

        public int LoadData(SurveyQuestion question, string language)
        {
            _stateData = new StateData(language);
            Identifiers = new List<Guid>();

            foreach (var option in question.Options.Lists)
            {
                _stateData.DataItems.Add(new DataItem(Identifiers.Count, option));
                Identifiers.Add(option.Identifier);
            }

            return question.Options.Lists.Count;
        }

        public bool Save(SurveyQuestion question)
        {
            var state = GetStateData();
            var lists = question.Options.Lists.ToDictionary(x => x.Identifier);

            question.Options.Lists.Clear();

            foreach (var item in state.DataItems)
            {
                SurveyOptionList list = null;

                if (item.Index >= 0 && item.Index <= Identifiers.Count && lists.TryGetValue(Identifiers[item.Index], out list))
                    lists.Remove(list.Identifier);

                if (list == null)
                    list = new SurveyOptionList
                    {
                        Identifier = UniqueIdentifier.Create(),
                        Question = question,
                        Table = question.Options
                    };

                if (list.Content == null)
                    list.Content = new ContentContainer();

                list.Content.Title.Text = item.Title;
                list.Category = item.Category;

                question.Options.Lists.Add(list);
            }

            return true;
        }

        #endregion

        #region Methods (helpers)

        internal void AddContent(ICollection<MultilingualString> collection)
        {
            var state = GetStateData();
            if (state == null)
                return;

            foreach (var item in state.DataItems)
                collection.Add(item.Title);
        }

        private StateData GetStateData()
        {
            if (_stateData == null && !string.IsNullOrEmpty(StateInput.Value))
                _stateData = JsonConvert.DeserializeObject<StateData>(StateInput.Value);

            return _stateData;
        }

        #endregion
    }
}