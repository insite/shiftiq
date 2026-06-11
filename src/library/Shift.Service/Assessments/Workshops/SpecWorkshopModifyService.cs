using InSite.Application.Banks.Write;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.Service;
using Shift.Service.Competency;
using Shift.Service.Evaluation.Workshops.Creators;

namespace Shift.Service.Evaluation.Workshops;

public class SpecWorkshopModifyService(
    ITimelineQuery timelineQuery,
    ICommanderAsync commander,
    StandardReader standardReader
) : ISpecWorkshopModifyService
{
    private enum ValidationType { HasChanges, NoChanges, Invalid };

    public async Task<bool> ModifyAsync(Guid bankId, Guid specificationId, SpecWorkshop.Input input)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);
        var spec = bank.FindSpecification(specificationId);

        var commands = new List<ICommand>();

        if (input.FormLimit != spec.FormLimit || input.QuestionLimit != spec.QuestionLimit)
            commands.Add(new ReconfigureSpecification(bankId, specificationId, ConsequenceType.Low, input.FormLimit, input.QuestionLimit));

        var standards = await new SpecWorkshopStandardCreator(standardReader).CreateAsync(spec);
        var initData = new SpecWorkshopDetailsCreator().CreateAsync(spec, standards);
        var standardNumbers = standards.ToDictionary(x => x.StandardId, x => x.AssetNumber.ToString());

        foreach (var inputCriterion in input.Criteria)
        {
            var criterion = initData.Criteria.FirstOrDefault(x => x.CriterionId == inputCriterion.CriterionId);
            
            var validateResult = ValidateInput(inputCriterion, criterion);
            if (validateResult == ValidationType.Invalid)
                return false;

            if (validateResult == ValidationType.NoChanges)
                continue;

            ApplyChanges(bankId, inputCriterion, standardNumbers, commands);
        }

        await commander.SendCommandsAsync(commands);

        return true;
    }

    private static void ApplyChanges(Guid bankId, SpecWorkshop.Input.InputCriterion inputCriterion, Dictionary<Guid, string> standardNumbers, List<ICommand> commands)
    {
        var pivotTable = new PivotTable();

        pivotTable.AddRow("Competency", inputCriterion.Competencies.Select(x => standardNumbers[x.StandardId]).ToArray());
        pivotTable.AddColumn("Taxonomy", ["1", "2", "3"]);

        var tax1Key = new MultiKey<string>("1");
        var tax2Key = new MultiKey<string>("2");
        var tax3Key = new MultiKey<string>("3");

        foreach (var inputCompetency in inputCriterion.Competencies)
        {
            var rowKey = new MultiKey<string>(standardNumbers[inputCompetency.StandardId]);

            pivotTable.SetCellValue(rowKey, tax1Key, Number.NullIfOutOfRange(inputCompetency.Tax1Count ?? 0, 0));
            pivotTable.SetCellValue(rowKey, tax2Key, Number.NullIfOutOfRange(inputCompetency.Tax2Count ?? 0, 0));
            pivotTable.SetCellValue(rowKey, tax3Key, Number.NullIfOutOfRange(inputCompetency.Tax3Count ?? 0, 0));
        }

        var weight = (decimal)inputCriterion.Weight / 10000m;

        commands.Add(new ChangeCriterionFilter(bankId, inputCriterion.CriterionId, weight, null, null, pivotTable));        
    }

    private static ValidationType ValidateInput(SpecWorkshop.Input.InputCriterion inputCriterion, SpecWorkshop.SpecDetails.DetailsCriterion? criterion)
    {
        if (criterion == null || criterion.Competencies.Length != inputCriterion.Competencies.Length)
            return ValidationType.Invalid;

        var hasChanges = criterion.Weight != inputCriterion.Weight;

        for (int i = 0; i < inputCriterion.Competencies.Length; i++)
        {
            var inputCompetency = inputCriterion.Competencies[i];
            var competency = criterion.Competencies[i];
            if (competency.StandardId != inputCompetency.StandardId)
                return ValidationType.Invalid;

            if ((inputCompetency.Tax1Count ?? 0) != (competency.Tax1Count ?? 0)
                || (inputCompetency.Tax2Count ?? 0) != (competency.Tax2Count ?? 0)
                || (inputCompetency.Tax3Count ?? 0) != (competency.Tax3Count ?? 0)
            )
            {
                hasChanges = true;
            }
        }

        return hasChanges ? ValidationType.HasChanges : ValidationType.NoChanges;
    }
}