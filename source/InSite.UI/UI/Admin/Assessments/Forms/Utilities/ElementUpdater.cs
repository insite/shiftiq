using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Domain.Banks;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Models
{
    public static class ElementUpdater
    {
        public static class ElementTypes
        {
            public const string QuestionTitle = "Question";
            public const string QuestionCode = "Code";
            public const string QuestionFlag = "Flag";
            public const string QuestionLIG = "LIG";
            public const string QuestionReference = "Ref";
            public const string QuestionTag = "Tag";
            public const string QuestionTaxonomy = "Tax";
            public const string QuestionCondition = "Condition";
            public const string QuestionPublicationStatus = "PublicationStatus";
            public const string QuestionStandard = "Standard";
            public const string OptionTitle = "Option";
            public const string OptionTitleColumn = "Column";
            public const string OptionHeaderColumn = "Header";
            public const string OptionPoints = "Points";
        }

        public static object SetElementValue(string elementType, string key, string value)
        {
            var keyParts = key.Split(new[] { ':' });
            var bankId = Guid.Parse(keyParts[0]);
            var questionId = Guid.Parse(keyParts[1]);
            var optionNumber = keyParts.Length > 2 ? int.Parse(keyParts[2]) : (int?)null;

            var bank = ServiceLocator.BankSearch.GetBankState(bankId);
            if (bank == null)
                throw ApplicationError.Create("Bank not found: {0}", bankId);

            var question = bank.FindQuestion(questionId);
            if (question == null)
                throw ApplicationError.Create("Question not found: {0}", questionId);

            Option option = null;
            if (optionNumber.HasValue && optionNumber != 0)
            {
                option = bank.FindOption(questionId, optionNumber.Value);
                if (option == null)
                    throw ApplicationError.Create("Option not found: {0}", optionNumber.Value);
            }

            object result = null;
            var commands = new List<ICommand>();

            switch (elementType)
            {
                case ElementTypes.QuestionTitle:
                    var questionContent = question.Content != null ? question.Content.Clone() : new ContentExamQuestion();

                    if (value.IsEmpty() || !value.StartsWith("+{"))
                        questionContent.Title.Default = value;
                    else
                        questionContent.Title = MultilingualString.Deserialize(value.Substring(1));

                    commands.Add(new ChangeQuestionContent(bankId, questionId, questionContent));

                    result = questionContent.Title.Default;
                    break;
                case ElementTypes.QuestionCode:
                    ValidateMaxLength(value, 40);
                    var classificationCode = question.Classification.Clone();
                    classificationCode.Code = value;
                    commands.Add(new ChangeQuestionClassification(bankId, questionId, classificationCode));
                    break;
                case ElementTypes.QuestionFlag:
                    commands.Add(new ChangeQuestionFlag(bankId, questionId, value.ToEnum<FlagType>()));
                    break;
                case ElementTypes.QuestionLIG:
                    ValidateMaxLength(value, 64);
                    var classificationLIG = question.Classification.Clone();
                    classificationLIG.LikeItemGroup = value;
                    commands.Add(new ChangeQuestionClassification(bankId, questionId, classificationLIG));
                    break;
                case ElementTypes.QuestionReference:
                    ValidateMaxLength(value, 500);
                    var classificationReference = question.Classification.Clone();
                    classificationReference.Reference = value;
                    commands.Add(new ChangeQuestionClassification(bankId, questionId, classificationReference));
                    break;
                case ElementTypes.QuestionTag:
                    ValidateMaxLength(value, 100);
                    var classificationTag = question.Classification.Clone();
                    classificationTag.Tag = value;
                    commands.Add(new ChangeQuestionClassification(bankId, questionId, classificationTag));
                    break;
                case ElementTypes.QuestionTaxonomy:
                    var classificationTaxonomy = question.Classification.Clone();
                    classificationTaxonomy.Taxonomy = int.TryParse(value, out var taxonomy) ? taxonomy : (int?)null;
                    commands.Add(new ChangeQuestionClassification(bankId, questionId, classificationTaxonomy));
                    break;
                case ElementTypes.QuestionPublicationStatus:
                    commands.Add(new ChangeQuestionPublicationStatus(bankId, questionId, value.ToEnum<PublicationStatus>()));
                    break;
                case ElementTypes.QuestionCondition:
                    commands.Add(new ChangeQuestionCondition(bankId, questionId, value));
                    break;
                case ElementTypes.QuestionStandard:
                    var standard = !string.IsNullOrEmpty(value) ? Guid.Parse(value) : Guid.Empty;
                    commands.Add(new ChangeQuestionStandard(bankId, questionId, standard, null));
                    break;
                case ElementTypes.OptionTitle:
                    var optionContent = option.Content != null ? option.Content.Clone() : new ContentTitle();

                    if (optionContent.Title == null)
                        optionContent.Title = new MultilingualString();

                    optionContent.Title.Default = value;

                    commands.Add(new ChangeOption(bankId, questionId, option.Number, optionContent, option.Points, option.IsTrue, option.CutScore, option.Standard));

                    result = value;
                    break;
                case ElementTypes.OptionTitleColumn:
                    var optionTitleTable = CreateOptionTitle(question, option, int.Parse(keyParts[3]), value);
                    var optionContentTable = option.Content.Clone();
                    optionContentTable.Title.Default = optionTitleTable;
                    commands.Add(new ChangeOption(bankId, questionId, option.Number, optionContentTable, option.Points, option.IsTrue, option.CutScore, option.Standard));
                    result = value;
                    break;
                case ElementTypes.OptionHeaderColumn:
                    var columnIndex = int.Parse(keyParts[3]);

                    if (columnIndex > 0 && question.Layout.Columns != null && columnIndex < question.Layout.Columns.Length)
                    {
                        var optionLayout = question.Layout.Clone();
                        var optionHeaderContent = optionLayout.Columns[columnIndex].Content;

                        if (optionHeaderContent.Title == null)
                            optionHeaderContent.Title = new MultilingualString();

                        optionHeaderContent.Title.Default = value;

                        commands.Add(new ChangeQuestionLayout(bankId, questionId, optionLayout));
                    }

                    result = value;

                    break;
                case ElementTypes.OptionPoints:
                    if (decimal.TryParse(value, out var optionPoints))
                    {
                        commands.Add(new ChangeOption(bankId, questionId, option.Number, option.Content.Clone(), optionPoints, option.IsTrue, option.CutScore, option.Standard));
                        result = optionPoints;
                    }
                    else
                    {
                        commands.Clear();
                        result = option.Points;
                    }
                    break;
                default:
                    throw new ArgumentException("Unsupported elementType: " + elementType);
            }

            foreach (var cmd in commands)
                ServiceLocator.SendCommand(cmd);

            return result;
        }

        private static string CreateOptionTitle(Question question, Option option, int columnNumber, string newValue)
        {
            var optionTitleTable = QuestionTable.Build(question.Layout.Columns, new[] { option.Content.Title.Default });
            var cells = optionTitleTable.GetBody()[0];
            var result = new StringBuilder();
            var currentColumnNumber = 0;

            foreach (var cell in cells)
            {
                if (result.Length > 0)
                    result.Append("|");

                if (columnNumber == currentColumnNumber)
                    result.Append(newValue);
                else
                    result.Append(cell.Text);

                currentColumnNumber++;
            }

            return result.ToString();
        }

        private static void ValidateMaxLength(string value, int maxLength)
        {
            if (value != null && value.Length > maxLength)
                throw ApplicationError.Create("The value has exceeded the maximum length of {0}. ", maxLength);
        }
    }
}