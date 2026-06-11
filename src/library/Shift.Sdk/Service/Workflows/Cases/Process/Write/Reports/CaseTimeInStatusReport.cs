using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Application.Contacts.Read;
using InSite.Application.Files.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Organizations.Read;
using InSite.Domain.Issues;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Application.Issues.Write
{
    public class CaseTimeInStatusReport
    {
        private class CaseReport
        {
            public string PersonCode { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public int CaseNumber { get; set; }
            public string CaseType { get; set; }
            public string CaseStatus { get; set; }
            public DateTimeOffset CaseOpened { get; set; }
            public DateTimeOffset? CaseClosed { get; set; }
            public string CredentialingCountry { get; set; }
            public string HomeCountry { get; set; }
            public int DaysInOpenCategoryStatus { get; set; }
            public Dictionary<string, int> DaysInStatus { get; set; }
        }

        private readonly ICaseSearch _caseSearch;
        private readonly IChangeStore _changeStore;
        private readonly IPersonSearch _personSearch;
        private readonly IOrganizationSearch _organizationSearch;
        private readonly IStorageService _storageService;

        private Guid _organizationId;
        private TimeZoneInfo _timeZone;
        private Dictionary<Guid, string> _statuseNames;
        private HashSet<Guid> _openStatuses;
        private List<string> _orderedStatuses;

        public CaseTimeInStatusReport(ICaseSearch caseSearch, IChangeStore changeStore, IPersonSearch personSearch, IOrganizationSearch organizationSearch, IStorageService storageService)
        {
            _caseSearch = caseSearch;
            _changeStore = changeStore;
            _personSearch = personSearch;
            _organizationSearch = organizationSearch;
            _storageService = storageService;
        }

        public byte[] Create(Guid organizationId, string[] caseTypes)
        {
            _organizationId = organizationId;

            var organization = _organizationSearch.GetModel(_organizationId);
            _timeZone = organization.TimeZone;

            GetStatuses(caseTypes);

            var cases = GetCases(caseTypes);
            var now = DateTimeOffset.UtcNow;
            var reports = cases.Select(x => GetCaseReport(x, now)).ToList();

            RemoveUnusedStatuses(reports);

            return CreateCsv(reports);
        }

        private byte[] CreateCsv(List<CaseReport> reports)
        {
            var csv = new StringBuilder();
            csv.Append("Inspire ID,Last Name,First Name,Case Number,Case Type,Case Status,Case Opened Date,Case Closed Date,Origin of Nursing,Home Country,Total Days in Open Category Status");
            foreach (var statusName in _orderedStatuses)
                csv.Append($",Days in {statusName} status");

            csv.AppendLine();

            foreach (var caseReport in reports)
            {
                AppendCsvValue(csv, caseReport.PersonCode);
                AppendCsvValue(csv, caseReport.LastName);
                AppendCsvValue(csv, caseReport.FirstName);
                AppendCsvValue(csv, caseReport.CaseNumber.ToString());
                AppendCsvValue(csv, caseReport.CaseType);
                AppendCsvValue(csv, caseReport.CaseStatus);
                AppendCsvDateValue(csv, caseReport.CaseOpened);
                AppendCsvDateValue(csv, caseReport.CaseClosed);
                AppendCsvValue(csv, caseReport.CredentialingCountry);
                AppendCsvValue(csv, caseReport.HomeCountry);
                AppendCsvValue(csv, caseReport.DaysInOpenCategoryStatus.ToString());

                for (int i = 0; i < _orderedStatuses.Count; i++)
                {
                    var statusName = _orderedStatuses[i];
                    caseReport.DaysInStatus.TryGetValue(statusName, out var days);

                    AppendCsvValue(csv, days.ToString(), i != _orderedStatuses.Count - 1);
                }

                csv.AppendLine();
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private void AppendCsvDateValue(StringBuilder csv, DateTimeOffset? value)
        {
            string text;

            if (value.HasValue)
            {
                var converted = TimeZones.ConvertFromUtc(value.Value, _timeZone);
                text = $"{converted:yyyy-MM-dd}";
            }
            else
                text = null;

            csv.Append($"{text},");
        }

        private static void AppendCsvValue(StringBuilder csv, string value, bool appendComma = true)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains('"') || value.Contains(','))
                {
                    value = value.Replace("\"", "\"\"");
                    csv.Append($"\"{value}\"");
                }
                else
                    csv.Append($"{value}");
            }

            if (appendComma)
                csv.Append(',');
        }

        private void GetStatuses(string[] caseTypes)
        {
            _statuseNames = new Dictionary<Guid, string>();
            _openStatuses = new HashSet<Guid>();
            _orderedStatuses = new List<string>();

            foreach (var caseType in caseTypes)
            {
                var statuses = _caseSearch.GetStatuses(_organizationId, caseType);
                foreach (var status in statuses)
                {
                    _statuseNames.Add(status.StatusIdentifier, status.StatusName);
                    if (string.Equals(status.StatusCategory, "Open", StringComparison.OrdinalIgnoreCase))
                        _openStatuses.Add(status.StatusIdentifier);

                    if (!_orderedStatuses.Any(x => string.Equals(status.StatusName, x, StringComparison.OrdinalIgnoreCase)))
                        _orderedStatuses.Add(status.StatusName);
                }
            }
        }

        private void RemoveUnusedStatuses(List<CaseReport> reports)
        {
            var usedStatuses = new HashSet<string>();
            foreach (var caseReport in reports)
            {
                foreach (var statusName in caseReport.DaysInStatus.Keys)
                    usedStatuses.Add(statusName.ToLower());
            }

            for (int i = _orderedStatuses.Count - 1; i >= 0; i--)
            {
                if (!usedStatuses.Contains(_orderedStatuses[i].ToLower()))
                    _orderedStatuses.RemoveAt(i);
            }
        }

        private List<VIssue> GetCases(string[] caseTypes)
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = _organizationId,
                IssueTypes = caseTypes
            };
            return _caseSearch.GetIssues(filter);
        }

        private CaseReport GetCaseReport(VIssue @case, DateTimeOffset now)
        {
            var times = GetStatusTimes(@case.IssueIdentifier, now);
            var openDays = CalcOpenDays(times);
            var daysInStatus = ToDaysInStatus(@case.IssueStatusName, times);

            var homeAddress = @case.TopicHomeAddressIdentifier.HasValue ? _personSearch.GetPersonAddress(@case.TopicHomeAddressIdentifier.Value) : null;

            return new CaseReport
            {
                PersonCode = @case.TopicPersonCode,
                LastName = @case.TopicLastName,
                FirstName = @case.TopicFirstName,
                CaseNumber = @case.IssueNumber,
                CaseType = @case.IssueType,
                CaseStatus = @case.IssueStatusName,
                CaseOpened = @case.IssueOpened,
                CaseClosed = @case.IssueClosed,
                CredentialingCountry = @case.TopicCredentialingCountry,
                HomeCountry = homeAddress?.Country,
                DaysInOpenCategoryStatus = openDays,
                DaysInStatus = daysInStatus
            };
        }

        private int CalcOpenDays(Dictionary<Guid, TimeSpan> times)
        {
            var result = new TimeSpan();
            foreach (var timePair in times)
            {
                if (_openStatuses.Contains(timePair.Key))
                    result += timePair.Value;
            }
            return (int)Math.Ceiling(result.TotalDays);
        }

        private Dictionary<string, int> ToDaysInStatus(string caseType, Dictionary<Guid, TimeSpan> times)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in times)
            {
                if (!_statuseNames.TryGetValue(pair.Key, out var name))
                    continue;

                var days = (int)Math.Ceiling(pair.Value.TotalDays);

                result.TryGetValue(name, out var currentDays);

                result[name] = currentDays + days;
            }

            return result;
        }

        private Dictionary<Guid, TimeSpan> GetStatusTimes(Guid caseId, DateTimeOffset now)
        {
            var changes = _changeStore.GetChanges(caseId, -1);

            var prevStatusId = (Guid?)null;
            var prevTime = (DateTimeOffset?)null;
            var times = new Dictionary<Guid, TimeSpan>();

            foreach (var c in changes)
            {
                if (!(c is CaseStatusChanged statusChanged) || prevStatusId == statusChanged.Status)
                    continue;

                AddStatusTime(statusChanged.ChangeTime);

                prevStatusId = statusChanged.Status;
                prevTime = statusChanged.ChangeTime;
            }

            AddStatusTime(now);

            return times;

            void AddStatusTime(DateTimeOffset currentTime)
            {
                if (prevStatusId == null)
                    return;

                if (!times.TryGetValue(prevStatusId.Value, out var timespan))
                    timespan = new TimeSpan();

                timespan += currentTime - prevTime.Value;

                times[prevStatusId.Value] = timespan;
            }
        }
    }
}
