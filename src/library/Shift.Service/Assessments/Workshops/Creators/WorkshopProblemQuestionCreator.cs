using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal static class WorkshopProblemQuestionCreator
{
    public static WorkshopProblemQuestion[] Create(IEnumerable<Question> questions)
    {
        var result = new List<WorkshopProblemQuestion>();

        foreach (var q in questions)
        {
            if (q.Type != QuestionItemType.SingleCorrect)
                continue;

            string description;

            if (q.Options.Count == 0)
                description= "The question contains no options.";
            else if (q.Options.Count == 1)
                description = "The question contains only one option.";
            else
            {
                var correctCount = q.Options.Where(x => x.HasPoints).Count();

                if (correctCount == 0)
                    description = "The question contains no correct option.";
                else if (correctCount > 1)
                    description = "The question contains more than one correct option.";
                else
                    continue;
            }

            result.Add(CreateProblemQuestion(q, description));
        }

        return result.ToArray();
    }

    private static WorkshopProblemQuestion CreateProblemQuestion(Question q, string problemDescription)
    {
        return new WorkshopProblemQuestion
        {
            QuestionId = q.Identifier,
            QuestionBankIndex = q.BankIndex,
            QuestionAssetNumber = q.Asset,
            QuestionAssetVersion = q.AssetVersion,
            QuestionSetName = q.Set.Name,
            QuestionTitle = Markdown.ToHtml(q.Content.Title?.Default),
            CanDelete = q.PublicationStatus == PublicationStatus.Drafted,
            ProblemDescription = problemDescription,
            Options = q.Options.Select(x => new WorkshopProblemQuestion.ProblemOption
            {
                Number = x.Number,
                Points = (int)(x.Points * 10000),
                Letter = x.Letter,
                Title = Markdown.ToHtml(x.Content.Title?.Default),
            }).ToArray()
        };
    }
}
