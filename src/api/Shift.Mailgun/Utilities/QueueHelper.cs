using Shift.Common;

namespace Shift.Mailgun;

internal class QueueHelper
{
    private readonly HashSet<string> ValidPartitions;
    private readonly HashSet<string> ValidEnvironments;

    public QueueHelper(AppSettings settings)
    {
        ValidPartitions = settings.Partitions.Select(x => $"e{x.Number:00}").ToHashSet();
        ValidEnvironments = new HashSet<string>();

        foreach (var env in settings.Integration.RabbitMq.Environments.EmptyIfNull())
        {
            var nameEnum = env.Trim().ToEnumNullable<Common.EnvironmentName>();
            if (nameEnum == null)
                continue;

            if (nameEnum == Common.EnvironmentName.Production)
                ValidEnvironments.Add("prod");
            else if (nameEnum == Common.EnvironmentName.Sandbox)
                ValidEnvironments.Add("sandbox");
            else if (nameEnum == Common.EnvironmentName.Development)
                ValidEnvironments.Add("dev");
            else if (nameEnum == Common.EnvironmentName.Local)
                ValidEnvironments.Add("local");
        }

        if (ValidEnvironments.Count == 0)
            ValidEnvironments.Add("local");
    }

    public bool TryGetQueueName(string domain, out string queueName)
    {
        queueName = null!;

        if (domain.IsEmpty())
            return false;

        var dotIndex = domain.IndexOf('.');
        if (dotIndex <= 0)
            return false;

        var subdomain = domain.Substring(0, dotIndex);

        string env;
        string part;

        var dashIndex = subdomain.LastIndexOf('-');
        if (dashIndex < 0)
        {
            env = "prod";
            part = subdomain;
        }
        else
        {
            env = subdomain.Substring(0, dashIndex);
            part = subdomain.Substring(dashIndex + 1);
        }

        if (!ValidEnvironments.Contains(env))
            return false;

        if (!ValidPartitions.Contains(part))
            return false;

        queueName = GetQueueName(env, part);

        return true;
    }

    public IEnumerable<string> EnumerateAllQueues()
    {
        foreach (var env in ValidEnvironments)
        {
            foreach (var part in ValidPartitions)
            {
                yield return GetQueueName(env, part);
            }
        }
    }

    private static string GetQueueName(string environment, string partition)
        => $"mailgun-webhooks-{environment}-{partition}".ToLowerInvariant();
}
