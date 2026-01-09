using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shift.Common
{
    public class Reflector : SimpleReflector
    {
        public Dictionary<string, string> FindConstants(Type type, char? separator = null)
        {
            var fields = new Dictionary<string, string>();

            FindConstantsRecursive(type, fields);

            if (separator.HasValue)
            {
                var list = new List<RelativePath>();
                foreach (var field in fields)
                    list.Add(new RelativePath(field.Key, field.Value, separator.Value));

                var collection = new RelativePathCollection(list);
                foreach (var item in collection.Items)
                    list.Add(new RelativePath(item.Name, item.Value, separator.Value));

                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (!fields.ContainsKey(item.Name))
                        fields.Add(item.Name, item.Value);
                }
            }

            return fields;
        }

        private void FindConstantsRecursive(Type type, Dictionary<string, string> collection)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field != null && field.IsLiteral)
                {
                    var fieldType = field.DeclaringType?.FullName;

                    var fieldValue = field.GetValue(null)?.ToString();

                    if (fieldType != null && fieldValue != null)
                    {
                        var fieldName = fieldType.Replace("+", ".") + "." + field.Name;

                        collection.Add(fieldName, fieldValue);
                    }
                }
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                FindConstantsRecursive(nestedType, collection);
            }
        }

        /// <summary>
        /// Returns the assembly-qualified class name without the version, culture, and public key token.
        /// </summary>
        public string GetClassName(Type type)
        {
            return $"{type.FullName}, {Assembly.GetAssembly(type).GetName().Name}";
        }

        /// <summary>
        /// Convert the type name (including namespace) to a kebab-case relative path.
        /// </summary>
        public string GetResourceName(Type type)
        {
            var className = type.FullName;
            var title = className.Replace(".", "/");
            var kebab = title.ToKebabCase();
            return kebab;
        }

        public static PropertyInfo[] GetSerializeProperties<T>(string includes)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (includes.IsEmpty())
                return properties;

            var nameMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var includesArray = includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in includesArray)
            {
                var trimmedName = name.Trim();
                if (!nameMapping.ContainsKey(trimmedName))
                    nameMapping.Add(trimmedName, nameMapping.Count);
            }

            var includeProperties = properties.Where(x => nameMapping.ContainsKey(x.Name)).OrderBy(x => nameMapping[x.Name]).ToArray();

            return includeProperties.Length > 0 ? includeProperties : properties;
        }
    }
}