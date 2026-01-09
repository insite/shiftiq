using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Standards.Read;

using Shift.Common;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    class EventVariableBuilder
    {
        public class CompetencyItem
        {
            public string Code { get; set; }
            public string Title { get; set; }
        }

        public class FormItem
        {
            public Guid? Identifier { get; set; }
            public string Title { get; set; }
        }

        private readonly EventProcessorHelper _helper;

        public EventVariableBuilder(EventProcessorHelper helper)
        {
            _helper = helper;
        }

        public void ExamEvent(MessageVariableList list, QEvent @event, QGroupAddress venueAddress)
        {
            const string TrainingProviderItem = @"Contact Name: {0}

Contact Phone: {1}";

            const string TrainingProviderItem003 = @"Training Provider Contact: {0} {1}";

            var venueAddressText = GetLocationPhysicalText(venueAddress);

            list.AddValue("EventIdentifier", @event.EventIdentifier);
            list.AddValue("ExamType", @event.ExamType);

            list.AddValue("ActivityBillingCode", @event.EventBillingType);
            list.AddValue("ActivityClassCode", @event.EventClassCode);
            list.AddValue("ActivityScheduledCountdownDays", Shift.Common.Humanizer.ToQuantity(@event.EventScheduledCountdownDays, "day"));
            list.AddValue("ActivityDate", @event.EventDate);
            list.AddValue("ActivityFormat", @event.EventFormat);
            list.AddValue("ActivityNumber", @event.EventNumber.ToString());
            list.AddValue("ActivityTimeLess30Minutes", @event.EventTimeLess30Minutes);
            list.AddValue("ActivityTime", @event.EventTime);
            list.AddValue("ActivityTitle", @event.EventTitle);
            list.AddValue("ActivityType", @event.EventType);

            list.AddValue("CurrentDatePlus60Days", string.Format("{0:MMMM d, yyyy}", DateTime.Today.AddDays(60)));
            list.AddValue("CurrentDate", string.Format("{0:MMMM d, yyyy}", DateTime.Today));

            list.AddValue("DistributionExpectedCountdownDays", @event.DistributionExpectedCountdownDays);
            list.AddValue("DistributionExpected", @event.DistributionExpectedText);
            list.AddValue("ExamMaterialReturnShipmentReceived", @event.ExamMaterialReturnShipmentReceived);
            list.AddValue("VenueLocationPhysical", venueAddressText);

            list.AddValue("VenueName", @event.VenueLocationName);
            list.AddValue("VenueOffice", @event.VenueOfficeName);
            list.AddValue("VenuePhone", @event.VenueLocation?.GroupPhone);
            list.AddValue("VenueRoom", @event.VenueRoom);

            list.AddValue("TrainingProviderList003", CreateTrainingProviderList(@event, TrainingProviderItem003));
            list.AddValue("TrainingProviderList", CreateTrainingProviderList(@event, TrainingProviderItem));

            AddDistributionExpectedReturn(list, @event);
        }

        private void AddDistributionExpectedReturn(MessageVariableList list, QEvent @event)
        {
            if (!list.IsVariableAccepted("DistributionReturn"))
                return;

            int dayCount;
            var days = _helper.GetITA026DayCount(@event);
            switch (days)
            {
                case EventProcessorHelper.ITA026DayCount.Days14:
                    dayCount = 14;
                    break;
                case EventProcessorHelper.ITA026DayCount.Days60:
                    dayCount = 60;
                    break;
                default:
                    return;
            }

            var distributionReturn = @event.EventScheduledStart.AddDays(dayCount);

            list.AddValue("DistributionReturn", distributionReturn.FormatDateOnly());
        }

        private static string GetLocationPhysicalText(QGroupAddress address)
        {
            if (address == null)
                return null;

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(address.Street1))
                sb.AppendFormat("{0}, ", address.Street1);

            if (!string.IsNullOrEmpty(address.Street2))
                sb.AppendFormat("{0}, ", address.Street2);

            if (!string.IsNullOrEmpty(address.City))
                sb.AppendFormat("{0}, ", address.City);

            if (!string.IsNullOrEmpty(address.Province))
                sb.AppendFormat("{0}", address.Province);

            if (!string.IsNullOrEmpty(address.PostalCode))
                sb.AppendFormat(" {0}", address.PostalCode);

            return sb.ToString();
        }

        private string CreateTrainingProviderList(QEvent @event, string template)
        {
            var contacts = GetTrainingProviderContacts(@event);
            var result = new StringBuilder();

            foreach (var contact in contacts)
            {
                if (result.Length > 0)
                {
                    result.AppendLine();
                    result.AppendLine();
                }

                result.AppendFormat(template, contact.UserFullName, contact.UserPhone);
            }

            return result.ToString();
        }

        private static List<VPerson> GetTrainingProviderContacts(QEvent @event)
        {
            var contacts = new List<VPerson>();

            if (@event.VenueCoordinator != null)
                contacts.Add(@event.VenueCoordinator);

            if (@event.Attendees != null)
            {
                var secondaryContacts = @event.Attendees
                    .Where(x => string.Equals(x.AttendeeRole, "Training Provider (Class) Venue Contact", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Person)
                    .OrderBy(x => x.User.UserFullName);

                contacts.AddRange(secondaryContacts);
            }

            return contacts;
        }

        public void CandidateRegistrationTable(MessageVariableList list, QRegistration[] registrations)
        {
            { // CandidateRegistrationTable
                var md = new StringBuilder();
                md.AppendLine("Name | Code | Exam | Status | Accommodation | Materials");
                md.AppendLine(":-- |:-- |:-- |:-- |:-- |:--");
                foreach (var registration in registrations)
                {
                    var accommodations = GetAccommodations(registration, true);
                    var materials = registration.MaterialsPermittedToCandidates;
                    var reason = StringHelper.Equals(registration.ApprovalStatus, "Eligible")
                        ? string.Empty : registration.ApprovalReason;
                    md.AppendLine($"{registration.Candidate.UserFullName} | {registration.Candidate.PersonCode} | {registration.Form?.FormTitle} | {registration.ApprovalStatus} {(reason != null ? ": " + reason : "")} | {accommodations} | {materials}");
                }
                list.AddValue("CandidateRegistrationTable", md.ToString());
            }
        }

        public void CandidateAuthenticationTable(MessageVariableList list, QRegistration[] registrations)
        {
            // CandidateAuthenticationTable
            var md = new StringBuilder();
            md.AppendLine("Name | Code | Exam | Password | Status | Accommodation | Materials | Reference Materials");
            md.AppendLine(":-- |:-- |:-- |:-- |:-- |:--");
            foreach (var registration in registrations)
            {
                var accommodations = GetAccommodations(registration, true);
                var materials = registration.MaterialsPermittedToCandidates;
                var referenceMaterials = (registration.Form?.FormHasReferenceMaterials).IsEmpty()
                    ? "No"
                    : registration.Form.FormHasReferenceMaterials;

                md.AppendLine($"{registration.Candidate.UserFullName} | {registration.Candidate.PersonCode} | {registration.Form?.FormTitle} | {registration.RegistrationPassword} | {registration.ApprovalStatus} | {accommodations} | {materials} | {referenceMaterials}");
            }
            list.AddValue("CandidateAuthenticationTable", md.ToString());
        }

        public static string GetAccommodations(QRegistration registration, bool includeTimeLimit)
        {
            var output = string.Empty;

            if (registration.Accommodations.IsEmpty())
                output = "None";
            else
                output = string.Join(", ", registration.Accommodations.Select(x => { return x.AccommodationType; }));

            if (includeTimeLimit)
            {
                if (output.Length > 0)
                    output += ", ";
                var candidateTimeLimit = Math.Round((registration.ExamTimeLimit ?? 0) / 60.0, 1);
                output += $"Time Limit: {candidateTimeLimit:n1} h";
            }

            return output;
        }

        public static string GetAccommodationNames(QRegistration registration)
        {
            var output = string.Join(
                ", ",
                registration.Accommodations
                    .Where(x => x.AccommodationName.IsNotEmpty())
                    .Select(x => x.AccommodationName));

            return output.Length == 0 ? "None" : output;
        }

        public static string GetAccommodationTimeExtension(QRegistration registration)
        {
            var extSum = registration.Accommodations.Where(x => x.TimeExtension.HasValue && x.TimeExtension.Value > 0).Sum(x => x.TimeExtension.Value);
            return extSum > 0 ? extSum.Minutes().Humanize(2, minUnit: TimeUnit.Minute) : "None";
        }

        public static string GetAccommodationTable(QRegistration registration)
        {
            var md = new StringBuilder();
            md.AppendLine("Accommodation Type | Name | Time Extension");
            md.AppendLine(":-- |:-- |:--");

            foreach (var accommodation in registration.Accommodations.OrderBy(x => x.AccommodationType))
            {
                var timeExtension = !accommodation.TimeExtension.HasValue || accommodation.TimeExtension.Value <= 0
                    ? string.Empty
                    : accommodation.TimeExtension.Value.Minutes().Humanize(2, minUnit: TimeUnit.Minute);

                md.AppendLine($"{accommodation.AccommodationType} | {accommodation.AccommodationName} | {timeExtension}");
            }

            return md.ToString();
        }

        public void CandidateSubmissionTable(MessageVariableList list, QRegistration[] registrations, QAttempt[] attempts, bool isSingleForm)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));
            if (attempts == null) throw new ArgumentNullException(nameof(attempts));

            var md = StartCandidateSubmissionTable(isSingleForm);
            foreach (var registration in registrations.Where(x => x != null && x.IsPresent))
            {
                WriteLearnerToCandidateSubmissionTable(md, registration, isSingleForm);

                var attempt = registration.AttemptIdentifier.HasValue
                    ? attempts.FirstOrDefault(x => x != null && x.AttemptIdentifier == registration.AttemptIdentifier)
                    : null;

                WriteScoreToCandidateSubmissionTable(md, attempt);
            }
            list.AddValue("CandidateSubmissionTable", md.ToString());
        }

        private StringBuilder StartCandidateSubmissionTable(bool isSingleForm)
        {
            var md = new StringBuilder();
            if (isSingleForm)
            {
                md.AppendLine("Name | Code | Score | Grade");
                md.AppendLine(":-- |:-- |:-- |:--");
            }
            else
            {
                md.AppendLine("Name | Code | Form | Score | Grade");
                md.AppendLine(":-- |:-- |:-- |:-- |:--");
            }
            return md;
        }

        private void WriteLearnerToCandidateSubmissionTable(StringBuilder md, QRegistration registration, bool isSingleForm)
        {
            md.Append($"{registration.Candidate?.UserFullName} | {registration.Candidate?.PersonCode}");
            if (!isSingleForm)
                md.Append($" | {registration.Form?.FormTitle}");
        }

        private void WriteScoreToCandidateSubmissionTable(StringBuilder md, QAttempt attempt)
        {
            var score = (attempt?.AttemptScore) ?? 0;
            var grade = string.Empty;
            if (attempt != null)
                grade = attempt.AttemptIsPassing ? "Pass" : "Fail";

            md.AppendLine($" | {score:p0} | {grade}");
        }

        public void CandidatePublicationTable(MessageVariableList list, QRegistration[] registrations, QAttempt[] attempts)
        {
            var md = new StringBuilder();
            md.AppendLine("Name | Code | Form | Score | Grade Status");
            md.AppendLine("-- | -- | --");
            foreach (var registration in registrations)
            {
                var attempt = registration.AttemptIdentifier.HasValue ? attempts.FirstOrDefault(x => x != null && x.AttemptIdentifier == registration.AttemptIdentifier) : null;
                md.AppendLine($"{registration.Candidate?.UserFullName} | {registration.Candidate?.PersonCode} | {registration.Form?.FormTitle} ({registration.Form?.FormAsset}.{registration.Form?.FormAssetVersion}) | {((attempt?.AttemptScore) ?? 0):p0} | {registration.GradingStatus}");
            }
            list.AddValue("CandidatePublicationTable", md.ToString());
        }

        public void FormCompetencyTable(MessageVariableList list, QRegistration[] registrations, QAttempt[] attempts, bool isSingleForm, FormItem[] forms)
        {
            var md = new StringBuilder();
            foreach (var form in forms)
            {
                var questions = registrations
                    .Where(x => x.ExamFormIdentifier == form.Identifier)
                    .SelectMany(x =>
                    {
                        var attempt = x.AttemptIdentifier.HasValue ? attempts.FirstOrDefault(y => y != null && y.AttemptIdentifier == x.AttemptIdentifier) : null;
                        return attempt != null ? attempt.Questions : new QAttemptQuestion[0];
                    }).ToArray();

                var competencies = GetCompetencyList(questions);

                if (!isSingleForm)
                    md.AppendLine($"## {form.Title}");

                md.AppendLine("Code | Title");
                md.AppendLine(":-- |:--");
                foreach (var item in competencies)
                    md.AppendLine($"{item.Code} | {item.Title}");
            }
            list.AddValue("FolderCompetencyTable", md.ToString());
        }

        private List<CompetencyItem> GetCompetencyList(QAttemptQuestion[] questions)
        {
            var list = new List<CompetencyItem>();

            foreach (var question in questions)
            {
                if (question.CompetencyAreaCode != null && question.CompetencyAreaTitle != null)
                {
                    if (!list.Any(x => x.Code == question.CompetencyAreaCode))
                    {
                        var item = new CompetencyItem
                        {
                            Code = question.CompetencyAreaCode,
                            Title = question.CompetencyAreaTitle
                        };
                        list.Add(item);
                    }
                }
            }

            return list.OrderBy(x => x.Code).ToList();
        }

        public void CandidateCompetencyTable(MessageVariableList list, QRegistration[] registrations, QAttempt[] attempts, bool isSingleForm, FormItem[] forms)
        {
            var md = new StringBuilder();

            try
            {
                foreach (var form in forms)
                {
                    if (!isSingleForm)
                        md.AppendLine($"## {form.Title}");

                    var reports = registrations
                        .Where(x => x.IsPresent && x.ExamFormIdentifier == form.Identifier)
                        .Select(x =>
                        {
                            var attempt = x.AttemptIdentifier.HasValue ? attempts.FirstOrDefault(y => y != null && y.AttemptIdentifier == x.AttemptIdentifier) : null;

                            return new
                            {
                                x.CandidateIdentifier,
                                CandidateColumnName = $"**{x.Candidate.UserFullName}** ({x.Candidate.PersonCode})",
                                CompetencyReport = new CompetencyReport(attempt?.Questions)
                            };
                        }).ToArray();

                    var folders = reports
                        .SelectMany(x => x.CompetencyReport.Folders.Select(y => new { y.Identifier, y.Code }))
                        .GroupBy(x => x.Identifier)
                        .Select(x =>
                        {
                            var info = x.First();

                            return new
                            {
                                info.Code,
                                ReportFolders = reports.Select(y => y.CompetencyReport.Folders.FirstOrDefault(z => z.Identifier == info.Identifier)).ToArray()
                            };
                        })
                        .OrderBy(x => x.Code)
                        .ToArray();

                    md.Append("Candidate / Topic");
                    foreach (var folder in folders)
                        md.Append(" | ").Append(folder.Code);
                    md.AppendLine(" | Total");

                    md.Append(":--");
                    foreach (var folder in folders)
                        md.Append("|:--:");
                    md.AppendLine("|:--:");

                    if (reports.Length > 0)
                    {
                        var reportFolders = reports.Select(x => x.CompetencyReport.Folders).Where(x => x.Count > 0).FirstOrDefault();
                        if (reportFolders != null)
                        {
                            md.Append("**Number of Questions**");

                            var total = 0;

                            foreach (var rf in reportFolders)
                            {
                                var count = rf == null ? 0 : rf.GetQuestionsCount();

                                total += count;

                                md.Append(" | ").AppendFormat("{0:n0}", count);
                            }

                            md.Append(" | ").AppendFormat("{0:n0}", total).AppendLine();
                        }
                    }

                    foreach (var report in reports)
                    {
                        md.Append(report.CandidateColumnName);

                        var answersPointsSum = 0M;
                        var questionsPointsSum = 0M;

                        foreach (var reportFolder in report.CompetencyReport.Folders)
                        {
                            md.Append(" | ");

                            var answerPoints = 0M;
                            var questionPoints = 0M;

                            if (reportFolder != null)
                            {
                                answerPoints = reportFolder.GetAnswersPoints();
                                questionPoints = reportFolder.GetQuestionsPoints();
                            }

                            md.Append(GetPercent(answerPoints, questionPoints));

                            answersPointsSum += answerPoints;
                            questionsPointsSum += questionPoints;
                        }

                        md.Append(" | ").Append(GetPercent(answersPointsSum, questionsPointsSum)).AppendLine();
                    }

                    md.Append("**Average**");

                    var totalAnswersPointsSum = 0M;
                    var totalQuestionsPointsSum = 0M;

                    foreach (var folder in folders)
                    {
                        var answerPoints = 0M;
                        var questionPoints = 0M;

                        foreach (var reportFolder in folder.ReportFolders)
                        {
                            if (reportFolder != null)
                            {
                                answerPoints += reportFolder.GetAnswersPoints();
                                questionPoints += reportFolder.GetQuestionsPoints();
                            }
                        }

                        md.Append(" | ").Append(GetPercent(answerPoints, questionPoints));

                        totalAnswersPointsSum += answerPoints;
                        totalQuestionsPointsSum += questionPoints;
                    }

                    md.Append(" | ").Append(GetPercent(totalAnswersPointsSum, totalQuestionsPointsSum)).AppendLine();
                }
            }
            catch (Exception)
            {
                // Ignore exceptions unless we can find a way to reproduce them. Ideally, this method should be
                // rewritten to create a list of objects that is subsequently used to generate the Markdown content.
            }

            list.AddValue("CandidateCompetencyTable", md.ToString());

            string GetPercent(decimal part, decimal total)
            {
                return total == 0 ? string.Empty : $"{Calculator.GetPercentDecimal(part, total):p0}";
            }
        }
    }
}