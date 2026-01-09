using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public static class QuestionPrintHelper
    {
        public interface IQuestionInfo
        {
            int PrimarySequence { get; }
            int? SecondarySequence { get; }
            string CompetencyName { get; }
            string TaxonomyName { get; }
            Domain.Banks.Section Section { get; }
            Question BankQuestion { get; }
            AttemptQuestion AttemptQuestion { get; }

            IReadOnlyList<ICommentInfo> GetAdminComments();
        }

        public interface ICommentInfo
        {
            string AuthorName { get; }
            DateTimeOffset PostedOn { get; }
            string IconHtml { get; }
            string Text { get; }
            bool HasFlag { get; }
        }

        private class QuestionCollection : List<QuestionInfo>
        {
            private Dictionary<Guid, ICommentInfo[]> _comments;

            public QuestionCollection() : base()
            {

            }

            public QuestionCollection(int capacity) : base(capacity)
            {

            }

            public IReadOnlyList<ICommentInfo> FindAdminComments(Guid id)
            {
                if (_comments == null)
                {
                    var buffer = new List<Tuple<Guid, Comment[]>>();
                    var authorFilter = new HashSet<Guid>();

                    foreach (var q in this)
                    {
                        var comments = q.BankQuestion.Comments
                            .Where(x => x.AuthorRole == CommentAuthorType.Administrator)
                            .OrderBy(x => x.Posted)
                            .ToArray(); ;

                        foreach (var c in comments)
                            authorFilter.Add(c.Author);

                        buffer.Add(new Tuple<Guid, Comment[]>(q.BankQuestion.Identifier, comments));
                    }

                    var authors = UserSearch
                        .Bind(
                            x => new { x.UserIdentifier, x.FullName },
                            new UserFilter { IncludeUserIdentifiers = authorFilter.ToArray() })
                        .ToDictionary(x => x.UserIdentifier, x => x.FullName);

                    _comments = buffer.ToDictionary(
                        x => x.Item1,
                        x => x.Item2.Select(y => (ICommentInfo)new CommentInfo
                        {
                            AuthorName = authors.TryGetValue(y.Author, out var author) ? author : y.Author.ToString(),
                            PostedOn = y.Posted,
                            IconHtml = y.Flag.ToIconHtml(),
                            Text = Markdown.ToHtml(y.Text),
                            HasFlag = y.Flag != FlagType.None
                        }).ToArray());
                }

                if (!_comments.TryGetValue(id, out var result))
                    _comments.Add(id, result = new CommentInfo[0]);

                return result;
            }
        }

        private class QuestionInfo : IQuestionInfo
        {
            public int PrimarySequence { get; set; }
            public int? SecondarySequence { get; set; }
            public string CompetencyName { get; set; }
            public string TaxonomyName { get; set; }
            public Domain.Banks.Section Section { get; set; }
            public Question BankQuestion { get; set; }
            public AttemptQuestion AttemptQuestion { get; set; }

            private QuestionCollection _collection;

            public QuestionInfo(QuestionCollection collection)
            {
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            IReadOnlyList<ICommentInfo> IQuestionInfo.GetAdminComments() => _collection.FindAdminComments(BankQuestion.Identifier);
        }

        private class CommentInfo : ICommentInfo
        {
            public string AuthorName { get; set; }
            public DateTimeOffset PostedOn { get; set; }
            public string IconHtml { get; set; }
            public string Text { get; set; }
            public bool HasFlag { get; set; }
        }

        public static IQuestionInfo[] GetQuestions(BankState bank)
        {
            var result = new QuestionCollection();

            foreach (var question in bank.Sets.SelectMany(x => x.Questions))
                result.Add(new QuestionInfo(result)
                {
                    PrimarySequence = question.BankIndex + 1,
                    BankQuestion = question,
                    AttemptQuestion = AttemptStarter.CreateQuestion(question, false, Language.Default)
                });

            return result.ToArray();
        }

        public static IQuestionInfo[] GetQuestions(Form form)
        {
            var questions = AttemptHelper.CreateAttemptQuestions(form, false, Language.Default);
            if (questions.Length == 0)
                return new IQuestionInfo[0];

            var result = new QuestionCollection(questions.Length);

            if (form.Specification.Type == SpecificationType.Static)
                BindStaticForm(form, questions, result);
            else
                BindDynamicForm(form, questions, result);

            return result.ToArray();
        }

        private static void BindStaticForm(Form form, IEnumerable<AttemptQuestion> questions, QuestionCollection collection)
        {
            var fieldMapping = new Dictionary<Guid, Field>();
            var optionMapping = new Dictionary<int, Option>();

            foreach (var f in form.Sections.SelectMany(x => x.Fields))
            {
                fieldMapping.Add(f.QuestionIdentifier, f);

                foreach (var o in f.Question.Options)
                    optionMapping.Add(o.Number, o);
            }

            var qSequence = 1;

            foreach (var question in questions)
            {
                var field = fieldMapping[question.Identifier];
                collection.Add(new QuestionInfo(collection)
                {
                    PrimarySequence = field.Question.BankIndex + 1,
                    SecondarySequence = qSequence++,
                    Section = field.Section,
                    BankQuestion = field.Question,
                    AttemptQuestion = question
                });
            }
        }

        private static void BindDynamicForm(Form form, IEnumerable<AttemptQuestion> questions, QuestionCollection collection)
        {
            var qSequence = 1;

            foreach (var q in questions)
            {
                var question = form.Specification.Bank.FindQuestion(q.Identifier);
                collection.Add(new QuestionInfo(collection)
                {
                    PrimarySequence = question.BankIndex + 1,
                    SecondarySequence = qSequence++,
                    BankQuestion = question,
                    AttemptQuestion = q
                });
            }
        }

        public static void InitProperties(IEnumerable<IQuestionInfo> questions, Guid organizationId)
        {
            var taxonomies = GetTaxonomies(organizationId);

            var filter = questions
                .Where(x => x.BankQuestion.Standard != Guid.Empty)
                .Select(x => x.BankQuestion.Standard)
                .Distinct()
                .ToArray();
            var competencies = StandardSearch
                .Bind(x => new SnippetBuilder.StandardModel
                {
                    Identifier = x.StandardIdentifier,
                    Label = x.StandardLabel,
                    Type = x.StandardType,
                    Name = x.ContentName,
                    Number = x.AssetNumber,
                    Title = x.ContentTitle,
                    Code = x.Code,
                    ParentCode = x.Parent.Code
                }, x => filter.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.Identifier, x => x);

            foreach (var q in questions)
            {
                var info = (QuestionInfo)q;
                if (q.BankQuestion.Standard != Guid.Empty && competencies.TryGetValue(q.BankQuestion.Standard, out var competency))
                    info.CompetencyName = SnippetBuilder.GetHtml(competency);

                if (q.BankQuestion.Classification.Taxonomy.HasValue && taxonomies.TryGetValue(q.BankQuestion.Classification.Taxonomy.Value, out var taxonomy))
                    info.TaxonomyName = $"{taxonomy.ItemSequence}. {taxonomy.ItemName}";
            }
        }

        private static Dictionary<int, TCollectionItem> GetTaxonomies(Guid organizationId)
        {
            var items = TCollectionItemCache
                .Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = organizationId,
                    CollectionName = CollectionName.Assessments_Questions_Classification_Taxonomy
                });

            if (items.IsEmpty())
                items = TCollectionItemCache
                    .Select(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = OrganizationIdentifiers.Global,
                        CollectionName = CollectionName.Assessments_Questions_Classification_Taxonomy
                    });


            return items.ToDictionary(x => x.ItemSequence, x => x);
        }

        public static IEnumerable<Tuple<string, string>> EnumerateLabels(Question q)
        {
            yield return new Tuple<string, string>("primary", $"Bank Q{q.BankIndex + 1}");

            if (q.Classification.Code.IsNotEmpty())
                yield return new Tuple<string, string>("info", q.Classification.Code);

            if (q.Classification.LikeItemGroup.IsNotEmpty())
                yield return new Tuple<string, string>("warning", q.Classification.LikeItemGroup);

            if (q.Classification.Tag.IsNotEmpty())
                yield return new Tuple<string, string>("default", q.Classification.Tag);
        }

        public static IEnumerable<Tuple<string, string>> EnumerateProperties(IQuestionInfo info)
        {
            var q = info.BankQuestion;

            yield return new Tuple<string, string>("Asset #", $"{q.Asset}.{q.AssetVersion}");

            if (info.CompetencyName != null)
                yield return new Tuple<string, string>("Competency", info.CompetencyName);

            if (info.TaxonomyName != null)
                yield return new Tuple<string, string>("Taxonomy", info.TaxonomyName);

            if (q.Classification.LikeItemGroup != null)
                yield return new Tuple<string, string>("LIG", WebUtility.HtmlDecode(q.Classification.LikeItemGroup));

            if (q.Classification.Code != null)
                yield return new Tuple<string, string>("Code", WebUtility.HtmlDecode(q.Classification.Code));

            if (q.Condition != null)
                yield return new Tuple<string, string>("Status", WebUtility.HtmlDecode(q.Condition));

            if (q.Flag != FlagType.None)
                yield return new Tuple<string, string>("Flag", q.Flag.ToIconHtml() + $"<span class='ms-1 form-text'>{q.Flag.GetName()}</span>");

            if (q.Classification.Reference != null)
                yield return new Tuple<string, string>("Reference", WebUtility.HtmlDecode(q.Classification.Reference));
        }
    }
}