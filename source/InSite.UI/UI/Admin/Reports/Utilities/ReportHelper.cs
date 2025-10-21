using System.Collections.Generic;

using InSite.Domain.Reports;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Reports.Utilities
{
    public static class ReportHelper
    {
        public static IEnumerable<ListItem> GetViewSelectorItems(this ReportDataSource source)
        {
            yield return new ListItem
            {
                Text = source.View.Title,
                Value = source.View.Alias
            };

            for (var i = 0; i < source.Joins.Length; i++)
            {
                var j = source.Joins[i];

                yield return new ListItem
                {
                    Text = j.Title,
                    Value = j.Alias
                };
            }
        }

        public static IEnumerable<VViewColumn> GetViewColumns(this ReportDataSource source, string alias)
        {
            var name = GetViewName(source, alias);

            GetNameParts(name, out var viewSchema, out var viewName);

            return VViewColumnSearch.Select(viewSchema, viewName);
        }

        public static string GetViewAlias(this ReportDataSource source, string name)
        {
            if (source.View.Name == name)
                return source.View.Alias;

            foreach (var join in source.Joins)
            {
                if (join.Name == name)
                    return join.Alias;
            }

            return null;
        }

        public static string GetViewName(this ReportDataSource source, string alias)
        {
            if (source.View.Alias == alias)
                return source.View.Name;

            foreach (var join in source.Joins)
            {
                if (join.Alias == alias)
                    return join.Name;
            }

            return null;
        }

        public static bool IsRequireColumn(this ReportAggregate.FunctionType func)
        {
            return func != ReportAggregate.FunctionType.Count;
        }

        public static string GetNamePart(string name, int index)
        {
            var strs = name.Split('.');
            return strs.Length > 0 && strs.Length > index ? strs[index] : null;
        }

        public static void GetNameParts(string name, out string part1, out string part2)
        {
            var strs = name.Split('.');

            if (strs.Length != 2)
                throw ApplicationError.Create("Unexpected name: " + name);

            part1 = strs[0];
            part2 = strs[1];
        }
    }
}