using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts.Utilities.TakerReport
{
    public static class TakerReportReader
    {
        public class File
        {
            public List<PersonRow> People { get; set; }
            public List<string> Errors { get; set; }
        }

        private static Guid OrganizationId => CurrentSessionState.Identity.OrganizationId;

        public static File Read(Guid fileId)
        {
            var (frameworks, people) = ParseCsvFile(fileId);
            var errors = new List<string>();

            var translations = ValidateFrameworks(frameworks, errors);
            ValidatePeople(people, frameworks.Count, errors);

            if (errors.Count == 0)
                TranslateFrameworks(people, translations);

            return new File
            {
                People = people,
                Errors = errors
            };
        }

        private static (List<string> frameworks, List<PersonRow> people) ParseCsvFile(Guid fileId)
        {
            string[][] values;
            var (_, stream) = ServiceLocator.StorageService.GetFileStream(fileId);
            using (stream)
                values = CsvImportHelper.GetValues(stream, null, false, Encoding.UTF8);

            var frameworks = new List<string>();
            for (int i = 9; i < values[0].Length; i++)
                frameworks.Add(values[0][i].TrimEnd());

            var people = new List<PersonRow>();
            for (int row = 1; row < values.Length; row++)
            {
                var line = values[row];

                var person = new PersonRow();
                people.Add(person);

                if (line.Length < 9)
                    continue;

                person.PersonCode = line[0].TrimEnd();
                person.CaseNumber = line[1].TrimEnd();
                person.CaseId = line[2].TrimEnd();
                person.FirstName = line[3].TrimEnd();
                person.MiddleName = line[4].TrimEnd();
                person.LastName = line[5].TrimEnd();
                person.Birthdate = line[6].TrimEnd();
                person.ExamDate = line[7].TrimEnd();
                person.ExamLanguage = line[8].TrimEnd();
                person.Frameworks = new List<PersonFramework>();

                for (int i = 9; i < line.Length; i++)
                {
                    person.Frameworks.Add(new PersonFramework
                    {
                        Status = line[i].TrimEnd(),
                        EnglishTitle = frameworks[i - 9]
                    });
                }
            }

            return (frameworks, people);
        }

        private static List<string> ValidateFrameworks(List<string> frameworks, List<string> errors)
        {
            var translations = new List<string>();

            foreach (var framework in frameworks)
            {
                var standard = StandardSearch.SelectFirst(x =>
                    x.OrganizationIdentifier == OrganizationId
                    && x.StandardType == "Framework"
                    && x.ContentTitle == framework
                );

                if (standard == null)
                {
                    errors.Add($"Framework '{framework}' is not found");
                    continue;
                }

                var text = ServiceLocator.ContentSearch.GetText(standard.StandardIdentifier, ContentLabel.Title, "fr");

                if (!string.IsNullOrEmpty(text))
                    translations.Add(text);
                else
                    translations.Add(framework);
            }

            return translations;
        }

        private static void ValidatePeople(List<PersonRow> people, int frameworkCount, List<string> errors)
        {
            for (int i = 0; i < people.Count; i++)
            {
                var person = people[i];

                if (string.IsNullOrEmpty(person.PersonCode))
                    errors.Add($"Line {i + 2}: Person Code is empty");

                if (string.IsNullOrEmpty(person.CaseNumber))
                    errors.Add($"Line {i + 2}: Case Number is empty");

                if (string.IsNullOrEmpty(person.CaseId))
                    errors.Add($"Line {i + 2}: Case Identifier is empty");
                else if (!Guid.TryParse(person.CaseId, out var caseId) || ServiceLocator.IssueSearch.GetIssue(caseId) == null)
                    errors.Add($"Line {i + 2}: Case does not exist");

                if (string.IsNullOrEmpty(person.FirstName))
                    errors.Add($"Line {i + 2}: First Name is empty");

                if (string.IsNullOrEmpty(person.LastName))
                    errors.Add($"Line {i + 2}: Last Name is empty");

                if (string.IsNullOrEmpty(person.ExamDate))
                    errors.Add($"Line {i + 2}: Exam Date is empty");

                if (!string.Equals(person.ExamLanguage, "English", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(person.ExamLanguage, "French", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    errors.Add($"Line {i + 2}: Exam Date is empty");
                }

                if (person.Frameworks.Count == 0
                    || person.Frameworks.Count != frameworkCount
                    || person.Frameworks.Any(x => !string.IsNullOrEmpty(x.Status)
                        && !string.Equals(x.Status, "Pass", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(x.Status, "Fail", StringComparison.OrdinalIgnoreCase))
                    )
                {
                    errors.Add($"Line {i + 2}: Framework status is invalid");
                }
            }
        }

        private static void TranslateFrameworks(List<PersonRow> people, List<string> translations)
        {
            foreach (var person in people)
            {
                for (int i = person.Frameworks.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(person.Frameworks[i].Status))
                        person.Frameworks.RemoveAt(i);
                    else
                        person.Frameworks[i].FrenchTitle = translations[i];
                }
            }
        }
    }
}
