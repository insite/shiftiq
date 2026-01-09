using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Banks.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Assessments.Options.Controls
{
    public abstract class OptionWriteRepeater : BaseUserControl
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Classes

        [Serializable]
        protected abstract class ItemCollection : IEnumerable<ItemCollection.DataItem>
        {
            #region Classes

            [Serializable]
            public abstract class DataItem
            {
                public int Number { get; }
                public int Sequence { get; private set; }
                public MultilingualString Text { get; }

                public string Letter => Calculator.ToBase26(Sequence);
                public bool IsReadOnly => _collection.IsItemReadOnly(this);

                private ItemCollection _collection;

                public DataItem()
                {
                    Number = -1;
                    Text = new MultilingualString();
                }

                public DataItem(int number)
                    : this()
                {
                    Number = number;
                }

                internal void SetCollection(ItemCollection collection)
                {
                    if (_collection != null)
                        throw ApplicationError.Create("The item is already added to collection");

                    _collection = collection;
                    Sequence = _collection.GetOptionSequence();
                }

                public virtual Option GetOption()
                {
                    var option = new Option();

                    option.Content.Title.Set(Text);

                    return option;
                }
            }

            #endregion

            #region Properties

            public Guid QuestionIdentifier { get; private set; }

            public int Count => _items.Count;

            public bool IsReadOnly { get; set; }

            public DataItem this[int index] => _items[index];

            #endregion

            #region Fields

            private int _sequence = 0;
            private HashSet<int> _lockedOptions;
            private List<DataItem> _items = new List<DataItem>();

            #endregion

            #region Construction

            public ItemCollection(Guid questionId)
            {
                QuestionIdentifier = questionId;
            }

            #endregion

            #region Methods (collection)

            public void Add(DataItem item)
            {
                item.SetCollection(this);

                _items.Add(item);
            }

            public void Remove(DataItem item) => _items.Remove(item);

            public void Sort(Comparison<DataItem> comparison) => _items.Sort(comparison);

            #endregion

            #region Methods (helpers)

            public void LoadLockedOptions()
            {
                _lockedOptions = IsReadOnly || QuestionIdentifier == Guid.Empty || _items.All(x => x.Number < 0)
                    ? null
                    : new HashSet<int>(ServiceLocator.AttemptSearch.GetAttemptExistOptionKeys(QuestionIdentifier));
            }

            private int GetOptionSequence()
            {
                return ++_sequence;
            }

            public void Clear()
            {
                _sequence = 0;
                _lockedOptions = null;
                _items = new List<DataItem>();
            }

            private bool IsItemReadOnly(DataItem item)
            {
                if (IsReadOnly)
                    return true;

                if (item.Number < 0)
                    return false;

                return _lockedOptions.Contains(item.Number);
            }

            #endregion

            #region Methods (IEnumerable)

            public IEnumerator<DataItem> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        #region Properties

        protected ItemCollection OptionItems
        {
            get => (ItemCollection)ViewState[nameof(OptionItems)];
            set => ViewState[nameof(OptionItems)] = value;
        }

        public bool IsReadOnly
        {
            get => OptionItems.IsReadOnly;
            set => OptionItems.IsReadOnly = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var repeater = GetRepeater();
            repeater.DataBinding += Repeater_DataBinding;
            repeater.ItemCreated += Repeater_ItemCreated;
            repeater.ItemDataBound += Repeater_ItemDataBound;
            repeater.ItemCommand += Repeater_ItemCommand;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            var repeater = GetRepeater();
            for (var i = 0; i < repeater.Items.Count; i++)
                SetupOptionItemValidator(repeater.Items[i], groupName);
        }

        private void SetupOptionItemValidator(RepeaterItem item, string groupName)
        {
            foreach (var v in GetItemValidators(item))
                v.ValidationGroup = groupName;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (OptionItems != null)
            {
                var repeater = GetRepeater();
                for (var i = 0; i < OptionItems.Count; i++)
                    OnOptionItemDataBound(repeater.Items[i], OptionItems[i]);
            }
        }

        #endregion

        #region Event handlers

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            ApplyReorder();

            OptionItems.LoadLockedOptions();

            ((Repeater)sender).DataSource = OptionItems;
        }

        private void Repeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            OnRepeaterItemCreated(e.Item);
            SetupOptionItemValidator(e.Item, ValidationGroup);
        }

        protected virtual void OnRepeaterItemCreated(RepeaterItem item)
        {

        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            OnRepeaterItemDataBound(e.Item);
        }

        protected virtual void OnRepeaterItemDataBound(RepeaterItem item)
        {

        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var item = OptionItems[e.Item.ItemIndex];

                if (CanRemove(OptionItems.QuestionIdentifier, item.Number, out var removeError))
                {
                    OptionItems.Remove(item);
                    GetRepeater().DataBind();
                }
                else
                {
                    OnAlert(AlertType.Error, removeError);
                }
            }
        }

        #endregion

        #region Methods (public)

        public virtual void LoadData()
        {
            OptionItems = CreateItemCollection(Guid.Empty);

            PopulateItems(null);

            GetRepeater().DataBind();
        }

        public virtual void LoadData(Question question)
        {
            OptionItems = CreateItemCollection(question.Identifier);

            PopulateItems(question);

            GetRepeater().DataBind();
        }

        public virtual Command[] GetCommands(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (OptionItems == null)
                return null;

            var commands = new List<Command>();
            var reorderCommand = new ReorderOptions(question.Set.Bank.Identifier, question.Identifier);
            var nextOptionSequence = question.Options.Count + 1;

            ApplyReorder();

            var hasCorrectAnswer = false;

            for (var i = 0; i < OptionItems.Count; i++)
            {
                var item = OptionItems[i];
                var itemSequence = i + 1;
                var option = item.GetOption();

                if (!hasCorrectAnswer && (option.HasPoints || option.IsTrue == true))
                    hasCorrectAnswer = true;

                if (item.Number > 0)
                {
                    var existOption = question.Options.First(x => x.Number == item.Number);
                    if (!option.Equals(existOption))
                        commands.Add(new ChangeOption(question.Set.Bank.Identifier, question.Identifier, existOption.Number, option.Content, option.Points, option.IsTrue, option.CutScore, option.Standard));

                    if (existOption.Sequence != itemSequence)
                        reorderCommand.Sequences.Add(existOption.Sequence, itemSequence);
                }
                else
                {
                    var newSequence = nextOptionSequence++;

                    commands.Add(new AddOption(question.Set.Bank.Identifier, question.Identifier, option.Content, option.Points, option.IsTrue, option.CutScore, option.Standard));

                    if (newSequence != itemSequence)
                        reorderCommand.Sequences.Add(newSequence, itemSequence);
                }
            }

            if (!hasCorrectAnswer)
                throw ApplicationError.Create("The question contains no correct option.");

            if (reorderCommand.Sequences.Count > 0)
                commands.Add(reorderCommand);

            foreach (var option in question.Options)
            {
                if (OptionItems.Any(o => o.Number == option.Number))
                    continue;

                if (!CanRemove(OptionItems.QuestionIdentifier, option.Number, out var removeError))
                    throw new ApplicationError(removeError);

                commands.Add(new DeleteOption(question.Set.Bank.Identifier, question.Identifier, option.Number));
            }

            return commands.ToArray();
        }

        public void GetInputValues(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (OptionItems == null)
                return;

            question.Options.Clear();

            ApplyReorder();

            var options = new List<Option>();

            for (var i = 0; i < OptionItems.Count; i++)
            {
                var item = OptionItems[i];
                var option = item.GetOption();

                if (item.Number > 0)
                {
                    var existOption = question.Options.First(x => x.Number == item.Number);

                    if (!option.Equals(existOption))
                    {
                        existOption.Points = option.Points;
                        existOption.IsTrue = option.IsTrue;
                        existOption.CutScore = option.CutScore;
                        existOption.Standard = option.Standard;
                        existOption.Content.Title.Set(option.Content?.Title);

                        options.Add(existOption);
                    }
                }

                options.Add(option);
            }

            question.Options.Clear();
            question.Options.AddRange(options);
        }

        #endregion

        #region Methods (abstract)

        protected abstract Repeater GetRepeater();
        protected abstract HiddenField GetItemsOrder();
        protected abstract IEnumerable<BaseValidator> GetItemValidators(RepeaterItem item);

        protected abstract void OnOptionItemDataBound(RepeaterItem repeaterItem, ItemCollection.DataItem optionItem);

        protected abstract ItemCollection CreateItemCollection(Guid questionId);

        #endregion

        #region Methods (helpers)

        protected abstract void PopulateItems(Question question);

        protected void ApplyReorder()
        {
            var itemsOrder = GetItemsOrder();
            if (itemsOrder.Value.IsEmpty())
                return;

            var orderDict = new Dictionary<int, int>();
            var orderArray = itemsOrder.Value.Split(';');

            for (var index = 0; index < orderArray.Length; index++)
            {
                var itemId = int.Parse(orderArray[index]);
                orderDict.Add(itemId, index + 1);
            }

            OptionItems.Sort((x, y) =>
            {
                var xSequence = orderDict.ContainsKey(x.Sequence) ? orderDict[x.Sequence] : int.MaxValue;
                var ySequence = orderDict.ContainsKey(y.Sequence) ? orderDict[y.Sequence] : int.MaxValue;

                return xSequence.CompareTo(ySequence);
            });

            itemsOrder.Value = string.Empty;
        }

        protected static bool CanRemove(Guid question, int number, out string error)
        {
            error = null;

            var attemptCount = number > 0
                ? ServiceLocator.AttemptSearch.CountAttempts(a => a.Options.Any(o => o.QuestionIdentifier == question && o.OptionKey == number))
                : 0;

            if (attemptCount == 0)
                return true;

            error = $"There {(attemptCount == 1 ? "is" : "are")} {"submission".ToQuantity(attemptCount)} to exam forms containing this question" +
                $", which cannot be invalidated by changes to this question.";

            return false;
        }

        public static OptionWriteRepeater LoadRepeater(DynamicControl container, QuestionItemType type)
        {
            var path = "~/UI/Admin/Assessments/Options/Controls/Write/{0}.ascx";

            if (type == QuestionItemType.SingleCorrect)
                path = path.Format("SingleCorrectOptionRepeater");
            else if (type.IsCheckList())
                path = path.Format("MultipleCorrectOptionRepeater");
            else if (type == QuestionItemType.TrueOrFalse)
                path = path.Format("TrueOrFalseOptionRepeater");
            else
                throw ApplicationError.Create("Unexpected question type: {0}", type.GetName());

            return (OptionWriteRepeater)container.LoadControl(path);
        }

        #endregion
    }
}