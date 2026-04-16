using InSite.Domain.Banks;

using Shift.Contract;
using Shift.Service.Competency;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class FormWorkshopStatisticsCreator(StandardReader standardReader)
{
    public async Task<FormWorkshop.QuestionStatistics> CreateAsync(BankState bank, IEnumerable<Question> questions, bool includeSubCompetencies)
    {
        var bankStandardCodes = await ReadBankStandardCodes(bank, includeSubCompetencies);

        return FormWorkshop.QuestionStatisticsCreator.Create(questions, bankStandardCodes, includeSubCompetencies);
    }

    private async Task<Dictionary<Guid, string>> ReadBankStandardCodes(BankState bank, bool includeSubCompetencies)
    {
        var bankStandardIds = GetBankStandards(bank, includeSubCompetencies);
        
        var criteria = new SearchStandards { StandardIds = bankStandardIds };
        criteria.Filter.Page = 0;

        var standards = await standardReader.SearchAsync(criteria);
        
        return standards.ToDictionary(x => x.Id, x => x.Code);
    }

    private static Guid[] GetBankStandards(BankState bank, bool includeSubCompetencies)
    {
        var filter = new HashSet<Guid>();

        foreach (var set in bank.Sets)
        {
            filter.Add(set.Standard);

            foreach (var question in set.Questions)
            {
                filter.Add(question.Standard);

                if (includeSubCompetencies && question.SubStandards != null)
                {
                    foreach (var sub in question.SubStandards)
                        filter.Add(sub);
                }

            }
        }

        return filter.ToArray();
    }
}
