using InSite.Application.Banks.Write;
using InSite.Application.Files.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Contract;

using Shift.Sdk.Service;
using Shift.Service.Competency;
using Shift.Service.Content;
using Shift.Service.Metadata.Sequences.Data;

namespace Shift.Service.Evaluation.Workshops;

public class WorkshopModifyQuestionService(
    ICommanderAsync commander,
    IStorageServiceAsync storageService,
    StandardReader standardReader,
    FileReader fileReader,
    ISequence sequence
) : IWorkshopModifyQuestionService
{
    public async Task<Guid> AddQuestionAsync(Guid organizationId, Guid bankId, Guid setId, Guid? standardId, WorkshopNewQuestionCommand command)
    {
        var question = new Question
        {
            Identifier = UniqueIdentifier.Create(),
            Asset = await sequence.IncrementAsync(organizationId, SequenceType.Asset),
            Condition = "Unassigned",
            Standard = standardId.HasValue ? standardId.Value : Guid.Empty,
            SubStandards = null,
        };

        switch (command)
        {
            case WorkshopNewQuestionCommand.QuickMultipleChoice:
                question.Type = QuestionItemType.SingleCorrect;
                question.CalculationMethod = QuestionCalculationMethod.Default;
                question.Options.Add(new Option());
                question.Options.Add(new Option());
                question.Options.Add(new Option());
                question.Options.Add(new Option());
                break;
            case WorkshopNewQuestionCommand.QuickComposedEssay:
                question.Type = QuestionItemType.ComposedEssay;
                question.CalculationMethod = QuestionCalculationMethod.Default;
                break;
            case WorkshopNewQuestionCommand.QuickComposedVoice:
                question.Type = QuestionItemType.ComposedVoice;
                question.CalculationMethod = QuestionCalculationMethod.Default;
                break;
            case WorkshopNewQuestionCommand.QuickMultipleCorrect:
                question.Type = QuestionItemType.MultipleCorrect;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                break;
            case WorkshopNewQuestionCommand.QuickBooleanTable:
                question.Type = QuestionItemType.BooleanTable;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                break;
            case WorkshopNewQuestionCommand.QuickMatching:
                question.Type = QuestionItemType.Matching;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;
                break;
            default:
                throw new NotImplementedException("Unexpected command: " + command);
        }

        await commander.SendCommandsAsync([new AddQuestion(bankId, setId, question)]);

        return question.Identifier;
    }

    public async Task ReplaceQuestionAsync(BankState bank, Guid bankId, Guid fieldId, WorkshopReplaceCommand command)
    {
        var field = bank.FindField(fieldId);
        if (field == null)
            throw ApplicationError.Create("Field not found: {0}", fieldId);

        var question = field.Question;

        var (newQuestionId, newCondition, isNewQuestion, replaceCommand) = await CreateReplaceCommandAsync(bankId, bank.Tenant, question, command);
        if (newQuestionId == Guid.Empty)
            return;

        var section = field.Section;
        var newFieldId = UniqueIdentifier.Create();
        var fieldIndex = section.Fields.IndexOf(field);
        var commands = new List<ICommand>();

        if (replaceCommand != null)
            commands.Add(replaceCommand);

        commands.Add(new DeleteField(bankId, field.Identifier, section.Form.Identifier, question.Identifier));
        commands.Add(new AddField(bankId, newFieldId, section.Identifier, newQuestionId, fieldIndex));

        if (newCondition != null)
            commands.Add(new ChangeQuestionCondition(bankId, question.Identifier, newCondition));

        if (isNewQuestion)
        {
            commands.Add(new ChangeQuestionFlag(bankId, newQuestionId, FlagType.None));

            var classification = question.Classification.Clone();
            classification.LikeItemGroup = null;
            classification.Reference = null;
            classification.Code = null;
            classification.Tag = null;
            commands.Add(new ChangeQuestionClassification(bankId, newQuestionId, classification));
        }

        await commander.SendCommandsAsync(commands);
    }

    private async Task<(Guid, string?, bool, ICommand?)> CreateReplaceCommandAsync(
        Guid bankId,
        Guid organizationId,
        Question question,
        WorkshopReplaceCommand command
        )
    {
        var newQuestionId = UniqueIdentifier.Create();

        switch (command)
        {
            case WorkshopReplaceCommand.NewVersion:
                return question.PublicationStatus != PublicationStatus.Published || !question.IsLastVersion()
                    ? (Guid.Empty, null, false, null)
                    : (newQuestionId, null, false, new UpgradeQuestion(bankId, question.Identifier, newQuestionId));

            case WorkshopReplaceCommand.NewQuestionAndSurplus:
                return (newQuestionId, "Surplus", true, new DuplicateQuestion(bankId, question.Identifier, newQuestionId, await sequence.IncrementAsync(organizationId, SequenceType.Asset)));

            case WorkshopReplaceCommand.NewQuestionAndPurge:
                return (newQuestionId, "Purge", true, new DuplicateQuestion(bankId, question.Identifier, newQuestionId, await sequence.IncrementAsync(organizationId, SequenceType.Asset)));

            case WorkshopReplaceCommand.RollbackQuestion:
                return question.AssetVersion <= 0 || question.IsFirstVersion()
                    ? (Guid.Empty, null, false, null)
                    : (question.PreviousVersion.Identifier, "Surplus", false, null);

            default:
                throw ApplicationError.Create("Command is not supported: {0}", command);
        }
    }

    public async Task<string?> ModifyAsync(BankState bank, Guid bankId, Guid questionId, WorkshopQuestionField field, int? columnIndex, string value)
    {
        var question = bank.FindQuestion(questionId);
        if (question == null)
            throw ApplicationError.Create("Question not found: {0}", questionId);

        string? result = null;
        List<ICommand> commands = new();

        switch (field)
        {
            case WorkshopQuestionField.Title:
                result = await ModifyQuestionTitle(bankId, bank.Tenant, question, value, commands);
                break;
            case WorkshopQuestionField.Flag:
                commands.Add(new ChangeQuestionFlag(bankId, questionId, value.ToEnum<FlagType>()));
                break;
            case WorkshopQuestionField.Condition:
                commands.Add(new ChangeQuestionCondition(bankId, questionId, value));
                break;
            case WorkshopQuestionField.Standard:
                await ModifyQuestionStandardAsync(bankId, question, value, commands);
                break;
            case WorkshopQuestionField.Code:
            case WorkshopQuestionField.LIG:
            case WorkshopQuestionField.Reference:
            case WorkshopQuestionField.Tag:
            case WorkshopQuestionField.Taxonomy:
                ModifyQuestionClassification(bankId, question, field, value, commands);
                break;
            case WorkshopQuestionField.ColumnHeader:
                result = ModifyColumnHeader(bankId, question, columnIndex, value, commands);
                break;
            default:
                throw new ArgumentException($"Unsupported field: {field}");
        }

        await commander.SendCommandsAsync(commands);

        return result;
    }

    private async Task<string?> ModifyQuestionTitle(Guid bankId, Guid organizationId, Question question, string value, List<ICommand> commands)
    {
        var content = question.Content != null ? question.Content.Clone() : new ContentExamQuestion();
        content.Title = MultilingualString.Deserialize(value);

        await new QuestionFileProcessor(storageService, fileReader, sequence).SaveFiles(bankId, organizationId, content.Title, commands);

        commands.Add(new ChangeQuestionContent(bankId, question.Identifier, content));

        var titleHtml = Markdown.ToHtml(content.Title.Default);

        return titleHtml;
    }

    private async Task ModifyQuestionStandardAsync(Guid bankId, Question question, string value, IList<ICommand> commands)
    {
        Guid standardId;

        if (string.IsNullOrEmpty(value))
            standardId = Guid.Empty;
        else if (question.Set.Standard == Guid.Empty)
            throw ApplicationError.Create("Cannot set this competency to the question because Set's area is not defined");
        else
        {
            standardId = Guid.Parse(value);

            var standard = await standardReader.RetrieveAsync(standardId);
            if (standard == null || standard.ParentStandardIdentifier != question.Set.Standard)
                throw ApplicationError.Create("Cannot set this competency to the question because it either does not exist or does not belong Set's area");
        }

        commands.Add(new ChangeQuestionStandard(bankId, question.Identifier, standardId, null));
    }

    private static void ModifyQuestionClassification(Guid bankId, Question question, WorkshopQuestionField field, string value, List<ICommand> commands)
    {
        var classification = question.Classification.Clone();

        switch (field)
        {
            case WorkshopQuestionField.Code:
                classification.Code = ValidateMaxLength(value, 40);
                break;
            case WorkshopQuestionField.LIG:
                classification.LikeItemGroup = ValidateMaxLength(value, 64);
                break;
            case WorkshopQuestionField.Reference:
                classification.Reference = ValidateMaxLength(value, 500);
                break;
            case WorkshopQuestionField.Tag:
                classification.Tag = ValidateMaxLength(value, 100);
                break;
            case WorkshopQuestionField.Taxonomy:
                classification.Taxonomy = int.TryParse(value, out var taxonomy) ? taxonomy : (int?)null;
                break;
            default:
                throw new ArgumentException($"Unsupported field: {field}");
        }

        commands.Add(new ChangeQuestionClassification(bankId, question.Identifier, classification));
    }

    private static string? ModifyColumnHeader(Guid bankId, Question question, int? columnIndex, string value, List<ICommand> commands)
    {
        if (columnIndex == null || columnIndex < 0 || question.Layout.Columns == null || columnIndex >= question.Layout.Columns.Length)
            throw new ArgumentException($"Invalid columnIndex: {columnIndex}");

        var layout = question.Layout.Clone();
        var content = layout.Columns[columnIndex.Value].Content;

        if (content.Title == null)
            content.Title = new MultilingualString();

        content.Title.Default = value;

        commands.Add(new ChangeQuestionLayout(bankId, question.Identifier, layout));

        var titleHtml = Markdown.ToHtml(content.Title.Default);

        return titleHtml;
    }

    private static string? ValidateMaxLength(string value, int maxLength)
    {
        if (value != null && value.Length > maxLength)
            throw ApplicationError.Create("The value has exceeded the maximum length of {0}. ", maxLength);

        return value;
    }
}
