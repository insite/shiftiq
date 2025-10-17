using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shift.Common
{
    public class MessageBuilder
    {
        #region Classes

        public enum Category
        {
            Information,
            ImportantInformation,
            Error,
            ImportantError,
            Warning,
            ImportantWarning
        }

        private class DataItem
        {
            public Category Category { get; }
            public string Format { get; }
            public object[] Arguments { get; }

            public DataItem(Category category, string format, object[] args)
            {
                Category = category;
                Format = format;
                Arguments = args;
            }

            public string GetText() => string.Format(Format, Arguments);
        }

        #endregion

        #region Fields

        private readonly List<DataItem> _items = new List<DataItem>();

        #endregion

        #region Properties

        public bool IsEmpty => _items.Count == 0;


        public bool HasInformation => _items.Any(x => x.Category == Category.Information);

        public bool HasImportantInformation => _items.Any(x => x.Category == Category.ImportantInformation);

        public bool HasError => _items.Any(x => x.Category == Category.Error);

        public bool HasImportantError => _items.Any(x => x.Category == Category.ImportantError);

        public bool HasWarning => _items.Any(x => x.Category == Category.Warning);

        public bool HasImportantWarning => _items.Any(x => x.Category == Category.ImportantWarning);


        public int InformationCount => _items.Count(x => x.Category == Category.Information);

        public int ImportantInformationCount => _items.Count(x => x.Category == Category.ImportantInformation);

        public int ErrorCount => _items.Count(x => x.Category == Category.Error);

        public int ImportantErrorCount => _items.Count(x => x.Category == Category.ImportantError);

        public int WarningCount => _items.Count(x => x.Category == Category.Warning);

        public int ImportantWarningCount => _items.Count(x => x.Category == Category.ImportantWarning);


        public IEnumerable<string> Informations => _items.Where(x => x.Category == Category.Information).Select(x => x.GetText());

        public IEnumerable<string> ImportantInformations => _items.Where(x => x.Category == Category.ImportantInformation).Select(x => x.GetText());

        public IEnumerable<string> Errors => _items.Where(x => x.Category == Category.Error).Select(x => x.GetText());

        public IEnumerable<string> ImportantErrors => _items.Where(x => x.Category == Category.ImportantError).Select(x => x.GetText());

        public IEnumerable<string> Warnings => _items.Where(x => x.Category == Category.Warning).Select(x => x.GetText());

        public IEnumerable<string> ImportantWarnings => _items.Where(x => x.Category == Category.ImportantWarning).Select(x => x.GetText());

        #endregion

        #region Methods (add)

        public void AddInformation(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.Information, format, args));
        }

        public void AddImportantInformation(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.ImportantInformation, format, args));
        }

        public void AddError(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.Error, format, args));
        }

        public void AddImportantError(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.ImportantError, format, args));
        }

        public void AddWarning(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.Warning, format, args));
        }

        public void AddImportantWarning(string format, params object[] args)
        {
            _items.Add(new DataItem(Category.ImportantWarning, format, args));
        }

        #endregion

        #region Methods (write)

        public void WriteHtml(StringBuilder html, int maxItems, params Category[] categories)
        {
            ToHtmlList(html, maxItems, _items.Where(x => categories.Contains(x.Category)).ToArray());
        }

        public void WriteInformation(StringBuilder html, int maxItems)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.Information).ToArray());
        }

        public void WriteImportantInformation(StringBuilder html, int maxItems)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.ImportantInformation).ToArray());
        }

        public void WriteErrors(StringBuilder html, int maxItems, params Category[] categories)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.Error).ToArray());
        }

        public void WriteImportantErrors(StringBuilder html, int maxItems)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.ImportantError).ToArray());
        }

        public void WriteWarnings(StringBuilder html, int maxItems)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.Warning).ToArray());
        }

        public void WriteImportantWarnings(StringBuilder html, int maxItems)
        {
            ToHtmlList(html, maxItems, _items.Where(x => x.Category == Category.ImportantWarning).ToArray());
        }

        private void ToHtmlList(StringBuilder html, int maxItems, DataItem[] items)
        {
            if (items.Length == 0)
                throw new ApplicationError("List is empty");

            html.Append("<ul>");

            var output = items;

            if (maxItems > 0 && items.Length > maxItems)
            {
                var groups = items.GroupBy(x => x.Format).Select(x => x.ToArray()).ToArray();
                var emptyGroups = groups.Length;
                var groupCounts = new int[emptyGroups];
                var itemsAlloc = maxItems;

                while (true)
                {
                    var isShortFound = false;
                    var itemsPerGroup = Number.CheckRange((int)Math.Floor((decimal)itemsAlloc / emptyGroups), 1);

                    for (var i = 0; i < groupCounts.Length; i++)
                    {
                        if (groupCounts[i] == 0 && groups[i].Length < itemsPerGroup)
                        {
                            var count = groups[i].Length;
                            groupCounts[i] = count;
                            itemsAlloc -= count;
                            emptyGroups--;
                            isShortFound = true;
                        }
                    }

                    if (isShortFound)
                        continue;

                    for (var i = 0; i < groupCounts.Length && itemsAlloc > 0; i++)
                    {
                        if (groupCounts[i] == 0)
                        {
                            groupCounts[i] = itemsPerGroup;
                            itemsAlloc -= itemsPerGroup;
                            emptyGroups--;
                        }
                    }

                    for (var i = 0; i < groupCounts.Length && itemsAlloc > 0; i++)
                    {
                        if (groups[i].Length - groupCounts[i] > 0)
                        {
                            groupCounts[i]++;
                            itemsAlloc--;
                        }
                    }

                    break;
                }

                output = groups.Select((x, i) => x.Take(groupCounts[i])).SelectMany(x => x).ToArray();
            }

            for (var i = 0; i < output.Length; i++)
            {
                html.Append("<li>");

                var item = output[i];
                if (item.Arguments.IsEmpty())
                    html.Append(item.Format);
                else
                    html.AppendFormat(item.Format, item.Arguments);

                html.Append(".</li>");
            }

            html.Append("</ul>");
        }

        #endregion
    }
}