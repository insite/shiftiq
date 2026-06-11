using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

namespace Shift.Common.Timeline.Registries
{
    public static class TypeRegistry
    {
        private static readonly IAggregateTypeInfo[] _aggregates = null;
        private static readonly IAggregateCommandTypeInfo[] _commands = null;
        private static readonly IReadOnlyDictionary<string, Type> _changes = null;

        static TypeRegistry()
        {
            var allTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x.FullName.StartsWith("Shift.Sdk"))
                .SelectMany(s => s.GetTypes())
                .ToArray();

            var commands = GetCommandTypes(allTypes);

            _changes = GetChanges(allTypes);
            _aggregates = GetAggregateTypes(allTypes, commands);
            _commands = commands;

            foreach (var cmd in commands)
                cmd.Lock();
        }

        private static AggregateCommandTypeInfo[] GetCommandTypes(Type[] allTypes)
        {
            var baseCommandType = typeof(Command);

            return allTypes
                .Where(x => baseCommandType.IsAssignableFrom(x))
                .Select(x => new AggregateCommandTypeInfo(x))
                .OrderBy(x => x.Type.Name)
                .ToArray();
        }

        private static AggregateTypeInfo[] GetAggregateTypes(Type[] allTypes, AggregateCommandTypeInfo[] commands)
        {
            var aggRootType = typeof(AggregateRoot);

            var result = new List<AggregateTypeInfo>();

            for (var i = 0; i < allTypes.Length; i++)
            {
                var t = allTypes[i];
                if (!aggRootType.IsAssignableFrom(t))
                    continue;

                var dataProp = t.GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (dataProp == null)
                    continue;

                var aggInfo = new AggregateTypeInfo(t);

                aggInfo.Changes = GetChangeTypes(dataProp.PropertyType, aggInfo);
                aggInfo.Commands = GetCommandTypes(aggInfo, commands);

                result.Add(aggInfo);
            }

            return result.OrderBy(x => x.Name).ToArray();
        }

        private static AggregateChangeTypeInfo[] GetChangeTypes(Type aggData, AggregateTypeInfo aggInfo)
        {
            var baseChangeType = typeof(Change);

            var result = new List<AggregateChangeTypeInfo>();
            var dataMethods = aggData
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (var i = 0; i < dataMethods.Length; i++)
            {
                var m = dataMethods[i];
                if (m.Name != "When")
                    continue;

                var parameters = m.GetParameters();
                if (parameters.Length != 1)
                    continue;

                var changeType = parameters[0].ParameterType;
                if (!baseChangeType.IsAssignableFrom(changeType))
                    continue;

                result.Add(new AggregateChangeTypeInfo(changeType, aggInfo));
            }

            return result.OrderBy(x => x.Type.Name).ToArray();
        }

        private static IAggregateCommandTypeInfo[] GetCommandTypes(AggregateTypeInfo aggInfo, AggregateCommandTypeInfo[] commands)
        {
            var result = new List<IAggregateCommandTypeInfo>();
            var aggregateNamePlural = ToPlural(aggInfo.Name);
            var namespacePattern = new Regex($@"InSite\.Application(\.\w+)?\.{aggregateNamePlural}", RegexOptions.Compiled);

            for (var i = 0; i < commands.Length; i++)
            {
                var cmdInfo = commands[i];
                if (cmdInfo.Type.Namespace == null || !namespacePattern.IsMatch(cmdInfo.Type.Namespace))
                    continue;

                result.Add(cmdInfo);
                cmdInfo.AddAggregate(aggInfo);
            }

            return result.OrderBy(x => x.Type.Name).ToArray();
        }

        private static string ToPlural(string name)
        {
            switch (name)
            {
                case "Glossary":
                    return "Glossaries";

                case "Person":
                    return "People";

                case "Progress":
                    return "Progresses";

                default:
                    return name + "s";
            }
        }

        public static IAggregateTypeInfo[] GetAggregates() => _aggregates;

        public static IAggregateCommandTypeInfo[] GetCommands() => _commands;

        public static Type GetChangeType(string name)
        {
            if (_changes.TryGetValue(name, out var result))
                return result;

            throw new Exceptions.ChangeClassNotFoundException(name);
        }

        private static IReadOnlyDictionary<string, Type> GetChanges(Type[] allTypes)
        {
            var baseChangeType = typeof(Change);

            try
            {
                var result = new Dictionary<string, Type>();
                var duplicates = new List<string>();

                for (var i = 0; i < allTypes.Length; i++)
                {
                    var t = allTypes[i];
                    if (!t.IsSubclassOf(baseChangeType))
                        continue;

                    if (result.ContainsKey(t.Name))
                        duplicates.Add(t.Name);
                    else
                        result.Add(t.Name, t);
                }

                if (duplicates.Count > 0)
                {
                    var names = string.Join(", ", duplicates.OrderBy(x => x).Distinct());

                    throw new Exceptions.DuplicateChangeTypeNamesException(names);
                }

                return new ReadOnlyDictionary<string, Type>(result);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();

                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);

                    if (exSub is FileNotFoundException exFileNotFound)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }

                    sb.AppendLine();
                }

                var errorMessage = sb.ToString();

                //Display or log the error based on your application.
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }
    }
}
