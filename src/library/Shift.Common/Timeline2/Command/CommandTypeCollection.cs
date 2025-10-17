﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shift.Common
{
    public class CommandTypeCollection
    {
        private readonly Reflector _reflector;
        private readonly Dictionary<string, Type> _commands;
        private readonly Dictionary<string, string> _resources;

        public CommandTypeCollection(Assembly assembly, Type commandType, string[] excludeNamespaces)
        {
            _reflector = new Reflector();

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var commandTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(commandType)
                    && !t.Namespace.StartsWithAny(excludeNamespaces))
                .ToList();

            // FIXME: Throw an exception if duplicate command names exist.

            var duplicateCommandNames = commandTypes
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .Distinct()
                .ToList();

            commandTypes = commandTypes.Where(x => !duplicateCommandNames.Contains(x.Name)).ToList();

            _commands = commandTypes.ToDictionary(t => t.Name.ToLower(), t => t);

            _resources = CreateResources(_commands);
        }

        private Dictionary<string, string> CreateResources(Dictionary<string, Type> queries)
        {
            var list = new Dictionary<string, string>();

            foreach (var key in queries.Keys)
            {
                var slug = _reflector.GetResourceSlug(queries[key]);
                list.Add(slug, queries[key].FullName);
            }

            return list;
        }

        public Type GetCommandType(string name)
        {
            var typeNameVariations = GetTypeNameVariations(name);

            foreach (var typeName in typeNameVariations)
            {
                if (_commands.TryGetValue(typeName, out Type type))
                {
                    return type;
                }
            }

            return null;
        }

        private List<string> GetTypeNameVariations(string typeName)
        {
            var list = new List<string>();

            if (string.IsNullOrWhiteSpace(typeName))
                return list;

            list.Add(RemoveHyphens(typeName));

            for (var i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Trim().ToLower();
            }

            return list;
        }

        public static string RemoveHyphens(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("-", string.Empty);
        }

        public Dictionary<string, string> GetResources()
            => _resources;
    }
}