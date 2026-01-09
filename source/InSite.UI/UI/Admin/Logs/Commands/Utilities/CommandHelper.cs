using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Registries;

using InSite.Persistence;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Common.Events;

namespace InSite.Admin.Logs.Commands.Utilities
{
    public static class CommandHelper
    {
        private static readonly JSchemaGenerator _schemaGenerator;
        private static readonly ConcurrentDictionary<string, JSchema> _schemaStorage = new ConcurrentDictionary<string, JSchema>();

        static CommandHelper()
        {
            _schemaGenerator = new JSchemaGenerator
            {
                DefaultRequired = Required.DisallowNull
            };
            _schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
        }

        public static IAggregateTypeInfo[] GetAggregates() => TypeRegistry.GetAggregates();

        public static bool IsCommandJsonValid(Guid aggregateId, Guid commandId, string json, out IList<string> errors)
        {
            errors = new string[0];

            var aggregate = GetAggregateTypeInfo(aggregateId);
            if (aggregate == null)
            {
                errors = new[] { "Aggregate info not found" };
                return false;
            }

            var command = aggregate.Commands.Where(x => x.ID == commandId).FirstOrDefault();
            if (command == null)
            {
                errors = new[] { "Command info not found" };
                return false;
            }

            return IsCommandJsonValidInternal(command, json, out errors);
        }

        public static bool IsCommandJsonValid(Guid commandId, string json, out IList<string> errors)
        {
            errors = new string[0];

            var command = TypeRegistry.GetCommands().Where(x => x.ID == commandId).FirstOrDefault();
            if (command == null)
            {
                errors = new[] { "Command info not found" };
                return false;
            }

            return IsCommandJsonValidInternal(command, json, out errors);
        }

        private static bool IsCommandJsonValidInternal(IAggregateCommandTypeInfo command, string json, out IList<string> errors)
        {
            var schema = _schemaStorage.GetOrAdd(command.Type.FullName, x => _schemaGenerator.Generate(command.Type));

            try
            {
                var jObj = JObject.Parse(json);

                jObj.IsValid(schema, out errors);

                if (errors.Count > 0)
                    return false;
            }
            catch (JsonException jsonex)
            {
                errors = new[] { jsonex.Message };

                return false;
            }

            return true;
        }

        public static IAggregateCommandTypeInfo GetCommandTypeInfo(Type t) =>
            TypeRegistry.GetCommands().Where(x => x.Type == t).FirstOrDefault();

        public static IAggregateCommandTypeInfo GetCommandTypeInfo(Guid aggregateId, Guid commandId)
        {
            var aggregate = GetAggregateTypeInfo(aggregateId);

            return aggregate != null
                ? aggregate.Commands.Where(x => x.ID == commandId).FirstOrDefault()
                : null;
        }

        public static IAggregateCommandTypeInfo GetCommandTypeInfo(string name)
        {
            return name.IsEmpty() ? null : TypeRegistry.GetCommands().FirstOrDefault(x => x.Type.Name == name);
        }

        public static IAggregateTypeInfo GetAggregateTypeInfo(Type t)
        {
            return GetAggregates().Where(x => x.Type == t).SingleOrDefault();
        }

        public static IAggregateTypeInfo GetAggregateTypeInfo(Guid id)
        {
            return GetAggregates().Where(x => x.ID == id).SingleOrDefault();
        }

        public static AlertArgs SendCommand(Guid id)
        {
            var command = ServiceLocator.CommandStore.Get(id);
            if (command == null)
                return new AlertArgs(AlertType.Error, "The command not found");

            if (command.SendStarted.HasValue)
                return new AlertArgs(AlertType.Warning, "The command can't be started");

            try
            {
                ServiceLocator.CommandQueue.Start(id);

                return new AlertArgs(AlertType.Success, "The command has been successfully started");
            }
            catch (DbEntityException dbex)
            {
                AppSentry.SentryError(dbex);
                return dbex.InnerException is ApplicationError
                    ? new AlertArgs(AlertType.Error, dbex.InnerException.Message)
                    : new AlertArgs(AlertType.Error, "An error occurred during command execution");
            }
        }

        public static AlertArgs RepeatCommand(Guid id)
        {
            var command = ServiceLocator.CommandStore.Get(id);
            if (command == null)
                return new AlertArgs(AlertType.Error, "The command not found");

            // Here we want to repeat the original command, not try to restart the original command.

            var repeat = command.Deserialize(false);

            repeat.ExpectedVersion = null;
            repeat.OriginOrganization = CurrentSessionState.Identity.Organization.Identifier;
            repeat.OriginUser = CurrentSessionState.Identity.User.UserIdentifier;

            try
            {
                ServiceLocator.SendCommand(repeat);

                return new AlertArgs(AlertType.Success, "The command has been successfully repeated");
            }
            catch (DbEntityException dbex)
            {
                AppSentry.SentryError(dbex);

                return dbex.InnerException is ApplicationError
                    ? new AlertArgs(AlertType.Error, dbex.InnerException.Message)
                    : new AlertArgs(AlertType.Error, "An error occurred during command execution");
            }
        }

        internal static StatusType GetStatus(string value)
        {
            return value.ToEnum(StatusType.Unknown);
        }
    }
}