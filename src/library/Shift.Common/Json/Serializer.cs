using System;
using System.Collections.Generic;
using System.Reflection;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Shift.Common.Json
{
    public class Serializer : IJsonSerializer
    {
        static readonly Resolver IgnoreAttrResolver = new Resolver(null, true);
        static readonly CommandContractResolver CommandResolver = new CommandContractResolver(typeof(ICommand), new[] {
            nameof(ICommand.AggregateIdentifier),
            nameof(ICommand.ExpectedVersion),
            nameof(ICommand.OriginOrganization),
            nameof(ICommand.OriginUser),
            nameof(ICommand.CommandIdentifier)
        });
        static readonly CommandContractResolver ChangeResolver = new CommandContractResolver(typeof(IChange), new[] {
            nameof(IChange.AggregateIdentifier),
            nameof(IChange.AggregateState),
            nameof(IChange.AggregateVersion),
            nameof(IChange.ChangeTime),
            nameof(IChange.OriginOrganization),
            nameof(IChange.OriginUser)
        });

        static readonly Dictionary<string, Resolver> ContractResolvers = new Dictionary<string, Resolver>();
        static readonly Dictionary<string, Resolver> IgnoreAttrContractResolvers = new Dictionary<string, Resolver>();

        private Resolver GetContractResolver(string exclusions, bool ignoreAttributes)
        {
            var contractResolvers = ignoreAttributes
                ? IgnoreAttrContractResolvers
                : ContractResolvers;

            return GetContractResolver(exclusions, ignoreAttributes, contractResolvers);
        }

        private Resolver GetContractResolver(string exclusions, bool ignoreAttributes, Dictionary<string, Resolver> contractResolvers)
        {
            lock (this)
            {
                if (!contractResolvers.ContainsKey(exclusions))
                    contractResolvers.Add(exclusions, new Resolver(exclusions.Split(new char[] { ',' }), ignoreAttributes));
            }

            return contractResolvers[exclusions];
        }

        public T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default;
            return JsonConvert.DeserializeObject<T>(value);
        }

        public T Deserialize<T>(string value, Type type, bool ignoreAttributes)
        {
            if (string.IsNullOrEmpty(value))
                return default;

            var settings = ignoreAttributes
                ? new JsonSerializerSettings { ContractResolver = IgnoreAttrResolver }
                : null;

            return (T)JsonConvert.DeserializeObject(value, type, settings);
        }

        public string Serialize<T>(T value)
        {
            if (value == null)
                return null;

            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(value, settings);
        }

        /// <summary>
        /// Exclude the aggregate identifier/version and the event time/user from the serialized event. These
        /// properties are stored in their own discrete columns in the Command table, so we don't need them
        /// duplicated in the CommandData column.
        /// </summary>
        public string Serialize(object command, string[] exclusions, bool ignoreAttributes)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = GetContractResolver(string.Join(",", exclusions), ignoreAttributes),
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(command, settings);
        }

        /// <summary>
        /// Exclude the aggregate identifier/version and the event time/user from the serialized event. These
        /// properties are stored in their own discrete columns in the Command table, so we don't need them
        /// duplicated in the CommandData column.
        /// </summary>
        public string SerializeCommand(ICommand command)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = CommandResolver,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(command, settings);
        }

        /// <summary>
        /// Exclude the aggregate identifier/version and the event time/user from the serialized event. These
        /// properties are stored in their own discrete columns in the Command table, so we don't need them
        /// duplicated in the CommandData column.
        /// </summary>
        public string SerializeChange(IChange command)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = ChangeResolver,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(command, settings);
        }

        /// <summary>
        /// Returns the assembly-qualified class name without the version, culture, and public key token.
        /// </summary>
        public string GetClassName(Type type)
        {
            return $"{type.FullName}, {Assembly.GetAssembly(type).GetName().Name}";
        }
    }
}
