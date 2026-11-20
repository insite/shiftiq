using InSite.Application.Attempts.Write;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Attempts;
using Shift.Common.Exceptions;
using Shift.Constant;
using Shift.Sdk.Service;
using Shift.Service.Assessment;
using Shift.Service.Evaluation;
using Shift.Service.Security;
using Shift.Service.Workspace;

namespace Shift.Service.Integrations.Proctoring.Proview;

public class ProviewService(
    AppSettings appSettings,
    AttemptReader attemptReader,
    UserReader userReader,
    PageReader pageReader,
    AssessmentReader assessmentReader,
    ITimelineQuery timelineQuery,
    ICommanderAsync commander
)
{
    public async Task ModifyProviewSessionIdAsync(Guid organizationId, Guid userId, string shiftSessionId, string proviewSessionId)
    {
        if (!Guid.TryParse(shiftSessionId, out var attemptId))
            throw new ClientErrorException("Attempt is not found");

        var attemptEntity = await attemptReader.RetrieveAsync(attemptId);
        if (IsAttemptInvalid(organizationId, userId, attemptEntity))
            throw new ClientErrorException("Attempt is not found");
        
        await commander.SendCommandsAsync([new ModifyAIProctoringSessionId(attemptId, proviewSessionId)]);
    }

    public async Task<ProviewSession> RetrieveByResourceAsync(Guid organizationId, Guid userId, string language, string resource, string attempt)
    {
        if (!Guid.TryParse(resource, out var pageId))
            throw new ClientErrorException("Resource parameter is invalid");

        var url = AttemptUrlResource.Load(AttemptActionType.Answer, pageId, attempt);
        if (url?.AttemptID == null)
            throw new ClientErrorException("Attempt parameter is invalid");

        var attemptEntity = await attemptReader.RetrieveAsync(url.AttemptID.Value);
        if (IsAttemptInvalid(organizationId, userId, attemptEntity))
            throw new ClientErrorException("Attempt parameter is invalid");

        var userEntity = await userReader.RetrieveAsync(attemptEntity!.LearnerUserIdentifier);
        if (userEntity == null)
            throw new ClientErrorException("Attempt parameter is invalid");

        var form = await RetrieveFormByPageIdAsync(organizationId, pageId);
        if (form == null || form.Identifier != attemptEntity.FormIdentifier)
            throw new ClientErrorException("Resource parameter is invalid");

        return BuildSession(url, attemptEntity, userEntity, form, language);
    }

    public async Task<ProviewSession> RetrieveByFormAsync(Guid organizationId, Guid userId, string language, Guid formId, string attempt)
    {
        var url = AttemptUrlForm.Load(AttemptActionType.Answer, formId, attempt);
        if (url?.AttemptID == null)
            throw new ClientErrorException("Attempt parameter is invalid");

        var attemptEntity = await attemptReader.RetrieveAsync(url.AttemptID.Value);
        if (IsAttemptInvalid(organizationId, userId, attemptEntity))
            throw new ClientErrorException("Attempt parameter is invalid");

        var userEntity = await userReader.RetrieveAsync(attemptEntity!.LearnerUserIdentifier);
        if (userEntity == null)
            throw new ClientErrorException("Attempt parameter is invalid");

        var form = await RetrieveFormByFormIdAsync(organizationId, formId);
        if (form == null || form.Identifier != attemptEntity.FormIdentifier)
            throw new ClientErrorException("Resource parameter is invalid");

        return BuildSession(url, attemptEntity, userEntity, form, language);
    }

    private static bool IsAttemptInvalid(Guid organizationId, Guid userId, AttemptEntity? attempt)
    {
        return attempt == null
            || attempt.OrganizationIdentifier != organizationId
            || attempt.LearnerUserIdentifier != userId
            || !Enum.TryParse<AIProctoringProvider>(attempt.AIProctoringProvider, true, out var provider)
            || provider != AIProctoringProvider.Proview;
    }

    private ProviewSession BuildSession(AttemptUrlBase url, AttemptEntity attempt, UserEntity user, Form form, string language)
    {
        var isFormOpened = !form.Invigilation.Opened.HasValue || form.Invigilation.Opened.Value <= DateTimeOffset.Now;
        var isFormClosed = form.Invigilation.Closed.HasValue && form.Invigilation.Closed.Value < DateTimeOffset.Now;

        if (!isFormOpened
            || isFormClosed
            || attempt.AttemptSubmitted != null
            || attempt.AttemptStarted == null
        )
        {
            return new ProviewSession { RedirectUrl = url.GetResultUrl() };
        }

        var proview = appSettings.Integration.Proview;

        return new ProviewSession
        {
            Data = new ProviewSession.DataType
            {
                InitScriptUrl = proview.InitUrl,
                ProviewToken = proview.Token,
                SessionTitle = form.Content.Title?[language] ?? form.Content.Title?.Default ?? form.Name,
                ProfileId = user.UserIdentifier.ToString().ToLower(),
                Session = attempt.AttemptIdentifier.ToString().ToLower(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email
            }
        };
    }

    private async Task<Form?> RetrieveFormByPageIdAsync(Guid organizationId, Guid pageId)
    {
        var page = await pageReader.RetrieveAsync(pageId);
        if (page == null
            || page.OrganizationIdentifier != organizationId
            || page.ObjectType != "Assessment"
            || page.ObjectIdentifier == null
        )
        {
            return null;
        }
        
        var form = await assessmentReader.RetrieveAsync(page.ObjectIdentifier.Value);
        if (form == null)
            return null;

        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(form.BankIdentifier);
        if (bank == null)
            return null;

        return bank.FindForm(form.FormIdentifier);
    }

    private async Task<Form?> RetrieveFormByFormIdAsync(Guid organizationId, Guid formId)
    {
        var form = await assessmentReader.RetrieveAsync(formId);
        if (form == null || form.OrganizationIdentifier != organizationId)
            return null;

        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(form.BankIdentifier);
        if (bank == null)
            return null;

        return bank.FindForm(form.FormIdentifier);
    }
}