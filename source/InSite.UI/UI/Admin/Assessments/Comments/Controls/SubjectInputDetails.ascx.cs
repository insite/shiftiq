using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Comments.Controls
{
    public partial class SubjectInputDetails : BaseUserControl
    {
        #region Properties

        private Guid? BankIdentifier
        {
            get => (Guid?)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        public Guid SubjectIdentifier
        {
            get
            {
                if (QuestionIdentifier.HasValue)
                    return QuestionIdentifier.Value;
                else if (FormIdentifier.HasValue)
                    return FormIdentifier.Value;
                else if (SpecIdentifier.HasValue)
                    return SpecIdentifier.Value;
                else
                    return BankIdentifier.Value;
            }
        }

        public CommentType SubjectType
        {
            get
            {
                if (QuestionIdentifier.HasValue)
                    return FormIdentifier.HasValue ? CommentType.Field : CommentType.Question;
                else if (FormIdentifier.HasValue)
                    return CommentType.Form;
                else if (SpecIdentifier.HasValue)
                    return CommentType.Specification;
                else
                    return CommentType.Bank;
            }
        }

        public Guid? SpecIdentifier
        {
            get => SpecSelector.ValueAsGuid;
            set
            {
                SpecSelector.ValueAsGuid = value;

                BindFormSelector();
            }
        }

        public Guid? FormIdentifier
        {
            get => FormSelector.ValueAsGuid;
            set
            {
                FormSelector.ValueAsGuid = value;

                OnFormSelected();
            }
        }

        public Guid? QuestionIdentifier
        {
            get => QuestionSelector.Value;
            set => QuestionSelector.Value = value;
        }

        private List<Tuple<Guid, string>> Questions
        {
            get => (List<Tuple<Guid, string>>)ViewState[nameof(Questions)];
            set => ViewState[nameof(Questions)] = value;
        }

        private List<Tuple<Guid, string>> Forms
        {
            get => (List<Tuple<Guid, string>>)ViewState[nameof(Forms)];
            set => ViewState[nameof(Forms)] = value;
        }

        private Dictionary<Guid, int[]> SpecFormMapping
        {
            get => (Dictionary<Guid, int[]>)ViewState[nameof(SpecFormMapping)];
            set => ViewState[nameof(SpecFormMapping)] = value;
        }

        private Dictionary<Guid, Tuple<Guid, int>[]> FormFieldMapping
        {
            get => (Dictionary<Guid, Tuple<Guid, int>[]>)ViewState[nameof(FormFieldMapping)];
            set => ViewState[nameof(FormFieldMapping)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SpecSelector.AutoPostBack = true;
            SpecSelector.ValueChanged += SpecSelector_ValueChanged;

            FormSelector.AutoPostBack = true;
            FormSelector.ValueChanged += FormSelector_ValueChanged;

            QuestionSelector.PageSize = int.MaxValue;
            QuestionSelector.EntityName = "Question";
            QuestionSelector.NeedDataCount += QuestionSelector_NeedDataCount;
            QuestionSelector.NeedDataSource += QuestionSelector_NeedDataSource;
            QuestionSelector.NeedSelectedItems += QuestionSelector_NeedSelectedItems;
        }

        #endregion

        #region Event handlers

        private void SpecSelector_ValueChanged(object sender, EventArgs e) => BindFormSelector();

        private void FormSelector_ValueChanged(object sender, EventArgs e) => OnFormSelected();

        private void OnFormSelected()
        {
            QuestionSelector.Value = null;
        }

        private void QuestionSelector_NeedDataCount(object sender, FindEntity.CountArgs args)
        {
            args.Count = int.MaxValue;
        }

        private void QuestionSelector_NeedDataSource(object sender, FindEntity.DataArgs args)
        {
            var items = GetQuestionDataSource();

            if (args.Keyword.IsNotEmpty())
                items = items.Where(x => x.Text.IndexOf(args.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            args.Items = items.ToArray();
        }

        private void QuestionSelector_NeedSelectedItems(object sender, FindEntity.ItemsArgs args)
        {
            args.Items = GetQuestionDataSource().Where(x => args.Identifiers.Contains(x.Value)).ToArray();
        }

        #endregion

        #region Methods (data bind)

        public void LoadData(BankState bank)
        {
            BankIdentifier = bank.Identifier;

            Forms = new List<Tuple<Guid, string>>();
            Questions = new List<Tuple<Guid, string>>();
            SpecFormMapping = new Dictionary<Guid, int[]>();
            FormFieldMapping = new Dictionary<Guid, Tuple<Guid, int>[]>();

            var indexQuestionMapping = new Dictionary<Guid, int>();

            foreach (var q in bank.Sets.SelectMany(x => x.EnumerateAllQuestions()))
            {
                indexQuestionMapping.Add(q.Identifier, Questions.Count);
                Questions.Add(new Tuple<Guid, string>(q.Identifier, q.Content.Title.Default));
            }

            SpecSelector.Items.Clear();
            SpecSelector.Items.Add(new ComboBoxOption());

            foreach (var spec in bank.Specifications)
            {
                var formIndexes = new List<int>();

                foreach (var form in spec.EnumerateAllForms())
                {
                    formIndexes.Add(Forms.Count);
                    Forms.Add(new Tuple<Guid, string>(form.Identifier, form.Name.IfNullOrEmpty(() => (form.Content.Title?.Default).IfNullOrEmpty("(Untitled)"))));
                    FormFieldMapping.Add(form.Identifier, form.Sections.SelectMany(x => x.Fields).Select(x => new Tuple<Guid, int>(x.Identifier, indexQuestionMapping[x.Question.Identifier])).ToArray());
                }

                SpecSelector.Items.Add(new ComboBoxOption(spec.Name, spec.Identifier.ToString()));

                SpecFormMapping.Add(spec.Identifier, formIndexes.ToArray());
            }

            BindFormSelector();
        }

        private void BindFormSelector()
        {
            var specId = SpecIdentifier;
            var forms = specId.HasValue && SpecFormMapping.ContainsKey(specId.Value)
                ? SpecFormMapping[specId.Value].Select(i => Forms[i])
                : Forms;

            FormSelector.LoadItems(forms, "Item1", "Item2");

            OnFormSelected();
        }

        private IQueryable<FindEntity.DataItem> GetQuestionDataSource()
        {
            var specId = SpecIdentifier;
            var formId = FormIdentifier;

            if (formId.HasValue && FormFieldMapping.ContainsKey(formId.Value))
            {
                return FormFieldMapping[formId.Value].AsQueryable().Select(
                    (f, i) => new FindEntity.DataItem
                    {
                        Value = f.Item1,
                        Text = $"{i + 1}. {StringHelper.RemoveNewLines(Markdown.ToText(Questions[f.Item2].Item2), true)}"
                    });
            }
            else if (specId.HasValue && SpecFormMapping.ContainsKey(specId.Value))
            {
                var filter = SpecFormMapping[specId.Value]
                    .Select(i => Forms[i])
                    .SelectMany(f => FormFieldMapping[f.Item1])
                    .Select(x => x.Item2)
                    .ToHashSet();

                return Questions.AsQueryable().Where((x, i) => filter.Contains(i)).Select(
                    (x, i) => new FindEntity.DataItem
                    {
                        Value = x.Item1,
                        Text = $"{i + 1}. {StringHelper.RemoveNewLines(Markdown.ToText(x.Item2), true)}"
                    });
            }
            else
            {
                return Questions.AsQueryable().Select(
                    (x, i) => new FindEntity.DataItem
                    {
                        Value = x.Item1,
                        Text = $"{i + 1}. {StringHelper.RemoveNewLines(Markdown.ToText(x.Item2), true)}"
                    });
            }
        }

        #endregion
    }
}