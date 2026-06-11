using System.Text;

using InSite.Application.Banks.Write;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Timeline.Commands;

using Shift.Contract;
using Shift.Sdk.Service;

namespace Shift.Service.Evaluation.Workshops;

public class WorkshopModifyOptionService(ICommanderAsync commander)
    : IWorkshopModifyOptionService
{
    public async Task<string?> ModifyAsync(BankState bank, Guid bankId, Guid questionId, int optionNumber, WorkshopQuestionOptionField field, int? columnIndex, string value)
    {
        var question = bank.FindQuestion(questionId);
        if (question == null)
            throw ApplicationError.Create("Question not found: {0}", questionId);

        var option = bank.FindOption(questionId, optionNumber);
        if (option == null)
            throw ApplicationError.Create("Option not found: {0}", optionNumber);

        string? result = null;
        ICommand command;

        switch (field)
        {
            case WorkshopQuestionOptionField.Title:
                (command, result) = ModifyOptionTitle(bankId, option, value);
                break;
            case WorkshopQuestionOptionField.ColumnTitle:
                (command, result) = ModifyColumnTitle(bankId, option, columnIndex, value);
                break;
            case WorkshopQuestionOptionField.Points:
                command = ModifyOptionPoints(bankId, option, value);
                break;               
            default:
                throw new ArgumentException($"Unsupported field: {field}");
        }

        await commander.SendCommandsAsync([command]);

        return result;
    }

    private static (ChangeOption, string?) ModifyOptionTitle(Guid bankId, Option option, string value)
    {
        var content = option.Content != null ? option.Content.Clone() : new ContentTitle();

        if (content.Title == null)
            content.Title = new MultilingualString();

        content.Title.Default = value;

        var command = new ChangeOption(bankId, option.Question.Identifier, option.Number, content, option.Points, option.IsTrue, option.CutScore, option.Standard);
        var titleHtml = Markdown.ToHtml(value);

        return (command, titleHtml);
    }

    private static (ChangeOption, string?) ModifyColumnTitle(Guid bankId, Option option, int? columnIndex, string value)
    {
        if (columnIndex == null)
            throw new ArgumentException($"Invalid columnIndex: {columnIndex}");

        var optionTitleTable = CreateOptionTitle(option.Question, option, columnIndex.Value, value);
        var optionContentTable = option.Content.Clone();
        optionContentTable.Title.Default = optionTitleTable;

        var command = new ChangeOption(bankId, option.Question.Identifier, option.Number, optionContentTable, option.Points, option.IsTrue, option.CutScore, option.Standard);
        var titleHtml = Markdown.ToHtml(value);

        return (command, titleHtml);
    }

    private static string CreateOptionTitle(Question question, Option option, int columnIndex, string newValue)
    {
        var optionTitleTable = BankQuestionTable.Build(question.Layout.Columns, new[] { option.Content.Title.Default });
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

    private static ChangeOption ModifyOptionPoints(Guid bankId, Option option, string value)
    {
        if (!decimal.TryParse(value, out var points))
            throw new ArgumentException($"Invalid points value: {value}");

        points /= 10000m;

        return new ChangeOption(bankId, option.Question.Identifier, option.Number, option.Content.Clone(), points, option.IsTrue, option.CutScore, option.Standard);
    }

    private static string? ValidateMaxLength(string value, int maxLength)
    {
        if (value != null && value.Length > maxLength)
            throw ApplicationError.Create("The value has exceeded the maximum length of {0}. ", maxLength);

        return value;
    }
}
