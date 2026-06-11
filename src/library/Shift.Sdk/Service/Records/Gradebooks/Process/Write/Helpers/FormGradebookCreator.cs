using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Write
{
    public class FormGradebookCreator
    {
        private class Item
        {
            public Guid ItemId { get; set; }
            public Guid QuestionId { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public decimal? MaxPoints { get; set; }

            public List<Item> ChildItems { get; set; }
        }

        private class Category
        {
            public Guid CategoryId { get; set; }
            public Guid? SectionId { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }

            public List<Item> Items { get; set; }
        }

        private readonly Guid _formId;
        private readonly Guid _bankId;
        private readonly BankState _bank;
        private readonly Dictionary<Guid, decimal> _rubricScores;

        private Guid _gradebookId;
        private List<Command> _gradebookCommands;
        private List<Command> _bankCommands;

        public static (Guid GradebookId, Command[] Commands) Create(Guid formId, BankState bank, IEnumerable<RubricScore> rubricScores)
        {
            var creator = new FormGradebookCreator(formId, bank, rubricScores);
            return creator.CreateCommands();
        }

        private FormGradebookCreator(Guid formId, BankState bank, IEnumerable<RubricScore> rubricScores)
        {
            _formId = formId;
            _bank = bank;
            _bankId = bank.Identifier;
            _rubricScores = rubricScores.ToDictionary(x => x.QuestionId, x => x.MaxPoints);
        }

        private (Guid GradebookId, Command[] Commands) CreateCommands()
        {
            var form = _bank.FindForm(_formId);
            var bank = form.Specification.Bank;

            _gradebookCommands = new List<Command>();
            _bankCommands = new List<Command>();

            _gradebookId = UuidFactory.Create();

            var gradebookTitle = form.Name;
            if (gradebookTitle.Length > 100)
                gradebookTitle = gradebookTitle.Substring(0, 100);

            _gradebookCommands.Add(new CreateGradebook(_gradebookId, bank.Tenant, gradebookTitle, GradebookType.Scores, null, null, null));
            _gradebookCommands.Add(new ReferenceGradebook(_gradebookId, _formId.ToString()));

            _bankCommands.Add(new ChangeFormGradebook(_bankId, _formId, _gradebookId));

            var categories = GetCategories();
            foreach (var category in categories)
                AddCategory(category);

            var commands = _gradebookCommands.Concat(_bankCommands).ToArray();

            return (_gradebookId, commands);
        }

        private void AddCategory(Category category)
        {
            _gradebookCommands.Add(
                new AddGradeItem(
                    _gradebookId,
                    category.CategoryId,
                    category.Code,
                    category.Name,
                    null,
                    true,
                    GradeItemFormat.None,
                    GradeItemType.Category,
                    GradeItemWeighting.None,
                    null,
                    null
                )
            );

            if (category.SectionId.HasValue)
                _gradebookCommands.Add(new ReferenceGradeItem(_gradebookId, category.CategoryId, category.SectionId.ToString()));

            foreach (var item in category.Items)
            {
                if (item.ChildItems != null)
                    AddLikertQuestion(category.CategoryId, item);
                else
                    AddItem(category.CategoryId, null, item);
            }
        }

        private void AddLikertQuestion(Guid categoryId, Item item)
        {
            _gradebookCommands.Add(
                new AddGradeItem(
                    _gradebookId,
                    item.ItemId,
                    item.Code,
                    item.Name,
                    null,
                    true,
                    GradeItemFormat.None,
                    GradeItemType.Category,
                    GradeItemWeighting.None,
                    null,
                    categoryId
                )
            );

            if (item.MaxPoints.HasValue)
                _gradebookCommands.Add(new ChangeGradeItemMaxPoints(_gradebookId, item.ItemId, item.MaxPoints));

            _gradebookCommands.Add(new ReferenceGradeItem(_gradebookId, item.ItemId, item.QuestionId.ToString()));

            _bankCommands.Add(new ChangeQuestionGradeItem2(_bankId, _formId, item.QuestionId, item.ItemId));

            foreach (var child in item.ChildItems)
                AddItem(item.ItemId, item.QuestionId, child);
        }

        private void AddItem(Guid categoryId, Guid? likertQuestionId, Item item)
        {
            _gradebookCommands.Add(
                new AddGradeItem(
                    _gradebookId,
                    item.ItemId,
                    item.Code,
                    item.Name,
                    null,
                    true,
                    GradeItemFormat.Point,
                    GradeItemType.Score,
                    GradeItemWeighting.None,
                    null,
                    categoryId
                )
            );

            if (item.MaxPoints.HasValue)
                _gradebookCommands.Add(new ChangeGradeItemMaxPoints(_gradebookId, item.ItemId, item.MaxPoints));

            _gradebookCommands.Add(new ReferenceGradeItem(_gradebookId, item.ItemId, item.QuestionId.ToString()));

            if (likertQuestionId.HasValue)
                _bankCommands.Add(new ChangeQuestionLikertRowGradeItem(_bankId, _formId, likertQuestionId.Value, item.QuestionId, item.ItemId));
            else
                _bankCommands.Add(new ChangeQuestionGradeItem2(_bankId, _formId, item.QuestionId, item.ItemId));
        }

        private List<Category> GetCategories()
        {
            var form = _bank.FindForm(_formId);

            var categories = form.Specification.Type != SpecificationType.Static
                ? GetFromDynamicQuestions(form)
                : GetFromStaticQuestions(form);

            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                category.Code = (i + 1).ToString();

                for (int k = 0; k < category.Items.Count; k++)
                {
                    var item = category.Items[k];
                    item.Code = $"{i + 1}.{k + 1}";

                    if (item.ChildItems == null)
                        continue;

                    for (int j = 0; j < item.ChildItems.Count; j++)
                        item.ChildItems[j].Code = $"{item.Code}.{j + 1}";
                }
            }

            return categories;
        }

        private List<Category> GetFromDynamicQuestions(Form form)
        {
            var questions = form.GetQuestions();
            var items = GetItems(questions);

            return new List<Category>
            {
                new Category
                {
                    CategoryId = UuidFactory.Create(),
                    Name = "All Questions",
                    Items = items
                }
            };
        }

        private List<Category> GetFromStaticQuestions(Form form)
        {
            var categories = new List<Category>();

            foreach (var section in form.Sections)
            {
                var questions = section.Fields.Select(x => x.Question).ToList();
                var items = GetItems(questions);

                categories.Add(new Category
                {
                    CategoryId = UuidFactory.Create(),
                    SectionId = section.Identifier,
                    Name = GetName(section.Criterion?.Title, "All Questions"),
                    Items = items
                });
            }

            return categories;
        }

        private List<Item> GetItems(List<Question> questions)
        {
            var items = new List<Item>();

            foreach (var question in questions)
            {
                decimal? maxPoints = _rubricScores.TryGetValue(question.Identifier, out var rubricMaxPoints)
                    ? rubricMaxPoints
                    : question.Points;

                items.Add(new Item
                {
                    ItemId = UuidFactory.Create(),
                    QuestionId = question.Identifier,
                    Name = GetName(question.Content.Title.Default, "Question"),
                    MaxPoints = maxPoints,
                    ChildItems = GetItemsFromLikert(question.Likert)
                });
            }
            return items;
        }

        private List<Item> GetItemsFromLikert(LikertMatrix likert)
        {
            if (likert == null || !likert.Rows.Any())
                return null;

            var items = new List<Item>();

            foreach (var likertRow in likert.Rows)
            {
                items.Add(new Item
                {
                    ItemId = UuidFactory.Create(),
                    QuestionId = likertRow.Identifier,
                    Name = GetName(likertRow.Content.Title.Default, "Likert Row"),
                    MaxPoints = likertRow.Points
                });
            }
            return items;
        }

        private static string GetName(string originalName, string defaultName)
        {
            const int maxNameLength = 75;

            string name;

            if (!string.IsNullOrWhiteSpace(originalName))
            {
                name = StringHelper.BreakHtml(StringHelper.StripHtml(originalName));
                if (string.IsNullOrWhiteSpace(name))
                    name = defaultName;
            }
            else
                name = defaultName;

            if (name.Length > maxNameLength)
                name = name.Substring(0, maxNameLength - 3) + "...";

            return name;
        }
    }
}
