using InSite.Domain.Banks;

using Shift.Constant;
using Shift.Contract;
using Shift.Common;
using Shift.Service.Competency;
using Shift.Service.Utility;
using Shift.Service.Evaluation.Workshops.Creators;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class WorkshopQuestionCreator(
    BankState bank,
    StandardReader standardReader,
    CollectionItemReader collectionItemReader
)
{
    private static readonly WorkshopQuestionData EmptyData = new()
    {
        TotalQuestionCount = 0,
        Sections = [],
        Taxonomies = [],
        FirstSectionId = Guid.Empty,
        FirstSectionAreaId = null,
        FirstSectionStandards = [],
        FirstSectionQuestions = []
    };

    private Dictionary<Guid, WorkshopComment[]> _commentsPerQuestion = default!;

    public async Task<WorkshopQuestionData> CreateInitDataAsync(Form form, Guid? sectionId, Guid? questionId, WorkshopCommentCreator commentCreator)
    {
        if (form.Sections.Count == 0)
            return EmptyData;
        
        var section = sectionId.HasValue
            ? form.Sections.Find(x => x.Identifier == sectionId) ?? form.Sections[0]
            : questionId.HasValue
                ? form.Sections.Find(x => x.Fields.Any(y => y.QuestionIdentifier == questionId)) ?? form.Sections[0]
                : form.Sections[0];

        var questions = section.Fields.Select(x => x.Question).ToArray();
        await CreateCommentsAsync(questions, commentCreator);

        return new WorkshopQuestionData
        {
            TotalQuestionCount = form.GetQuestions().Count,
            Sections = form.Sections
                .Select(x => new WorkshopItem
                {
                    Value = x.Identifier.ToString().ToLower(),
                    Text = x.Criterion.Title,
                })
                .ToArray(),
            Taxonomies = await CreateTaxonomies(),
            FirstSectionId = section.Identifier,
            FirstSectionAreaId = null,
            FirstSectionStandards = await CreateStandardsAsync(section.Criterion.Sets),
            FirstSectionQuestions = CreateSectionQuestions(section)
        };
    }

    public async Task<WorkshopQuestionData> CreateInitDataAsync(Specification spec, Guid? setId, Guid? questionId, WorkshopCommentCreator commentCreator)
    {
        var sets = spec.Criteria.SelectMany(x => x.Sets).Distinct().OrderBy(x => x.Sequence).ToList();
        if (sets.Count == 0)
            return EmptyData;
        
        var set = setId.HasValue
            ? sets.Find(x => x.Identifier == setId) ?? sets[0]
            : questionId.HasValue
                ? sets.Find(x => x.Questions.Any(y => y.Identifier == questionId)) ?? sets[0]
                : sets[0];

        var questions = set.Questions.ToArray();
        await CreateCommentsAsync(questions, commentCreator);

        return new WorkshopQuestionData
        {
            TotalQuestionCount = sets.SelectMany(s => s.Questions).Count(),
            Sections = sets
                .Select(x => new WorkshopItem
                {
                    Value = x.Identifier.ToString().ToLower(),
                    Text = x.Name,
                })
                .ToArray(),
            Taxonomies = await CreateTaxonomies(),
            FirstSectionId = set.Identifier,
            FirstSectionAreaId = set.Standard != Guid.Empty ? set.Standard : null,
            FirstSectionStandards = null,
            FirstSectionQuestions = CreateSetQuestions(set)
        };
    }

    public async Task<FormWorkshop.Section> CreateSectionDataAsync(Section section, WorkshopCommentCreator commentCreator)
    {
        var questions = section.Fields.Select(x => x.Question).ToArray();
        await CreateCommentsAsync(questions, commentCreator);

        return new FormWorkshop.Section
        {
            Standards = await CreateStandardsAsync(section.Criterion.Sets),
            Questions = CreateSectionQuestions(section)
        };
    }

    public async Task<SpecWorkshop.Set> CreateSetDataAsync(Set set, WorkshopCommentCreator commentCreator)
    {
        var questions = set.Questions.ToArray();
        await CreateCommentsAsync(questions, commentCreator);

        return new SpecWorkshop.Set
        {
            AreaId = set.Standard != Guid.Empty ? set.Standard : null,
            Questions = CreateSetQuestions(set)
        };
    }

    public static async Task<WorkshopQuestionComments> CreateQuestionCommentsAsync(Question question, WorkshopCommentCreator commentCreator)
    {
        var inputComments = question
            .Comments.Where(y => y.AuthorRole == CommentAuthorType.Administrator)
            .ToArray();
            
        var outputComments = await commentCreator.CreateAsync(inputComments, false);

        var candidateCommentCount = question.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count();

        return new WorkshopQuestionComments
        {
            Comments = outputComments,
            CandidateCommentCount = candidateCommentCount
        };
    }

    private async Task<WorkshopItem[]> CreateTaxonomies()
    {
        var items = await collectionItemReader
            .CollectAsync(bank.Tenant, CollectionName.Assessments_Questions_Classification_Taxonomy);

        return items
            .Select(x => new WorkshopItem
            {
                Value = x.ItemSequence.ToString(),
                Text = $"{x.ItemSequence}. {x.ItemName}"
            })
            .ToArray();
    }

    private WorkshopQuestion[] CreateSectionQuestions(Section section)
    {
        return section.Fields
            .Select(f => CreateQuestion(f.Question, f, _commentsPerQuestion))
            .ToArray();
    }

    private WorkshopQuestion[] CreateSetQuestions(Set set)
    {
        return set
            .Questions.Select(q => CreateQuestion(q, null, _commentsPerQuestion))
            .ToArray();
    }
    public static WorkshopQuestion CreateQuestion(Question q, Field? f, Dictionary<Guid, WorkshopComment[]>? commentsPerQuestion)
    {
        var table = q.Layout.Type == OptionLayoutType.Table
            ? BankQuestionTable.Build(q.Layout.Columns, q.Options.Select(x => x.Content.Title.Default))
            : null;

        return new WorkshopQuestion
        {
            QuestionId = q.Identifier,
            ParentStandardId = q.Set.Standard != Guid.Empty ? q.Set.Standard : null,
            StandardId = q.Standard != Guid.Empty ? q.Standard : null,
            FieldId = f?.Identifier,
            QuestionBankIndex = q.BankIndex,
            QuestionFormSequence = f?.FormSequence,
            QuestionFlag = q.Flag,
            QuestionType = q.Type,
            QuestionTitle = q.Content.Title,
            QuestionTitleHtml = Markdown.ToHtml(q.Content.Title.Default),
            Rationale = q.Content.Rationale?.Default,
            RationaleOnCorrectAnswer = q.Content.RationaleOnCorrectAnswer?.Default,
            RationaleOnIncorrectAnswer = q.Content.RationaleOnIncorrectAnswer?.Default,
            QuestionAssetNumber = q.Asset,
            QuestionAssetVersion = q.AssetVersion,
            QuestionPublicationStatusDescription = q.PublicationStatus.GetDescription(),
            QuestionCondition = q.Condition,
            QuestionTaxonomy = q.Classification.Taxonomy,
            QuestionLikeItemGroup = q.Classification.LikeItemGroup,
            QuestionReference = q.Classification.Reference,
            QuestionCode = q.Classification.Code,
            QuestionTag = q.Classification.Tag,
            QuestionLayoutType = q.Layout.Type,
            QuestionPoints = q.Points != null ? (int)(q.Points.Value * 10000) : null,
            QuestionCutScore = q.CutScore != null ? (int)(q.CutScore.Value * 10000) : null,
            QuestionCalculationMethodDescription = q.CalculationMethod.GetDescription(),
            QuestionClassificationDifficulty = q.Classification.Difficulty,
            QuestionRandomizationEnabled = q.Randomization.Enabled,
            LayoutColumns = CreateLayoutColumns(table),
            CandidateCommentCount = q.Comments.Where(x => x.AuthorRole == CommentAuthorType.Candidate).Count(),
            CanEdit = q.PublicationStatus != PublicationStatus.Published,
            CanNavigateToChangePage = q.FirstPublished == null,
            CanCopyField = f == null,
            ReplaceButtons = CreateReplaceButtons(q, f),
            Source = CreateSource(q),
            Forms = CreateForms(q),
            Comments = commentsPerQuestion != null && commentsPerQuestion.TryGetValue(q.Identifier, out var comments) ? comments : [],
            Options = CreateOptions(q, table),
            Matches = CreateMatches(q),
            Distractors = CreateDistractors(q)
        };
    }

    private static WorkshopQuestion.LayoutColumn[]? CreateLayoutColumns(BankQuestionTable? table)
    {
        if (table == null)
            return null;

        return table.GetHeader()
            .Select(col => new WorkshopQuestion.LayoutColumn
            {
               Alignment = col.Alignment,
               CssClass = col.CssClass,
               TextMarkdown = col.Text,
               TextHtml = Markdown.ToHtml(col.Text) 
            })
            .ToArray();
    }

    private static WorkshopQuestion.QuestionReplaceButtons CreateReplaceButtons(Question q, Field? f)
    {
        var isFormUnpublished = f != null && f.Section.Form.Publication.Status != PublicationStatus.Published;

        return new WorkshopQuestion.QuestionReplaceButtons
        {
            NewVersion = isFormUnpublished && q.PublicationStatus == PublicationStatus.Published && q.IsLastVersion(),
            NewQuestionAndSurplus = isFormUnpublished,
            NewQuestionAndPurge = isFormUnpublished,
            RollbackQuestion = isFormUnpublished && q.AssetVersion > 0 && !q.IsFirstVersion()
        };
    }

    private static WorkshopQuestion.QuestionSource? CreateSource(Question q)
    {
        if (q.Source == null)
            return null;

        var s = q.Set.Bank.FindQuestion(q.Source.Value);
        return s != null
            ? new WorkshopQuestion.QuestionSource
            {
                QuestionId = s.Identifier,
                QuestionAssetNumber = s.Asset
            } : null;
    }

    private static WorkshopQuestion.QuestionForm[]? CreateForms(Question q)
    {
        if (q.Fields.Count == 0)
            return null;

        return q.Fields
            .OrderBy(x => x.Section.Form.Sequence)
            .Select(x => new WorkshopQuestion.QuestionForm
            {
                FormId = x.Section.Form.Identifier,
                FormName = x.Section.Form.Name,
                FormSequence = x.Section.Form.Sequence,
                FormAssetNumber = x.Section.Form.Asset,
                FormAssetversion = x.Section.Form.AssetVersion
            })
            .ToArray();
    }

    private static WorkshopQuestionOption[]? CreateOptions(Question q, BankQuestionTable? table)
    {
        switch (q.Type)
        {
            case QuestionItemType.SingleCorrect:
            case QuestionItemType.TrueOrFalse:
            case QuestionItemType.MultipleCorrect:
            case QuestionItemType.BooleanTable:
                break;
            default:
                return null;
        }

        return q.Options
            .Select((x, index) => new WorkshopQuestionOption
            {
                Number = x.Number,
                Letter = x.Letter,
                TitleMarkdown = x.Content.Title?.Default,
                TitleHtml = Markdown.ToHtml(x.Content.Title?.Default),
                Points = (int)(x.Points * 10000),
                IsTrue = x.IsTrue,
                Columns = CreateOptionColumns(table, index)
            })
            .ToArray();
    }

    private static WorkshopQuestionOption.OptionColumn[]? CreateOptionColumns(BankQuestionTable? table, int rowIndex)
    {
        if (table == null)
            return null;

        var columns = new WorkshopQuestionOption.OptionColumn[table.ColumnsCount];
        for (var i = 0; i < table.ColumnsCount; i++)
        {
            var cell = table.GetBody(rowIndex, i);
            columns[i] = new WorkshopQuestionOption.OptionColumn
            {
                TextMarkdown = cell.Text,
                TextHtml = Markdown.ToHtml(cell.Text)
            };
        }

        return columns;
    }

    private static WorkshopQuestionMatch[]? CreateMatches(Question q)
    {
        if (q.Type != QuestionItemType.Matching)
            return null;

        return q.Matches.Pairs
            .Select(x => new WorkshopQuestionMatch
            {
                Left = x.Left.Title.Default,
                Right = x.Right.Title.Default,
                Points = (int)(x.Points * 10000)
            })
            .ToArray();
    }

    private static string[]? CreateDistractors(Question q)
    {
        if (q.Type != QuestionItemType.Matching)
            return null;

        return q.Matches.Distractors
            .Where(x => !string.IsNullOrEmpty(x.Title.Default))
            .Select(x => x.Title.Default)
            .ToArray();
    }

    private async Task<WorkshopStandard[]> CreateStandardsAsync(IEnumerable<Set> sets)
    {
        var creator = new WorkshopStandardCreator(standardReader);

        var standards = new List<WorkshopStandard>();
        var parentIds = sets.Where(x => x.Standard != Guid.Empty).Select(x => x.Standard).ToArray();

        await creator.CreateAsync(parentIds, standards);

        foreach (var standard in standards)
            standard.ParentId = null;

        await creator.CreateByParentAsync(parentIds, standards);

        return standards.ToArray();
    }

    private async Task CreateCommentsAsync(Question[] questions, WorkshopCommentCreator commentCreator)
    {
        var inputComments = questions
            .SelectMany(x =>
                x.Comments
                    .Where(y => y.AuthorRole == CommentAuthorType.Administrator)
                    .Select(y =>
                    {
                        y.Subject = x.Identifier;
                        return y;
                    })
            )
            .ToArray();
            
        var outputComments = await commentCreator.CreateAsync(inputComments, false);

        _commentsPerQuestion = outputComments
            .GroupBy(x => x.EntityId)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }
}
