using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Reports.Controls
{
    public partial class BankCommentaryCriteria : SearchCriteriaController<BankCommentaryFilter>
    {
        #region Properties

        private Guid? DefaultBank => Guid.TryParse(Request["bank"], out var value) ? value : (Guid?)null;

        private Guid? DefaultForm => Guid.TryParse(Request["form"], out var value) ? value : (Guid?)null;

        private Guid? DefaultQuestion => Guid.TryParse(Request["question"], out var value) ? value : (Guid?)null;

        private string DefaultRole
        {
            get
            {
                var value = Request["role"];

                return !string.IsNullOrEmpty(value) && (value == CommentAuthorType.Administrator || value == CommentAuthorType.Candidate) ? value : null;
            }
        }

        private bool? DefaultIsShowAuthor
        {
            get
            {
                var value = Request["showAuthor"];

                return value == "1" ? true : value == "0" ? false : (bool?)null;
            }
        }

        #endregion

        #region SearchCriteriaController

        [Serializable]
        public class InnerFilter : BankCommentaryFilter
        {
            public Guid? InnerBankIdentifier { get; set; }
            public Guid? InnerFormIdentifier { get; set; }
            public Guid? InnerQuestionIdentifier { get; set; }
            public bool IsShowAuthor { get; set; } = true;
            public FlagType? QuestionFlag { get; set; }
        }

        public override BankCommentaryFilter Filter
        {
            get
            {
                var filter = new InnerFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    InnerBankIdentifier = BankIdentifier.Value,
                    InnerFormIdentifier = FormIdentifier.Value,
                    InnerQuestionIdentifier = QuestionIdentifier.Value,
                    QuestionFlag = QuestionFlag.EnumValue,
                    CommentFlag = CommentFlag.EnumValue,
                    CommentText = CommentText.Text,
                    CommentPosted = new DateTimeOffsetRange
                    {
                        Since = CommentPostedSince.Value,
                        Before = CommentPostedBefore.Value
                    },
                    CommentCategory = CommentCategory.FlattenOptions().All(x => x.Selected)
                        ? null
                        : CommentCategory.ValuesArray,
                    AttemptTag = AttemptTag.ValuesArray,
                    AttemptRegistrationEventIdentifier = AttemptEventIdentifier.Value,
                    EventFormat = EventFormat.Value,
                    AuthorRole = AuthorRole.Value,
                    IsShowAuthor = IsShowAuthor.ValueAsBoolean.Value,
                };

                if (!IsPostBack)
                    SetDefaultFilterValues(filter);

                SetupSubjects(filter);

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                if (!(value is InnerFilter filter))
                {
                    Clear();
                    return;
                }

                if (!IsPostBack)
                    SetDefaultFilterValues(filter);

                SetBankIdentifier(filter.InnerBankIdentifier);
                SetFormIdentifier(filter.InnerFormIdentifier);

                QuestionIdentifier.Value = filter.InnerQuestionIdentifier;

                QuestionFlag.EnumValue = filter.QuestionFlag;
                CommentFlag.EnumValue = filter.CommentFlag;
                CommentText.Text = filter.CommentText;
                CommentPostedSince.Value = filter.CommentPosted?.Since;
                CommentPostedBefore.Value = filter.CommentPosted?.Before;
                CommentCategory.Values = filter.CommentCategory.IsEmpty() || CommentCategory.FlattenOptions().All(x => filter.CommentCategory.Contains(x.Value))
                    ? null
                    : filter.CommentCategory;
                AttemptTag.Values = filter.AttemptTag;
                AttemptEventIdentifier.Value = filter.AttemptRegistrationEventIdentifier;
                EventFormat.Value = filter.EventFormat;
                AuthorRole.Value = filter.AuthorRole;
                IsShowAuthor.ValueAsBoolean = filter.IsShowAuthor;
            }
        }

        private static void SetupSubjects(InnerFilter filter)
        {
            IEnumerable<Question> questions = null;

            if (filter.InnerQuestionIdentifier.HasValue)
            {
                var question = filter.InnerBankIdentifier.HasValue
                    ? ServiceLocator.BankSearch.GetBankState(filter.InnerBankIdentifier.Value)?.FindQuestion(filter.InnerQuestionIdentifier.Value)
                    : ServiceLocator.BankSearch.GetFormData(filter.InnerFormIdentifier.Value)?.Specification.Bank.FindQuestion(filter.InnerQuestionIdentifier.Value);

                if (question != null)
                    questions = new[] { question };
                else
                    questions = new Question[0];
            }
            else if (filter.InnerFormIdentifier.HasValue)
            {
                var form = ServiceLocator.BankSearch.GetFormData(filter.InnerFormIdentifier.Value);

                questions = form.Sections.AsQueryable().SelectMany(x => x.Fields.Select(y => y.Question));

                if (!filter.QuestionFlag.HasValue)
                    filter.FormIdentifier = new[] { form.Identifier };
            }
            else if (filter.InnerBankIdentifier.HasValue)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(filter.InnerBankIdentifier.Value);

                questions = bank.Sets.AsQueryable().SelectMany(x => x.EnumerateAllQuestions());

                if (!filter.QuestionFlag.HasValue)
                {
                    filter.BankIdentifier = new[] { bank.Identifier };
                    filter.SpecificationIdentifier = bank.Specifications.Select(x => x.Identifier).ToArray();
                    filter.FormIdentifier = bank.Specifications.SelectMany(x => x.EnumerateAllForms()).Select(x => x.Identifier).ToArray();
                }
            }

            if (questions != null)
            {
                if (filter.QuestionFlag.HasValue)
                    questions = questions.Where(x => x.Flag == filter.QuestionFlag.Value);

                if (questions.Any())
                {
                    filter.QuestionIdentifier = questions.Select(x => x.Identifier).ToArray();
                    filter.FieldIdentifier = questions.SelectMany(x => x.Fields).Select(x => x.Identifier).ToArray();
                }
                else
                {
                    filter.QuestionIdentifier = new[] { Guid.Empty };
                }
            }
            else if (filter.QuestionFlag.HasValue)
            {
                var entities = ServiceLocator.BankSearch.GetQuestions(new QBankQuestionFilter
                {
                    OrganizationIdentifier = filter.OrganizationIdentifier.Value,
                    QuestionFlag = filter.QuestionFlag.Value.GetName()
                });

                filter.QuestionIdentifier = entities.Count > 0
                    ? entities.Select(x => x.QuestionIdentifier).ToArray()
                    : new[] { Guid.Empty };
            }
        }

        public override void Clear()
        {
            SetBankIdentifier(null);

            QuestionFlag.Value = null;
            CommentFlag.Value = null;
            CommentText.Text = null;
            CommentPostedSince.Value = null;
            CommentPostedBefore.Value = null;
            CommentCategory.ClearSelection();
            AttemptTag.ClearSelection();
            AttemptEventIdentifier.Value = null;
            EventFormat.ClearSelection();
            AuthorRole.ClearSelection();
            IsShowAuthor.ClearSelection();

            SetDefaultInputValues();
        }

        private void SetDefaultFilterValues(InnerFilter filter)
        {
            if (DefaultBank.HasValue)
                filter.InnerBankIdentifier = DefaultBank.Value;

            if (DefaultForm.HasValue)
                filter.InnerFormIdentifier = DefaultForm.Value;

            if (DefaultQuestion.HasValue)
                filter.InnerQuestionIdentifier = DefaultQuestion.Value;

            if (!string.IsNullOrEmpty(DefaultRole))
                filter.AuthorRole = DefaultRole;

            if (DefaultIsShowAuthor.HasValue)
                filter.IsShowAuthor = DefaultIsShowAuthor.Value;
        }

        private void SetDefaultInputValues()
        {
            if (DefaultBank.HasValue)
                SetBankIdentifier(DefaultBank.Value);

            if (DefaultForm.HasValue)
                SetFormIdentifier(DefaultForm.Value);

            if (DefaultQuestion.HasValue)
                QuestionIdentifier.Value = DefaultQuestion.Value;

            if (!string.IsNullOrEmpty(DefaultRole))
            {
                var option = AuthorRole.FindOptionByValue(DefaultRole, true);
                if (option != null)
                    option.Selected = true;
            }

            if (DefaultIsShowAuthor.HasValue)
                IsShowAuthor.ValueAsBoolean = DefaultIsShowAuthor.Value;
        }

        private void SetBankIdentifier(Guid? value)
        {
            BankIdentifier.Value = value;
            OnBankIdentifierChanged();
        }

        private void SetFormIdentifier(Guid? value)
        {
            FormIdentifier.Value = value;
            OnFormIdentifierChanged();
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BankIdentifier.AutoPostBack = true;
            BankIdentifier.ValueChanged += BankIdentifier_ValueChanged;

            FormIdentifier.AutoPostBack = true;
            FormIdentifier.ValueChanged += FormIdentifier_ValueChanged;

            QuestionIdentifier.PageSize = int.MaxValue;
            QuestionIdentifier.EntityName = "Question";
            QuestionIdentifier.NeedDataCount += QuestionIdentifier_NeedDataCount;
            QuestionIdentifier.NeedDataSource += QuestionIdentifier_NeedDataSource;
            QuestionIdentifier.NeedSelectedItems += QuestionIdentifier_NeedSelectedItems;
        }

        #endregion

        #region Event handlers

        private void BankIdentifier_ValueChanged(object sender, EventArgs e) => OnBankIdentifierChanged();

        private void OnBankIdentifierChanged()
        {
            FormIdentifier.Filter.BankIdentifier = BankIdentifier.Value;
            FormIdentifier.Value = null;

            OnFormIdentifierChanged();
        }

        private void FormIdentifier_ValueChanged(object sender, EventArgs e) => OnFormIdentifierChanged();

        private void OnFormIdentifierChanged()
        {
            QuestionIdentifier.Value = null;
            QuestionIdentifier.Enabled = BankIdentifier.Value.HasValue || FormIdentifier.Value.HasValue;
        }

        private void QuestionIdentifier_NeedDataCount(object sender, FindEntity.CountArgs args)
        {
            args.Count = int.MaxValue;
        }

        private void QuestionIdentifier_NeedDataSource(object sender, FindEntity.DataArgs args)
        {
            var items = GetQuestionIdentifierItems();

            if (args.Keyword.IsNotEmpty())
                items = items.Where(x => x.Text.IndexOf(args.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            args.Items = items.ToArray();
        }

        private void QuestionIdentifier_NeedSelectedItems(object sender, FindEntity.ItemsArgs args)
        {
            args.Items = GetQuestionIdentifierItems().Where(x => args.Identifiers.Contains(x.Value)).ToArray();
        }

        #endregion

        #region Helper methods

        private IQueryable<FindEntity.DataItem> GetQuestionIdentifierItems()
        {
            IQueryable<Question> questions = null;

            if (FormIdentifier.Value.HasValue)
            {
                var form = ServiceLocator.BankSearch.GetFormData(FormIdentifier.Value.Value);
                questions = form.Sections.AsQueryable().SelectMany(x => x.Fields.Select(y => y.Question));
            }
            else if (BankIdentifier.Value.HasValue)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankIdentifier.Value.Value);
                questions = bank.Sets.AsQueryable().SelectMany(x => x.EnumerateAllQuestions());
            }
            else
            {
                return Enumerable.Empty<FindEntity.DataItem>().AsQueryable();
            }

            return questions.Select(
                (q, i) => new FindEntity.DataItem
                {
                    Value = q.Identifier,
                    Text = $"{i + 1}. {q.Content.Title.Default}"
                });
        }

        #endregion
    }
}