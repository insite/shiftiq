using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class LikertTableColumnGrid : BaseUserControl
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateData
        {
            [JsonProperty(PropertyName = "lang")]
            public string Language { get; set; }

            [JsonProperty(PropertyName = "content")]
            public MultilingualDictionary Content { get; private set; } = new MultilingualDictionary();

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

            [JsonProperty(PropertyName = "text")]
            public MultilingualString Text { get; set; }

            [JsonProperty(PropertyName = "points")]
            public decimal Points { get; set; }

            [JsonProperty(PropertyName = "category")]
            public string Category { get; set; }

            public DataItem()
            {

            }

            public DataItem(int index, SurveyOptionItem option)
                : this()
            {
                Index = index;
                Text = option.Content?.Title?.Text.Clone() ?? new MultilingualString();
                Points = option.Points;
                Category = option.Category;
            }

            public bool ShouldSerializePoints() => Points != 0;

            public bool ShouldSerializeCategory() => !Category.IsEmpty();
        }

        #endregion

        #region Properties

        public bool HasItems => (GetStateData()?.DataItems?.Count ?? 0) > 0;

        private Guid[][] Identifiers
        {
            get => (Guid[][])ViewState[nameof(Identifiers)];
            set => ViewState[nameof(Identifiers)] = value;
        }

        #endregion

        #region Fields

        private StateData _stateData = null;

        #endregion

        #region Initialization

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                HighestRatingText.Attributes["data-content"] = ContentLabel.LikertHighest;
                LowestRatingText.Attributes["data-content"] = ContentLabel.LikertLowest;
            }
        }

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
            Identifiers = new Guid[question.Options.Lists.Count][];

            _stateData.Content[ContentLabel.LikertLowest] = question.Content?[ContentLabel.LikertLowest]?.Text;
            _stateData.Content[ContentLabel.LikertHighest] = question.Content?[ContentLabel.LikertHighest]?.Text;

            if (question.Options.Lists.Count == 0)
                return 0;

            foreach (var option in question.Options.Lists[0].Items)
                _stateData.DataItems.Add(new DataItem(_stateData.DataItems.Count, option));

            for (var i = 0; i < Identifiers.Length; i++)
            {
                var list = question.Options.Lists[i];
                var listIds = Identifiers[i] = new Guid[list.Items.Count];

                for (var j = 0; j < listIds.Length; j++)
                    listIds[j] = list.Items[j].Identifier;
            }

            return Identifiers[0].Length;
        }

        public bool Save(SurveyQuestion question)
        {
            var state = GetStateData();

            question.Content[ContentLabel.LikertLowest].Text = state.Content.Get(ContentLabel.LikertLowest);
            question.Content[ContentLabel.LikertHighest].Text = state.Content[ContentLabel.LikertHighest];

            if (question.Options.Lists.Count == 0)
                return false;

            var optionMapping = new int[state.DataItems.Count];
            if (optionMapping.Length > 0)
            {
                for (var i = 0; i < optionMapping.Length; i++)
                    optionMapping[i] = -1;

                Guid[] identifiers = null;
                List<SurveyOptionItem> options = null;

                for (var i = 0; i < Identifiers.Length; i++)
                {
                    var ids = Identifiers[i];

                    for (var j = 0; j < question.Options.Lists.Count; j++)
                    {
                        var items = question.Options.Lists[j].Items;
                        if (items.Any(x => ids.Contains(x.Identifier)))
                        {
                            identifiers = ids;
                            options = items.ToList();
                            break;
                        }
                    }

                    if (identifiers != null)
                        break;
                }

                if (identifiers != null)
                {
                    for (var i = 0; i < optionMapping.Length; i++)
                    {
                        var item = state.DataItems[i];
                        if (item.Index >= 0 && item.Index <= identifiers.Length)
                        {
                            var id = identifiers[item.Index];
                            var index = options.FindIndex(x => x.Identifier == id);

                            if (!optionMapping.Contains(index))
                                optionMapping[i] = index;
                        }
                    }
                }
            }

            foreach (var list in question.Options.Lists)
            {
                var options = list.Items;

                list.Items = new List<SurveyOptionItem>();

                for (var i = 0; i < state.DataItems.Count; i++)
                {
                    var item = state.DataItems[i];
                    var index = optionMapping[i];
                    var option = index >= 0 && index < options.Count
                        ? options[index]
                        : new SurveyOptionItem(UniqueIdentifier.Create());

                    option.Points = item.Points;
                    option.Category = item.Category.NullIfWhiteSpace();
                    if (option.Content == null)
                        option.Content = new Shift.Common.ContentContainer();
                    option.Content.Title.Text = item.Text;

                    // Remove old content properties from Surveys I no longer supported for Likert Columns in Surveys II.
                    // option.Content.Get("Body").Clear();
                    // option.Content.Get("Description").Clear();
                    // option.Content.Get("Feedback").Clear();
                    // option.Content.Get("Hint").Clear();
                    // option.Content.Get("Summary").Clear();

                    option.List = list;

                    list.Items.Add(option);
                }
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

            foreach (var item in state.Content.GetItems())
                collection.Add(item);

            foreach (var item in state.DataItems)
                collection.Add(item.Text);
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