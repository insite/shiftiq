using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Service.Competency;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class FormWorkshopDetailsCreator(StandardReader standardReader)
{
    public async Task<FormWorkshop.FormDetails> CreateAsync(BankState bank, Form form, string? publicationStatus)
    {
        var standard = await ReadStandardAsync(bank.Standard);
        var (isQuestionOrderMatch, verifiedQuestions) = GetVerifiedQuestions(bank, form);

        return new FormWorkshop.FormDetails
        {
            SpecificationName = form.Specification.Name,
            SpecificationType = form.Specification.Type,
            Standard = standard,
            FormName  = form.Name,
            FormAssetNumber  = form.Asset,
            FormAssetVersion = form.AssetVersion,
            FormCode = form.Code,
            FormSource = form.Source,
            FormOrigin = form.Origin,
            FormHook = form.Hook,
            PublicationStatus = publicationStatus,
            ThirdPartyAssessmentIsEnabled = form.ThirdPartyAssessmentIsEnabled,
            StaticQuestionOrderVerified = verifiedQuestions != null ? form.StaticQuestionOrderVerified : null,
            VerifiedQuestions  = verifiedQuestions,
            IsQuestionOrderMatch = isQuestionOrderMatch
        };
    }

    private async Task<WorkshopStandard?> ReadStandardAsync(Guid standardId)
    {
        var entity = await standardReader.RetrieveAsync(standardId);
        if (entity == null)
            return null;

        var parentEntity = entity.ParentStandardIdentifier != null
            ? await standardReader.RetrieveAsync(entity.ParentStandardIdentifier.Value)
            : null;

        var standard = ToStandardModel(entity);
        standard.Parent = parentEntity != null ? ToStandardModel(parentEntity) : null;

        return standard;
    }

    private static WorkshopStandard ToStandardModel(StandardEntity entity)
    {
        return new WorkshopStandard
        {
            StandardId = entity.StandardIdentifier,
            AssetNumber = entity.AssetNumber,
            Sequence = entity.Sequence,
            Code = entity.Code,
            Label = entity.StandardLabel ?? entity.StandardType,
            Title = entity.ContentTitle
        };
    }

    public static (bool, FormWorkshop.StaticQuestionOrder[]?) GetVerifiedQuestions(BankState bank, Form form)
    {
        if (form.Specification.Type != SpecificationType.Static || form.StaticQuestionOrder == null)
            return (false, null);

        var currentQuestions = form.GetStaticFormQuestionIdentifiersInOrder();
        var isQuestionOrderMatch = form.StaticQuestionOrder.SequenceEqual(currentQuestions);

        var list = new List<FormWorkshop.StaticQuestionOrder>();
        var sequence = 0;

        foreach (var questionId in form.StaticQuestionOrder)
        {
            var question = bank.FindQuestion(questionId);
            if (question == null)
                continue;

            var text = StringHelper.StripMarkdown(question.Content.Title.Default);
            text = StringHelper.StripHtml(text);
            text = StringHelper.BreakHtml(text);
            text = text.MaxLength(95);

            list.Add(new FormWorkshop.StaticQuestionOrder
            {
                Sequence = ++sequence,
                Code = question.Classification.Code,
                Tag = question.Classification.Tag,
                Text = text
            });
        }

        return (isQuestionOrderMatch, list.ToArray());
    }
}
