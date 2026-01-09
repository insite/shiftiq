using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class PersonListCsv : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FinalValidator.ServerValidate += FinalValidator_ServerValidate;
        }

        private void FinalValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var list = new List<PersonItem>();
            var errors = new List<string>();

            args.IsValid = Parse(list, errors);

            if (!args.IsValid)
            {
                var error = new StringBuilder();
                for (int i = 0; i < errors.Count && i < 20; i++)
                    error.Append($"<div>{errors[i]}</div>");

                FinalValidator.ErrorMessage = error.ToString();
            }
        }

        public List<PersonItem> GetPeople()
        {
            var list = new List<PersonItem>();

            Parse(list, null);

            return list;
        }

        private bool Parse(List<PersonItem> list, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(CsvText.Text))
            {
                errors.Add("Input text is empty");
                return false;
            }

            string[][] values;

            try
            {
                using (var reader = new StringReader(CsvText.Text))
                    values = CsvImportHelper.GetValues(reader, null, true);
            }
            catch
            {
                errors.Add("Input text has wrong format");
                return false;
            }

            var maxColumnCount = values.Max(x => x.Length);
            var minColumnCount = values.Min(x => x.Length);
            var columnCount = maxColumnCount != 6 ? maxColumnCount : minColumnCount;

            if (columnCount != 6)
            {
                errors.Add($"Expected 6 columns in the line but this input text has {columnCount}");
                return false;
            }

            for (int i = 0; i < values.Length; i++)
            {
                var row = values[i];

                var personItem = new PersonItem
                {
                    FirstName = GetRequired(row[0], i + 1, "First Name", errors),
                    LastName = GetRequired(row[1], i + 1, "Last Name", errors),
                    Email = row[2],
                    Birthdate = ParseDate(row[3], i + 1, errors),
                    PersonCode = row[4],
                    Phone = row[5]
                };

                if (personItem.Birthdate == null && string.IsNullOrEmpty(personItem.PersonCode))
                    errors.Add($"Line {i + 1}: either Birthday or Tradeworker Number should be specified");

                list.Add(personItem);
            }

            return errors.IsEmpty();
        }

        private static string GetRequired(string input, int line, string fieldName, List<string> errors)
        {
            if (string.IsNullOrEmpty(input))
                errors.Add($"Line {line}: {fieldName} must be specified");

            return input;
        }

        private static DateTime? ParseDate(string input, int line, List<string> errors)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (DateTime.TryParseExact(input, "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            errors.Add($"Line {line}: Date '{input}' has wrong format");

            return null;
        }
    }
}