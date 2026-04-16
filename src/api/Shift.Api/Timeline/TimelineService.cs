using System.Collections.Concurrent;
using System.Net;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;
using Shift.Sdk.Service;
using Shift.Toolbox;

namespace Shift.Api;

public class TimelineService(AppSettings appSettings, IPrincipalProvider principalProvider) : ICommanderAsync, ITimelineQuery
{
    private static (string? Token, DateTimeOffset Created) _token = (null, DateTimeOffset.UtcNow);

    private static TimelineClient? _client;
    private static TimelineClient GetClient(AppSettings appSettings)
    {
        if (_client == null)
            _client = _client = new TimelineClient(appSettings.v1ApiBaseUrl, appSettings.Security);

        return _client;
    }

    public async Task<StateType?> GetAggregateStateAsync<AggregateType, StateType>(Guid aggregateId)
        where AggregateType : AggregateRoot
        where StateType : AggregateState
    {
        var token = await GetTokenAsync();
        var client = GetClient(appSettings);

        var result = await client.GetAggregateStateAsync<AggregateType, StateType>(aggregateId, token);
        if (result.Status == HttpStatusCode.NotFound)
            return null;

        if (!result.IsOK())
            throw new InvalidOperationException($"Error while querying aggregate state: {result.Problem?.Title ?? "Unknown"}. {result.Problem?.Detail ?? "Unknown"}");

        return result.Data;
    }

    public async Task SendCommandsAsync(IEnumerable<ICommand> commands)
    {
        var principal = principalProvider.GetPrincipal();
        var token = await GetTokenAsync();
        var client = GetClient(appSettings);

        foreach (var command in commands)
        {
            command.OriginOrganization = principal.OrganizationId;
            command.OriginUser = principal.UserId;
        }

        var result = await client.QueueCommandsAsync(commands, token);
        if (!result.IsOK())
            throw new InvalidOperationException($"Error while executing command: {result.Problem?.Title ?? "Unknown"}. {result.Problem?.Detail ?? "Unknown"}");
    }

    private async Task<string> GetTokenAsync()
    {
        var lifetimeInMinutes = appSettings.Security.Token.Lifetime + 1;

        if (string.IsNullOrEmpty(_token.Token) || _token.Created.AddMinutes(lifetimeInMinutes - 1) <= DateTimeOffset.UtcNow)
        {
            var now = DateTimeOffset.UtcNow;
            var client = GetClient(appSettings);
            var result = await client.GetTokenAsync(appSettings.Security.Sentinels.Root.Secret, null, lifetimeInMinutes);
            if (!result.IsOK())
                throw new InvalidOperationException($"Error while taking token: {result.Problem?.Title ?? "Unknown"}. {result.Problem?.Detail ?? "Unknown"}");

            _token = (result.Data.AccessToken, now);
        }

        return _token.Token;
    }
}