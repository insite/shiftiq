using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class OptionGrid : BaseUserControl
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class StateData
        {
            [JsonProperty(PropertyName = "lang")]
            public string Language { get; set; }

            [JsonProperty(PropertyName = "summary")]
            public Dictionary<int, bool> SummaryRows { get; private set; } = new Dictionary<int, bool>();

            [JsonProperty(PropertyName = "content")]
            public List<MultilingualDictionary> Content { get; private set; } = new List<MultilingualDictionary>();

            public StateData(string language)
            {
                Language = language;
            }
        }

        private class DataItem
        {
            public Guid? Identifier { get; set; }
            public string Letter { get; set; }
            public decimal Points { get; set; }
            public string Category { get; set; }
            public bool IsSummaryVisible { get; set; }
            public MultilingualDictionary Content { get; set; }
        }

        #endregion

        #region Properties

        public int ItemsCount => Repeater.Items.Count;

        private List<Guid?> Identifiers
        {
            get => (List<Guid?>)ViewState[nameof(Identifiers)];
            set => ViewState[nameof(Identifiers)] = value;
        }

        #endregion

        #region Fields

        private StateData _stateData = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += Repeater_DataBinding;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.ItemCommand += Repeater_ItemCommand;

            AddNewOptionCommand.Click += AddNewOptionCommand_Click;
            ImportOptionsSaveButton.Click += ImportOptionsSaveButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReorderInput.Value))
                BindRepeater(GetItems());

            if (_stateData != null)
                StateInput.Value = JsonConvert.SerializeObject(_stateData);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void AddNewOptionCommand_Click(object sender, EventArgs e)
        {
            var items = GetItems();

            items.Add(new DataItem
            {
                Letter = Calculator.ToBase26(items.Count + 1),
                Content = new MultilingualDictionary()
            });

            BindRepeater(items);
        }

        private void ImportOptionsSaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ImportOptionsText.Text))
                return;

            var items = GetItems();
            var language = GetStateData().Language;
            var lines = ImportOptionsText.Text.Replace("\r", string.Empty).Split(new[] { '\n', ';' });

            foreach (string line in lines)
            {
                var trimmedLine = line.Trim(' ');
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                var item = new DataItem
                {
                    Letter = Calculator.ToBase26(items.Count + 1),
                    Content = new MultilingualDictionary()
                };

                item.Content.AddOrGet(ContentLabel.Title)[language] = trimmedLine;

                items.Add(item);
            }

            BindRepeater(items);
        }

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            Identifiers = new List<Guid?>();

            var stateData = GetStateData();
            stateData.Content.Clear();
            stateData.SummaryRows.Clear();

            ReorderInput.Value = string.Empty;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var option = (DataItem)e.Item.DataItem;

            Identifiers.Add(option.Identifier);

            var stateData = GetStateData();
            stateData.Content.Add(option.Content);

            if (option.IsSummaryVisible)
                stateData.SummaryRows.Add(e.Item.ItemIndex, true);
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var index = e.Item.ItemIndex;
                var items = GetItems();

                items.RemoveAt(index);

                BindRepeater(items);
            }
        }

        #endregion

        #region Data operations

        public int LoadData(SurveyQuestion question, string language)
        {
            _stateData = new StateData(language);

            var options = question != null && !question.Options.IsEmpty
                ? (ICollection<SurveyOptionItem>)question.Options.Lists[0].Items
                : new SurveyOptionItem[0];

            BindRepeater(options.Select(x =>
            {
                var item = new DataItem
                {
                    Identifier = x.Identifier,
                    Letter = x.Letter,
                    Points = x.Points,
                    Category = x.Category
                };

                item.Content = new MultilingualDictionary();
                item.Content[ContentLabel.Title] = x.Content.Title.Text.Clone();
                item.Content[ContentLabel.Description] = x.Content.Description.Text.Clone();
                item.Content[ContentLabel.Feedback] = x.Content.Feedback.Text.Clone();
                item.Content[ContentLabel.FeedbackWhenNotSelected] = x.Content.FeedbackWhenNotSelected.Text.Clone();
                item.IsSummaryVisible = !item.Content[ContentLabel.Description].IsEmpty
                                     || !item.Content[ContentLabel.Feedback].IsEmpty;

                return item;
            }));

            return options.Count;
        }

        public bool Save(SurveyQuestion question)
        {
            var defaultLang = question.Form.Language;

            if (question.Options.IsEmpty)
                question.Options.Add(new SurveyOptionList(UniqueIdentifier.Create()));

            var optionList = question.Options.Lists[0];
            var options = optionList.Items;
            var items = GetItems();

            optionList.Items = new List<SurveyOptionItem>();

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var option = item.Identifier.HasValue
                    ? options.Find(x => x.Identifier == item.Identifier.Value)
                    : new SurveyOptionItem(UniqueIdentifier.Create());

                option.Points = item.Points;
                option.Category = item.Category.NullIfWhiteSpace();

                option.Content.Title.Text = item.Content.Get(ContentLabel.Title);
                option.Content.Description.Text = item.Content.Get(ContentLabel.Description);
                option.Content.Feedback.Text = item.Content.Get(ContentLabel.Feedback);
                option.Content.FeedbackWhenNotSelected.Text = item.Content.Get(ContentLabel.FeedbackWhenNotSelected);

                if (string.IsNullOrEmpty(option.Content.Title.Text[defaultLang]))
                    option.Content.Title.Text[defaultLang] = Calculator.ToBase26(i + 1);

                option.List = optionList;

                optionList.Items.Add(option);
            }

            return true;
        }

        #endregion

        #region Methods (helpers)

        internal void Translate(string fromLang, string[] toLangs)
        {
            var isUpdated = false;
            var items = GetItems();

            foreach (var toLang in toLangs)
            {
                var data = new List<MultilingualString>();

                foreach (var item in items)
                {
                    TryAddString(item.Content.Get(ContentLabel.Title));
                    TryAddString(item.Content.Get(ContentLabel.Description));
                    TryAddString(item.Content.Get(ContentLabel.Feedback));
                    TryAddString(item.Content.Get(ContentLabel.FeedbackWhenNotSelected));
                }

                if (data.Count > 0)
                {
                    ((IHasTranslator)Page).Translator.Translate(fromLang, toLang, data);
                    isUpdated = true;
                }

                void TryAddString(MultilingualString str)
                {
                    if (str != null && !string.IsNullOrEmpty(str[fromLang]) && string.IsNullOrEmpty(str[toLang]))
                        data.Add(str);
                }
            }

            if (isUpdated)
                BindRepeater(items);
        }

        private void BindRepeater(IEnumerable<DataItem> dataSource)
        {
            Repeater.Visible = true;

            Repeater.DataSource = dataSource;
            Repeater.DataBind();
            Repeater.Visible = Repeater.Items.Count > 0;
        }

        private StateData GetStateData()
        {
            if (_stateData == null && !string.IsNullOrEmpty(StateInput.Value))
                _stateData = JsonConvert.DeserializeObject<StateData>(StateInput.Value);

            return _stateData;
        }

        private List<DataItem> GetItems()
        {
            var result = GetItemsFromRepeater();

            if (string.IsNullOrEmpty(ReorderInput.Value))
                return result;

            var newOrder = JsonConvert.DeserializeObject<int[]>(ReorderInput.Value);
            if (newOrder.Length != newOrder.Where(x => x >= 0 && x < newOrder.Length).Distinct().Count())
                return result;

            var origOrder = result.ToArray();
            if (origOrder.Length != newOrder.Length)
                return result;

            for (var i = 0; i < origOrder.Length; i++)
                result[i] = origOrder[newOrder[i]];

            return result;
        }

        private List<DataItem> GetItemsFromRepeater()
        {
            var stateData = GetStateData();
            var result = new List<DataItem>();

            for (int i = 0; i < Repeater.Items.Count && i < stateData.Content.Count; i++)
            {
                var item = Repeater.Items[i];

                var letterInput = (ITextBox)item.FindControl("LetterInput");
                var pointsInput = (NumericBox)item.FindControl("PointsInput");
                var categoryInput = (ITextBox)item.FindControl("CategoryInput");

                var dataItem = new DataItem
                {
                    Identifier = Identifiers[i],
                    Letter = letterInput.Text,
                    Points = pointsInput.ValueAsDecimal.Value,
                    Category = categoryInput.Text,
                    IsSummaryVisible = stateData.SummaryRows.ContainsKey(i) && stateData.SummaryRows[i],
                    Content = stateData.Content[i]
                };

                result.Add(dataItem);
            }

            return result;
        }

        #endregion
    }
}