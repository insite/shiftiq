using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain;

namespace InSite.Persistence
{
    public class PartitionSearch
    {
        public static IPartitionModel GetPartitionModel(bool isRequired)
        {
            var trace = string.Empty;

            try
            {
                var model = new PartitionModel();

                using (var db = new InternalDbContext())
                {
                    trace += $"ConnectionString = {db.Database.Connection.ConnectionString}";

                    var settings = db.TPartitionSettings.ToList();

                    model.Host = GetValueAsString(settings, "Host:Name");
                    model.Identifier = GetValueAsGuid(settings, "Partition:Identifier");
                    model.Name = GetValueAsString(settings, "Partition:Name");
                    model.Number = GetValueAsInt(settings, "Partition:Number");
                    model.Slug = GetValueAsString(settings, "Partition:Slug");

                    model.Email = GetValueAsString(settings, "Support:Email");

                    model.WhitelistDomains = GetValueAsString(settings, "Whitelist:Domains");
                    model.WhitelistEmails = GetValueAsString(settings, "Whitelist:Emails");

                    model.DatabaseMonitorLargeCommandSize = GetValueAsInt(settings, "Database:LargeCommandSize");
                }

                return model;
            }
            catch (Exception ex)
            {
                if (isRequired)
                {
                    var message = "An unexpected error occurred when loading mandatory partition"
                        + " settings from the database."
                        + $" {trace}";

                    throw new Exception(message, ex);
                }

                // If partition settings are optional then an empty (and non-null) model is permitted.
                return new PartitionModel();
            }
        }

        private static string GetValueAsString(List<TPartitionSettingEntity> settings, string name)
        {
            var setting = settings.FirstOrDefault(x => x.SettingName == name);

            if (setting == null)
                throw new ArgumentException($"Setting name not found: {name}", nameof(name));

            return setting.SettingValue;
        }

        private static int GetValueAsInt(List<TPartitionSettingEntity> settings, string name)
        {
            var value = GetValueAsString(settings, name);

            if (int.TryParse(value, out var valueAsInt))
                return valueAsInt;

            throw new ArgumentException($"Setting value for {name} is not an integer: {value}", nameof(name));
        }

        private static Guid GetValueAsGuid(List<TPartitionSettingEntity> settings, string name)
        {
            var value = GetValueAsString(settings, name);

            if (Guid.TryParse(value, out var valueAsGuid))
                return valueAsGuid;

            throw new ArgumentException($"Setting value for {name} is not a GUID: {value}", nameof(name));
        }
    }
}