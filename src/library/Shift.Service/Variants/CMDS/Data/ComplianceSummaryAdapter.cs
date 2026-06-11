using Shift.Common;

namespace Shift.Service.Variant.CMDS;

public class ComplianceSummaryAdapter
{
    public IEnumerable<ComplianceSummaryModel> ToModel(
        ComplianceSummaryCriteria criteria,
        IEnumerable<ComplianceSummaryEntity> entities)
    {
        return entities.Select(x => ToModel(criteria, x));
    }

    public ComplianceSummaryModel ToModel(
        ComplianceSummaryCriteria criteria,
        ComplianceSummaryEntity entity)
    {
        var response = new ComplianceSummaryModel
        {
            Department = new Model
            {
                Identifier = entity.DepartmentIdentifier,
                Name = entity.DepartmentName
            },

            Learner = new Model
            {
                Identifier = entity.UserIdentifier,
                Name = entity.UserFullName
            },

            Measurement = new ComplianceSummaryMeasurement
            {
                Key = entity.Sequence,
                Name = entity.Heading,
                Score = entity.Score,

                Required = entity.Required,
                Satisfied = entity.Satisfied,
                Expired = entity.Expired,
                NotCompleted = entity.NotCompleted,
                NotApplicable = entity.NotApplicable,
                NeedsTraining = entity.NeedsTraining,
                SelfAssessed = entity.SelfAssessed,
                Submitted = entity.Submitted,
                Validated = entity.Validated
            }
        };

        if (entity.PrimaryProfileIdentifier.HasValue)
        {
            response.PrimaryProfile = new Model
            {
                Identifier = entity.PrimaryProfileIdentifier.Value,
                Name = entity.PrimaryProfileTitle,
                Slug = entity.PrimaryProfileNumber
            };
        }

        if (response.Measurement.Required == 0)
        {
            switch (criteria.ZeroRequirementsProgressDisplay)
            {
                case "ShowNoActivity":
                    response.Measurement.Score = 0; // Show 0% (No Activity)
                    break;

                case "ShowRequirementsMet":
                    response.Measurement.Score = 1.0m; // Show 100% (Requirements Met)
                    break;

                case "ShowNotApplicable":
                    response.Measurement.Score = null; // Show N/A (Not Applicable)
                    break;
            }
        }

        return response;
    }
}
