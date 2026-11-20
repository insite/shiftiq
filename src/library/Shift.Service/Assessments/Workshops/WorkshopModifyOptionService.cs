using System.Text;

using InSite.Application.Banks.Write;
using InSite.Domain.Banks;

using Shift.Common;

using Shift.Contract;
using Shift.Sdk.Service;

namespace Shift.Service.Evaluation.Workshops;

public class WorkshopModifyOptionService(ITimelineQuery timelineQuery, ICommanderAsync commander)
    : IWorkshopModifyOptionService
{
    public async Task<WorkshopModifyOptionResult> ModifyAsync(BankState bank, Guid bankId, Guid questionId, int optionNumber, WorkshopQuestionOptionField field, int? columnIndex, string value)
    {
        var question = bank.FindQuestion(questionId);
        if (question == null)
            throw ApplicationError.Create("Question not found: {0}", questionId);

        var option = bank.FindOption(questionId, optionNumber);
        if (option == null)
            throw ApplicationError.Create("Option not found: {0}", optionNumber);

        string? html = null;
        int? questionPoints = null;

        switch (field)
        {
            case WorkshopQuestionOptionField.Title:
                html = await ModifyOptionTitleAsync(bankId, option, value);
                break;
            case WorkshopQuestionOptionField.ColumnTitle:
                html = await ModifyColumnTitleAsync(bankId, option, columnIndex, value);
                break;
            case WorkshopQuestionOptionField.Points:
                questionPoints = await ModifyOptionPointsAsync(bankId, option, value);
                break;               
            default:
                throw new ArgumentException($"Unsupported field: {field}");
        }

        return new WorkshopModifyOptionResult
        {
            Html = html,
            QuestionPoints = questionPoints
        };
    }

    private async Task<string?> ModifyOptionTitleAsync(Guid bankId, Option option, string value)
    {
        var content = option.Content != null ? option.Content.Clone() : new ContentTitle();

        content.Title ??= new MultilingualString();
        content.Title.Default = value;

        var command = new ChangeOption(bankId, option.Question.Identifier, option.Number, content, option.Points, option.IsTrue, option.CutScore, option.Standard);
        var titleHtml = Markdown.ToHtml(value);

        await commander.SendCommandsAsync([command]);

        return titleHtml;
    }

    private async Task<string?> ModifyColumnTitleAsync(Guid bankId, Option option, int? columnIndex, string value)
    {
        if (columnIndex == null)
            throw new ArgumentException($"Invalid columnIndex: {columnIndex}");

        var optionTitleTable = CreateOptionTitle(option.Question, option, columnIndex.Value, value);
        var optionContentTable = option.Content.Clone();
        optionContentTable.Title.Default = optionTitleTable;

        var command = new ChangeOption(bankId, option.Question.Identifier, option.Number, optionContentTable, option.Points, option.IsTrue, option.CutScore, option.Standard);
        var titleHtml = Markdown.ToHtml(value);

        await commander.SendCommandsAsync([command]);

        return titleHtml;
    }

    private static string CreateOptionTitle(Question question, Option option, int columnIndex, string newValue)
    {
        var optionTitleTable = BankQuestionTable.Build(question.Layout.Columns, [option.Content.Title.Default]);
        var cells = optionTitleTable.GetBody()[0];
        var result = new StringBuilder();
        var currentColumnIndex = 0;

        foreach (var cell in cells)
        {
            if (result.Length > 0)
                result.Append('|');

            if (columnIndex == currentColumnIndex)
                result.Append(newValue);
            else
                result.Append(cell.Text);

            currentColumnIndex++;
        }

        return result.ToString();
    }

    private async Task<int> ModifyOptionPointsAsync(Guid bankId, Option option, string value)
    {
        if (!decimal.TryParse(value, out var points))
            throw new ArgumentException($"Invalid points value: {value}");

        points /= 10000m;

        var command = new ChangeOption(bankId, option.Question.Identifier, option.Number, option.Content.Clone(), points, option.IsTrue, option.CutScore, option.Standard);

        await commander.SendCommandsAsync([command]);

        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        var question = bank.FindQuestion(option.Question.Identifier)
            ?? throw new ArgumentOutOfRangeException($"Question is not found: {option.Question.Identifier}");

        return question.Points.HasValue ? (int)(question.Points.Value * 10000m) : 0;
    }

    private static string? ValidateMaxLength(string value, int maxLength)
    {
        if (value != null && value.Length > maxLength)
            throw ApplicationError.Create("The value has exceeded the maximum length of {0}. ", maxLength);

        return value;
    }
}
