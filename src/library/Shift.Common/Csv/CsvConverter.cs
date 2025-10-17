using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shift.Common
{
    public static class CsvConverter
    {
        public static string ConvertListToCsvText(IEnumerable list, bool addQuotes = false)
        {
            if (list == null)
                return null;

            var result = new StringBuilder();

            foreach (object item in list)
            {
                if (result.Length != 0)
                    result.Append(',');

                if (addQuotes)
                {
                    if (item != null)
                        result.AppendFormat("'{0}'", item.ToString().Replace("'", "''"));
                }
                else
                    result.Append(item);
            }

            return result.ToString();
        }

        public static string ListToStringList(IEnumerable list, string delimiter, string qualifier = "")
        {
            if (list == null)
                return null;

            var result = new StringBuilder();

            foreach (object value in list)
            {
                if (result.Length > 0)
                    result.Append(delimiter);

                result.Append($"{qualifier}{value}{qualifier}");
            }

            return result.ToString();
        }

        public static string[] CsvTextToList(string csvText)
        {
            var list = new List<string>();

            if (!string.IsNullOrEmpty(csvText))
            {
                string[] parts = csvText.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in parts)
                {
                    string trimmed = s.Trim();

                    if (!string.IsNullOrEmpty(trimmed) && !list.Contains(trimmed))
                        list.Add(trimmed);
                }
            }

            return list.ToArray();
        }

        public static int[] CsvTextToIntList(string csvText)
        {
            if (string.IsNullOrEmpty(csvText))
                return new int[] {};

            var list = new List<int>();

            string[] parts = csvText.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in parts)
            {
                string trimmed = s.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                if (int.TryParse(trimmed, out var temp))
                    list.Add(temp);
            }

            return list.ToArray();
        }

        public static Guid[] CsvTextToGuidList(string csvText)
        {
            if (string.IsNullOrEmpty(csvText))
                return new Guid[] { };

            var list = new List<Guid>();

            string[] parts = csvText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in parts)
            {
                string trimmed = s.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                if (Guid.TryParse(trimmed, out var temp))
                    list.Add(temp);
            }

            return list.ToArray();
        }
    }
}