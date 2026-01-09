using System;
using System.Collections.Generic;
using System.Globalization;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Standards.Read;

using Shift.Common;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public static class ApiRequestBuilder
    {
        public static ExamEventCandidateInput GetExamEventCandidateInputForUpdate(QEvent @event, QBankForm form, QRegistration registration)
        {
            var input = new ExamEventCandidateInput
            {
                FormStatus = "Field",
                Exam = form.FormCode,
                AWISExamName = form.FormTitle.NullIf("None").IfNullOrEmpty(form.FormName),
                CourseSessionId = @event.EventClassCode ?? string.Empty,
                Trade = 0,
                Calculator = "",
                CodeBook = "",
                Active = "N",
                Removed = "N",
                RemovalReason = string.Empty,
                DictionaryAllowed = "N",
                IneligibleCodes = new string[0],
                IneligibilityOverride = "N",
                Void = "N"
            };

            // Assume an exam candidate is eligible if the status does not explicitly indicate otherwise.
            input.Eligible = registration.EligibilityStatus != "Not Eligible" ? "Y" : "N";

            // Convert the duration from minutes (InSite) to hours (Direct Access)
            input.Duration = (@event.ExamDurationInMinutes ?? 0) / 60;

            // If the exam submission is complete then indicate Pass or Fail.
            input.ResultStatus = registration.Attempt == null || !registration.Attempt.AttemptGraded.HasValue
                ? "Pending"
                : registration.Attempt.AttemptIsPassing
                    ? "Pass"
                    : "Fail";

            return input;
        }

        public static ExamEventCandidateInput GetExamEventCandidateInputForDelete(string classCode, string formCode, string formName, string reason)
        {
            var input = new ExamEventCandidateInput
            {
                FormStatus = "Field",
                Exam = formCode,
                AWISExamName = formName,
                CourseSessionId = classCode,
                Trade = 0,
                Calculator = "",
                CodeBook = "",
                Active = "N",
                Removed = "Y",
                RemovalReason = reason,
                DictionaryAllowed = "N",
                IneligibleCodes = new string[0],
                IneligibilityOverride = "N",
                Void = "N"
            };

            return input;
        }

        public static ExamSubmissionRequest CreateExamRegistrationRequest(QEvent @event, List<QRegistration> candidates, IRegistrationSearch registrationSearch, IBankSearch bankSearch, IOldStandardSearch standardSearch, IAttemptSearch attemptSearch, IContactSearch contactSearch)
        {
            return CreateExamSubmissionRequest(@event, candidates, bankSearch, registrationSearch, standardSearch, attemptSearch, contactSearch);
        }

        private static ExamSubmissionRequest CreateExamSubmissionRequest(QEvent @event, List<QRegistration> registrations, IBankSearch bankSearch, IRegistrationSearch registrationSearch, IOldStandardSearch standardSearch, IAttemptSearch attemptSearch, IContactSearch contactSearch)
        {
            var request = new ExamSubmissionRequest();
            DateTimeOffset? date = null;

            {
                var session = new ExamSubmissionSession
                {
                    ExamLocation = @event.VenueLocation?.GroupName ?? "Not Specified"
                };

                if (@event.EventClassCode != null)
                    session.SessionId = @event.EventClassCode;
                else
                    session.SessionId = @event.EventNumber.ToString();

                foreach (var registration in registrations)
                {
                    if (registration.EventIdentifier != @event.EventIdentifier)
                        continue;

                    if (!registration.ExamFormIdentifier.HasValue)
                        continue;

                    var form = bankSearch.GetForm(registration.ExamFormIdentifier.Value);
                    if (form == null)
                        continue;

                    var bank = bankSearch.GetBank(form.BankIdentifier);
                    if (bank == null)
                        continue;

                    var candidate = contactSearch.GetPerson(registration.CandidateIdentifier, registration.OrganizationIdentifier);
                    if (candidate == null)
                        continue;

                    var person = new ExamSubmissionPerson
                    {
                        ExamId = form.FormCode,
                        IsNoShow = !registration.IsPresent
                    };

                    if (int.TryParse(candidate.PersonCode, out int personCode))
                        person.IndividualId = personCode;

                    if (registration.AttemptIdentifier.HasValue)
                    {
                        var attempt = attemptSearch.GetAttempt(registration.AttemptIdentifier.Value);

                        if (attempt != null)
                        {
                            person.Mark = (int)Math.Round(100 * (attempt.AttemptScore ?? 0), MidpointRounding.AwayFromZero);

                            if (bank.FrameworkIdentifier.HasValue)
                            {
                                var method = standardSearch.GetCalculationMethod(bank.FrameworkIdentifier.Value);

                                person.Topics = LoadTopics(person, attemptSearch.GetAttemptQuestions(registration.AttemptIdentifier.Value), method);
                            }

                            if (!date.HasValue || (attempt.AttemptGraded.HasValue && attempt.AttemptGraded < date))
                                date = attempt.AttemptGraded;

                            if (attempt.AttemptImported.HasValue)
                            {
                                var pacific = TimeZones.FormatDateOnly(attempt.AttemptImported.Value,
                                    TimeZones.Pacific, CultureInfo.CurrentCulture, "{0:yyyy-MM-dd}");

                                person.SetVariable("AttemptImportDate", pacific);
                            }

                            if (attempt.AttemptGraded.HasValue)
                            {
                                var pacific = TimeZones.FormatDateOnly(attempt.AttemptGraded.Value,
                                    TimeZones.Pacific, CultureInfo.CurrentCulture, "{0:yyyy-MM-dd}");

                                person.SetVariable("AttemptGradeDate", pacific);
                            }
                        }
                    }

                    session.Persons.Add(person);
                }

                if (!date.HasValue)
                    date = @event.EventScheduledStart;

                var zone = TimeZones.Pacific;
                var time = TimeZoneInfo.ConvertTime(date.Value, zone);
                session.ExamCompletedDate = $"{time:yyyy-MM-dd}";

                session.SetVariable("EventExamType", @event.ExamType);
                session.SetVariable("EventNumber", @event.EventNumber.ToString());
                session.SetVariable("EventSchedulingStatus", @event.EventSchedulingStatus);

                if (session.Persons.Count > 0)
                    request.Sessions.Add(session);
            }

            return request;
        }

        private static List<ExamSubmissionTopic> LoadTopics(ExamSubmissionPerson person, List<QAttemptQuestion> list, string method)
        {
            return GetTopics(list, method);
        }

        private static List<ExamSubmissionTopic> GetTopics(List<QAttemptQuestion> questions, string calculationMethod)
        {
            var topics = new List<ExamSubmissionTopic>();

            var report = new CompetencyReport(questions);

            if (calculationMethod == "sum_tier0")
            {
                foreach (var folder in report.Folders)
                {
                    foreach (var item in folder.Items)
                    {
                        topics.Add(new ExamSubmissionTopic
                        {
                            Name = $"{folder.Code}{item.Code}. {item.Title}",
                            Mark = (int)(item.Score * 100)
                        });
                    }
                }
            }
            else
            {
                foreach (var folder in report.Folders)
                {
                    topics.Add(new ExamSubmissionTopic
                    {
                        Name = $"{folder.Label} {folder.Code}. {folder.Title}",
                        Mark = (int)(folder.Score * 100)
                    });
                }
            }

            return topics;
        }
    }
}