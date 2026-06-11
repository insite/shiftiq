using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.UI.Admin.Assessments.Attempts.Controls;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Attempts.Utilities.TakerReport
{
    public static class TakerReportUploader
    {
        private const string FileExtension = ".pdf";

        private static Guid OrganizationId => CurrentSessionState.Identity.Organization.Identifier;
        private static Guid UserId => CurrentSessionState.Identity.User.Identifier;
        private static TimeZoneInfo TimeZone => CurrentSessionState.Identity.User.TimeZone;

        public static void UploadBatch(Page page, List<PersonRow> people)
        {
            foreach (var person in people)
            {
                var attempt = CreateAttempt(person);
                var caseId = Guid.Parse(person.CaseId);
                var report = TakerReportControl.GetPdf(page, new List<TakerReportControl.AttemptItem> { attempt });

                var now = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, TimeZone);
                var documentName = $"{person.LastName}_{person.FirstName}_{person.PersonCode}_CAMLPR_ExamReport_{now:yyyy}-{now:MM}-{now:dd}";

                SaveReportToCase(report, caseId, documentName);
            }
        }

        public static void SaveReportToCase(byte[] report, Guid issueId, string documentName)
        {
            var props = CreateFileProps(issueId, documentName);
            var fileName = CreateFileName(issueId, documentName);

            FileStorageModel model;

            using (var file = new MemoryStream(report))
            {
                model = ServiceLocator.StorageService.Create(
                    file,
                    fileName,
                    OrganizationId,
                    UserId,
                    issueId,
                    FileObjectType.Issue,
                    props,
                    null
                );
            }

            var command = new AddAttachment(
                issueId,
                model.FileName,
                Path.GetExtension(model.FileName),
                model.FileIdentifier,
                DateTimeOffset.UtcNow,
                UserId
            );

            ServiceLocator.SendCommand(command);
        }

        private static string CreateFileName(Guid issueId, string documentName)
        {
            var namePart = ServiceLocator.StorageService.AdjustFileName(documentName);
            var fileName = namePart + FileExtension;
            var number = 1;

            while (ServiceLocator.IssueSearch.GetAttachment(issueId, fileName) != null)
            {
                fileName = namePart + "-" + number + FileExtension;
                number++;
            }

            return fileName;
        }

        private static FileProperties CreateFileProps(Guid issueId, string documentName)
        {
            var settings = CurrentSessionState.Identity.Organization.Toolkits.Assessments?.TakerReport;

            return new FileProperties
            {
                DocumentName = documentName,
                Category = settings?.DocumentType,
                Subcategory = settings?.DocumentSubtype,
                Status = "System Generated",
                AllowLearnerToView = CaseAttachmentHelper.AllowLearnerToViewByIssue(issueId)
            };
        }

        private static TakerReportControl.AttemptItem CreateAttempt(PersonRow person)
        {
            var middleName = !string.IsNullOrEmpty(person.MiddleName) ? " " + person.MiddleName : "";
            var language = string.Equals(person.ExamLanguage, "English", StringComparison.OrdinalIgnoreCase) ? TakerReportControl.Language.English : TakerReportControl.Language.French;

            return new TakerReportControl.AttemptItem
            {
                PersonCode = person.PersonCode,
                FullName = $"{person.FirstName}{middleName} {person.LastName}",
                Birthdate = person.Birthdate,
                ExamDate = person.ExamDate,
                Language = language,
                Frameworks = person.Frameworks
                    .Select(x => new TakerReportControl.FrameworkItem
                    {
                        FrameworkTitle = language == TakerReportControl.Language.English ? x.EnglishTitle : x.FrenchTitle,
                        IsPass = string.Equals(x.Status, "Pass", StringComparison.OrdinalIgnoreCase)
                    }).ToList()
            };
        }
    }
}