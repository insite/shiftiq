using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionOrderingDetails : BaseUserControl
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        private ControlData Data
        {
            get => (ControlData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        private bool ShowLabels
        {
            get => (bool)(ViewState[nameof(ShowLabels)] ?? false);
            set => ViewState[nameof(ShowLabels)] = value;
        }

        private ItemCollection<OptionItem> Options
        {
            get => (ItemCollection<OptionItem>)ViewState[nameof(Options)];
            set => ViewState[nameof(Options)] = value;
        }

        private ItemCollection<SolutionItem> Solutions
        {
            get => (ItemCollection<SolutionItem>)ViewState[nameof(Solutions)];
            set => ViewState[nameof(Solutions)] = value;
        }

        public bool IsReadOnly
        {
            get => (bool)(ViewState[nameof(IsReadOnly)] ?? false);
            set => ViewState[nameof(IsReadOnly)] = value;
        }

        #endregion

        #region Fields

        private bool _rebindOptions = false;
        private bool _rebindSolutions = false;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OptionRepeater.DataBinding += OptionRepeater_DataBinding;
            OptionRepeater.ItemCreated += OptionRepeater_ItemCreated;
            OptionRepeater.ItemCommand += OptionRepeater_ItemCommand;

            ImageSelectorRefresh.Click += ImageSelectorRefresh_Click;

            SolutionRepeater.DataBinding += SolutionRepeater_DataBinding;
            SolutionRepeater.ItemDataBound += SolutionRepeater_ItemDataBound;
            SolutionRepeater.ItemCommand += SolutionRepeater_ItemCommand;

            ToggleLabelsButton.Click += ToggleLabelsButton_Click;
            AddOptionButton.Click += AddOptionButton_Click;
            AddSolutionButton.Click += AddSolutionButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            InitCollections();

            base.OnLoad(e);

            if (!IsPostBack)
                return;

            foreach (RepeaterItem item in OptionRepeater.Items)
                SetOptionValues(item, Options.FindByKey(GetItemKey(item)));

            foreach (RepeaterItem item in SolutionRepeater.Items)
                SetSolutionValues(item, Solutions.FindByKey(GetItemKey(item)));

            if (ReorderItems(OptionsOrder.Value, Options))
                _rebindOptions = true;

            if (ReorderItems(SolutionsOrder.Value, Solutions))
                _rebindSolutions = true;

            OptionsOrder.Value = null;
            SolutionsOrder.Value = null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_rebindOptions)
                OptionRepeater.DataBind();

            if (_rebindSolutions)
                SolutionRepeater.DataBind();

            AddOptionButton.Visible = !IsReadOnly;
            AddSolutionButton.Visible = !IsReadOnly;

            base.OnPreRender(e);

            RegisterInitScript();
        }

        private void InitCollections()
        {
            if (Options != null)
            {
                Options.ItemAdded += Options_ItemAdded;
                Options.ItemRemoved += Options_ItemRemoved;
            }

            if (Solutions != null)
            {
                Solutions.ItemAdded += Solutions_ItemAdded;
            }
        }

        private void RegisterInitScript()
        {
            var script = new StringBuilder();

            script.AppendFormat("optionWriteRepeater.initReorder('{0}','{1}');", OptionRepeater.ClientID, OptionsOrder.ClientID);
            script.AppendLine();

            script.AppendFormat("optionWriteRepeater.initReorder('{0}','{1}');", SolutionRepeater.ClientID, SolutionsOrder.ClientID);
            script.AppendLine();

            foreach (RepeaterItem item in SolutionRepeater.Items)
            {
                var optionsOrder = item.FindControl("OptionsOrder");

                script.AppendFormat("orderingDetails.initReorder('{0}');", optionsOrder.ClientID);
                script.AppendLine();
            }

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(QuestionOrderingDetails),
                "init",
                script.ToString(),
                true);
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < OptionRepeater.Items.Count; i++)
                SetupOptionRepeaterValidationGroup(OptionRepeater.Items[i], groupName);
        }

        private void SetupOptionRepeaterValidationGroup(RepeaterItem item, string groupName)
        {
            var textValidator = (BaseValidator)item.FindControl("TextRequiredValidator");
            textValidator.ValidationGroup = groupName;
        }

        #endregion

        #region Event handlers

        private void Options_ItemAdded(object sender, ItemEventArgs<OptionItem> args)
        {
            foreach (var solution in Solutions)
                solution.Options.Add(args.Item.Key);
        }

        private void Options_ItemRemoved(object sender, ItemEventArgs<OptionItem> args)
        {
            foreach (var solution in Solutions)
                solution.Options.Remove(args.Item.Key);
        }

        private void Solutions_ItemAdded(object sender, ItemEventArgs<SolutionItem> args)
        {
            args.Item.Options = Options.OrderBy(x => x.Key).Select(x => x.Key).ToList();
        }

        private void OptionRepeater_DataBinding(object sender, EventArgs e)
        {
            UpdateOptionsReadOnly();

            OptionRepeater.DataSource = Options
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Key);
        }

        private void OptionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupOptionRepeaterValidationGroup(e.Item, ValidationGroup);

            var optionUpload = (EditorUpload)e.Item.FindControl("OptionUpload");
            optionUpload.Custom += OptionUpload_Custom;
        }

        private void OptionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                if (IsReadOnly)
                    return;

                var option = Options.FindByKey(GetItemKey(e.Item));
                if (option.IsReadOnly || !Options.Remove(option))
                    return;

                Options.UpdateItemIndexes();

                _rebindOptions = true;
                _rebindSolutions = true;
            }
        }

        private void OptionUpload_Custom(object sender, EditorUpload.CustomEventArgs args)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(args.File.FileName);
            var extension = System.IO.Path.GetExtension(args.File.FileName);
            var filename = StringHelper.ToIdentifier(name) + extension;

            var attachmentEntity = InSite.Admin.Assessments.Attachments.Forms.Add.AttachFile(Data.BankId, Data.BankAsset, filename, filename, args.File.InputStream);
            var uploadEntity = UploadSearch.Select(attachmentEntity.Upload);

            var repeaterItem = (RepeaterItem)((Control)sender).NamingContainer;
            var optionEditor = (MarkdownEditor)repeaterItem.FindControl("OptionEditor");

            optionEditor.SetupCallback(args.Callback, uploadEntity.Name, FileHelper.GetUrl(uploadEntity.NavigateUrl));
        }

        private void ImageSelectorRefresh_Click(object sender, EventArgs e)
        {
            ImageSelectorRepeater.LoadData(Data.BankId);

            var hasImages = !ImageSelectorRepeater.IsEmpty;

            ImageSelectorRepeater.Visible = hasImages;
            ImageSelectorEmptyMessage.Visible = !hasImages;

            if (hasImages)
                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "initimgs_" + ClientID,
                    "orderingDetails.initImgSelector();",
                    true);
        }

        private void SolutionRepeater_DataBinding(object sender, EventArgs e)
        {
            UpdateSolutionsReadOnly();

            SolutionRepeater.DataSource = Solutions
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Key);
        }

        private void SolutionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            BindSolutionOptions(e.Item, (SolutionItem)e.Item.DataItem);
        }

        private void SolutionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                if (IsReadOnly)
                    return;

                var solution = Solutions.FindByKey(GetItemKey(e.Item));
                if (solution.IsReadOnly || !Solutions.Remove(solution))
                    return;

                Solutions.UpdateItemIndexes();

                _rebindSolutions = true;
            }
        }

        private void ToggleLabelsButton_Click(object sender, EventArgs e)
        {
            ShowLabels = !ShowLabels;

            SetupLabelButton();
        }

        private void AddOptionButton_Click(object sender, EventArgs e)
        {
            Options.Add();
            Options.UpdateItemIndexes();

            _rebindOptions = true;
            _rebindSolutions = true;
        }

        private void AddSolutionButton_Click(object sender, EventArgs e)
        {
            Solutions.Add();
            Solutions.UpdateItemIndexes();

            _rebindSolutions = true;
        }

        #endregion

        #region Methods (public)

        public void InitData(BankState bank)
        {
            Data = new ControlData(bank);
        }

        public void LoadData()
        {
            ShowLabels = false;
            TopLabel.Text.Clear();
            BottomLabel.Text.Clear();

            SetupLabelButton();

            Options = new ItemCollection<OptionItem>();
            Solutions = new ItemCollection<SolutionItem>();

            InitCollections();

            Options.Add();
            Options.Add();
            Options.Add();
            Options.Add();

            Solutions.Add();

            Options.UpdateItemIndexes();
            Solutions.UpdateItemIndexes();

            _rebindOptions = true;
            _rebindSolutions = true;
        }

        public void LoadData(Question question)
        {
            Data = new ControlData(question);

            var ordering = question.Ordering;

            ShowLabels = ordering.Label.Show;
            TopLabel.Text.Set(ordering.Label.TopContent.Title);
            BottomLabel.Text.Set(ordering.Label.BottomContent.Title);

            SetupLabelButton();

            Options = new ItemCollection<OptionItem>();
            Solutions = new ItemCollection<SolutionItem>();

            InitCollections();

            foreach (var option in ordering.Options)
            {
                var item = Options.Add(option.Identifier);
                item.Number = option.Number;
                item.Text.Set(option.Content.Title);
            }

            foreach (var solution in ordering.Solutions)
            {
                var item = Solutions.Add(solution.Identifier);
                item.Points = solution.Points;
                item.CutScore = solution.CutScore;
                item.Options = solution.Options.Select(o => Options.First(x => x.Identifier == o).Key).ToList();
            }

            Options.UpdateItemIndexes();
            Solutions.UpdateItemIndexes();

            _rebindOptions = true;
            _rebindSolutions = true;
        }

        public Ordering GetOrdering()
        {
            var result = new Ordering();

            SetLabelInputValues(result.Label);

            var optionMapping = new Dictionary<int, Guid>();

            foreach (var item in Options)
            {
                var option = result.AddOption(UniqueIdentifier.Create());
                option.Content.Title.Set(item.Text);
                optionMapping.Add(item.Key, option.Identifier);
            }

            foreach (var item in Solutions)
            {
                var solution = result.AddSolution(UniqueIdentifier.Create());
                solution.Points = item.Points;
                solution.CutScore = item.CutScore;
                solution.ReorderOptions(
                    item.Options
                        .Select((x, i) => (Index: i, Identifier: optionMapping[x]))
                        .ToDictionary(x => x.Identifier, x => x.Index));
            }

            return result;
        }

        public Command[] GetCommands(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (Options == null || Solutions == null)
                return null;

            var bankId = question.Set.Bank.Identifier;
            var commands = new List<Command>();
            var optionMapping = new Dictionary<int, Guid>();
            var optionOrder = new List<Guid>();
            var solutionOrder = new List<Guid>();

            AppendLabelCommands(question, commands);
            AppendOptionsCommands(question, commands, optionMapping, optionOrder);
            AppendSolutionsCommands(question, commands, optionMapping, solutionOrder);

            commands.Add(new ReorderQuestionOrderingOptions(bankId, question.Identifier, optionOrder.ToArray()));
            commands.Add(new ReorderQuestionOrderingSolutions(bankId, question.Identifier, solutionOrder.ToArray()));

            return commands.ToArray();
        }

        private void AppendLabelCommands(Question question, List<Command> commands)
        {
            var bankId = question.Set.Bank.Identifier;
            var questionId = question.Identifier;

            var label = new OrderingLabel();
            SetLabelInputValues(label);
            commands.Add(new ChangeQuestionOrderingLabel(bankId, questionId, label));
        }

        private void AppendOptionsCommands(Question question, List<Command> commands, Dictionary<int, Guid> optionMapping, List<Guid> optionOrder)
        {
            var bankId = question.Set.Bank.Identifier;
            var questionId = question.Identifier;

            foreach (var item in Options.EnumerateRemoved())
            {
                if (item.Identifier != Guid.Empty && !item.IsReadOnly)
                    commands.Add(new DeleteQuestionOrderingOption(bankId, questionId, item.Identifier));
            }

            foreach (var item in Options.OrderBy(x => x.Sequence).ThenBy(x => x.Key))
            {
                var optionId = item.Identifier;

                if (optionId == Guid.Empty)
                {
                    optionId = UniqueIdentifier.Create();
                    commands.Add(new AddQuestionOrderingOption(bankId, questionId, optionId, new ContentTitle { Title = item.Text }));
                }
                else
                {
                    commands.Add(new ChangeQuestionOrderingOption(bankId, questionId, optionId, new ContentTitle { Title = item.Text }));
                }

                optionMapping.Add(item.Key, optionId);
                optionOrder.Add(optionId);
            }
        }

        private void AppendSolutionsCommands(Question question, List<Command> commands, Dictionary<int, Guid> optionMapping, List<Guid> solutionOrder)
        {
            var bankId = question.Set.Bank.Identifier;
            var questionId = question.Identifier;

            foreach (var item in Solutions.EnumerateRemoved())
            {
                if (item.Identifier != Guid.Empty && !item.IsReadOnly)
                    commands.Add(new DeleteQuestionOrderingSolution(bankId, questionId, item.Identifier));
            }

            foreach (var item in Solutions.OrderBy(x => x.Sequence).ThenBy(x => x.Key))
            {
                var solutionId = item.Identifier;

                if (solutionId == Guid.Empty)
                {
                    solutionId = UniqueIdentifier.Create();
                    commands.Add(new AddQuestionOrderingSolution(bankId, questionId, solutionId, item.Points, item.CutScore));
                }
                else
                {
                    commands.Add(new ChangeQuestionOrderingSolution(bankId, questionId, solutionId, item.Points, item.CutScore));
                }

                commands.Add(new ReorderQuestionOrderingSolutionOptions(bankId, questionId, solutionId, item.Options.Select(x => optionMapping[x]).ToArray()));

                solutionOrder.Add(solutionId);
            }
        }

        public string GetError()
        {
            if (Options.Count == 0)
                return "The question has no options.";

            if (Solutions.Count == 0)
                return "The question has no solutions.";

            if (Solutions.All(x => x.Points == default))
                return "The question contains no correct solution.";

            if (Solutions.Any(s1 => Solutions.Where(s2 => s1 != s2).Any(s2 => s1.Options.SequenceEqual(s2.Options))))
                return "The question contains two or more of the same solutions.";

            if (TryGetRemoveError(out var removeError))
                return removeError;

            return null;
        }

        private bool TryGetRemoveError(out string error)
        {
            var unremovedOptions = new List<int>();
            var unremovedSolutions = new List<int>();

            UpdateOptionsReadOnly();
            UpdateSolutionsReadOnly();

            foreach (var option in Options.EnumerateRemoved())
            {
                if (!option.IsReadOnly)
                    continue;

                Options.Unremove(option);
                unremovedOptions.Add(option.Key);
            }

            foreach (var solution in Solutions.EnumerateRemoved())
            {
                if (!solution.IsReadOnly)
                    continue;

                Solutions.Unremove(solution);
                unremovedSolutions.Add(solution.Key);
            }

            error = string.Empty;

            if (unremovedOptions.Count > 0)
            {
                error = "Some of deleted options can't be removed: "
                    + string.Join(", ", unremovedOptions.Select(x => $"Option #{x}"))
                    + Environment.NewLine;

                Options.UpdateItemIndexes();

                _rebindOptions = true;
            }

            if (unremovedSolutions.Count > 0)
            {
                error = "Some of deleted solutions can't be removed: "
                    + string.Join(", ", unremovedSolutions.Select(x => $"Solution {Calculator.ToBase26(x)}"))
                    + Environment.NewLine;

                Solutions.UpdateItemIndexes();

                _rebindSolutions = true;
            }

            return error.IsNotEmpty();
        }

        #endregion

        #region Methods (repeater helpers)

        private int GetItemKey(RepeaterItem item) => int.Parse(((ITextControl)item.FindControl("Key")).Text);

        private void SetOptionValues(RepeaterItem item, OptionItem option)
        {
            var text = (EditorTranslation)item.FindControl("OptionText");
            option.Text.Set(text.Text);
        }

        private void SetSolutionValues(RepeaterItem item, SolutionItem solution)
        {
            var points = (NumericBox)item.FindControl("Points");
            solution.Points = points.ValueAsDecimal ?? 0;

            var cutScore = (NumericBox)item.FindControl("CutScore");
            solution.CutScore = cutScore.ValueAsDecimal;

            var optionsOrder = (HiddenField)item.FindControl("OptionsOrder");
            if (optionsOrder.Value.IsNotEmpty())
            {
                var options = optionsOrder.Value
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .Distinct()
                    .ToList();
                var isChanged = solution.Options.Count == options.Count
                    && solution.Options.All(x => options.Contains(x))
                    && !solution.Options.SequenceEqual(options);

                if (isChanged)
                {
                    solution.Options = options;
                    BindSolutionOptions(item, solution);
                }

                optionsOrder.Value = null;
            }
        }

        private void BindSolutionOptions(RepeaterItem item, SolutionItem solution)
        {
            var optionRepeater = (Repeater)item.FindControl("OptionRepeater");
            optionRepeater.DataSource = solution.Options
                .Select(x => Options.FindByKey(x));
            optionRepeater.DataBind();
        }

        #endregion

        #region Methods (helpers)

        private void SetupLabelButton()
        {
            TopLabelField.Visible = ShowLabels;
            BottomLabelField.Visible = ShowLabels;

            if (ShowLabels)
            {
                ToggleLabelsButton.Icon = "fas fa-eye-slash";
                ToggleLabelsButton.Text = "Hide Labels";
            }
            else
            {
                ToggleLabelsButton.Icon = "fas fa-eye";
                ToggleLabelsButton.Text = "Show Labels";
            }
        }

        private void SetLabelInputValues(OrderingLabel label)
        {
            label.Show = ShowLabels;
            label.TopContent.Title.Set(TopLabel.Text);
            label.BottomContent.Title.Set(BottomLabel.Text);
        }

        private bool ReorderItems(string inputOrder, IReadOnlyList<BaseItem> items)
        {
            var result = false;

            if (inputOrder.IsEmpty())
                return result;

            var array = inputOrder.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            if (array.Length != items.Count)
                return result;

            for (var i = 0; i < array.Length; i++)
            {
                var key = int.Parse(array[i]);
                var item = items.First(x => x.Key == key);
                var sequence = i + 1;

                if (item.Sequence != sequence)
                {
                    item.Sequence = sequence;
                    result = true;
                }
            }

            return result;
        }

        private void UpdateOptionsReadOnly()
        {
            var questionId = Data.QuestionId;
            if (!questionId.HasValue)
                return;

            var locked = new HashSet<int>(ServiceLocator.AttemptSearch.GetAttemptExistOptionKeys(questionId.Value));
            foreach (var option in Options.EnumerateAll())
                option.IsReadOnly = IsReadOnly || option.Identifier != Guid.Empty && locked.Contains(option.Number);
        }

        private void UpdateSolutionsReadOnly()
        {
            var questionId = Data.QuestionId;
            if (!questionId.HasValue)
                return;

            var locked = new HashSet<Guid>(ServiceLocator.AttemptSearch.GetAttemptExistSolutionIds(questionId.Value));
            foreach (var solution in Solutions.EnumerateAll())
                solution.IsReadOnly = IsReadOnly || solution.Identifier != Guid.Empty && locked.Contains(solution.Identifier);
        }

        #endregion
    }
}