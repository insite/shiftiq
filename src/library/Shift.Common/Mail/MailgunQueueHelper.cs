using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Constant;

namespace Shift.Common
{
    public class MailgunQueueHelper
    {
        public const string VariableName = "queue-name";

        private const string _fullNamePrefix = "mailgun-webhooks-";

        private readonly HashSet<string> _allowedShortNames;

        public MailgunQueueHelper(AppSettings settings)
        {
            var validPartitions = new HashSet<string>(
                settings.Partitions.Select(x => GetPartitionName(x.Number)), StringComparer.Ordinal);
            var validEnvironments = new HashSet<EnvironmentName>
            {
                EnvironmentName.Local,
                EnvironmentName.Development,
                EnvironmentName.Sandbox,
                EnvironmentName.Production
            };

            _allowedShortNames = GetAllowedShortNames(settings, validPartitions, validEnvironments);
        }

        #region Initialization

        private static HashSet<string> GetAllowedShortNames(AppSettings settings, HashSet<string> validPartitions, HashSet<EnvironmentName> validEnvironments)
        {
            var rabbitMq = settings.Integration.RabbitMq;
            var environments = ParseEnvironments(rabbitMq.Environments, validEnvironments);
            var partitions = ParsePartitions(rabbitMq.Partitions, validPartitions);
            var names = new HashSet<string>(StringComparer.Ordinal);

            foreach (var environment in environments)
                foreach (var partition in partitions)
                    names.Add(GetQueueShortName(environment, partition));

            foreach (var value in rabbitMq.IncludeQueue.EmptyIfNull())
            {
                ValidateShortName(value, nameof(RabbitMq.IncludeQueue), validEnvironments, validPartitions);

                names.Add(value);
            }

            foreach (var value in rabbitMq.ExcludeQueue.EmptyIfNull())
            {
                ValidateShortName(value, nameof(RabbitMq.ExcludeQueue), validEnvironments, validPartitions);

                names.Remove(value);
            }

            return names;
        }

        private static string[] ParseEnvironments(string[] values, HashSet<EnvironmentName> validEnvironments)
        {
            var result = new List<EnvironmentName>();

            foreach (var value in values.EmptyIfNull())
            {
                if (value.IsEmpty())
                    continue;

                var name = value.ToEnumNullable<EnvironmentName>();
                if (!name.HasValue || !validEnvironments.Contains(name.Value))
                    throw ApplicationError.Create("Unknown environment name: {0}", value);

                if (!result.Contains(name.Value))
                    result.Add(name.Value);
            }

            if (result.Count == 0)
                result.Add(EnvironmentName.Local);

            return result.Select(x => GetEnvironmentName(x)).ToArray();
        }

        private static string[] ParsePartitions(int[] values, HashSet<string> validPartitions)
        {
            var result = new List<string>();

            foreach (var value in values.EmptyIfNull())
            {
                var name = GetPartitionName(value);
                if (!validPartitions.Contains(name))
                    throw ApplicationError.Create("Unknown partition: {0}", value);

                if (!result.Contains(name))
                    result.Add(name);
            }

            if (result.Count == 0)
                result.AddRange(validPartitions);

            return result.ToArray();
        }

        private static void ValidateShortName(
            string value,
            string setting,
            HashSet<EnvironmentName> validEnvironments,
            HashSet<string> validPartitions)
        {
            if (value.IsEmpty())
                throw ApplicationError.Create("Empty queue name in {0}: {1}", setting, value);

            var parts = value.Split('-');
            if (parts.Length != 2)
                throw ApplicationError.Create("Unexpected queue name in {0}: {1}", setting, value);

            var environment = GetEnvironmentName(parts[0]);
            if (!validEnvironments.Contains(environment))
                throw ApplicationError.Create("Unknown environment name in {0}: {1}", setting, value);

            var partition = parts[1];
            if (!validPartitions.Contains(partition))
                throw ApplicationError.Create("Unknown partition in {0}: {1}", setting, value);
        }

        #endregion

        public bool TryGetQueueFullName(string shortName, out string queueName)
        {
            var isValid = shortName.IsNotEmpty() && _allowedShortNames.Contains(shortName);

            queueName = isValid ? _fullNamePrefix + shortName : null;

            return isValid;
        }

        public IEnumerable<string> EnumerateAllQueues() =>
            _allowedShortNames.OrderBy(x => x, StringComparer.Ordinal).Select(x => _fullNamePrefix + x);

        public static string GetQueueFullName(AppSettings settings) =>
            _fullNamePrefix + GetQueueShortName(settings);

        public static string GetQueueShortName(AppSettings settings) =>
            GetQueueShortName(settings.Environment.Name, settings.Partition.Number);

        private static string GetQueueShortName(EnvironmentName environment, int partition) =>
            GetQueueShortName(GetEnvironmentName(environment), GetPartitionName(partition));

        private static string GetQueueShortName(string environment, string partition) => $"{environment}-{partition}";

        private static class EnvNameMapping
        {
            public const string Production = "prod";
            public const string Sandbox = "sandbox";
            public const string Development = "dev";
            public const string Local = "local";
        };

        private static string GetEnvironmentName(EnvironmentName value)
        {
            if (value == EnvironmentName.Production)
                return EnvNameMapping.Production;

            if (value == EnvironmentName.Sandbox)
                return EnvNameMapping.Sandbox;

            if (value == EnvironmentName.Development)
                return EnvNameMapping.Development;

            if (value == EnvironmentName.Local)
                return EnvNameMapping.Local;

            throw ApplicationError.Create("Unknown environment name: {0}", value.GetName());
        }

        private static EnvironmentName GetEnvironmentName(string value)
        {
            if (value == EnvNameMapping.Production)
                return EnvironmentName.Production;

            if (value == EnvNameMapping.Sandbox)
                return EnvironmentName.Sandbox;

            if (value == EnvNameMapping.Development)
                return EnvironmentName.Development;

            if (value == EnvNameMapping.Local)
                return EnvironmentName.Local;

            throw ApplicationError.Create("Unknown environment name: {0}", value);
        }

        private static string GetPartitionName(int number)
        {
            if (number < 0 || number > 99)
                throw ApplicationError.Create("Unexpected partition number: {0}", number);

            return $"e{number:00}";
        }
    }
}
