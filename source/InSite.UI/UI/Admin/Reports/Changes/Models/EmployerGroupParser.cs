using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Shift.Common;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    class EmployerGroupParser
    {
        private static readonly Regex _linePattern = new Regex("(^|\\r\\n|\\r|\\n)- (?<FieldName>\\w+) changed from \\*(?<ValueBefore>.+?)\\* to \\*\\*(?<ValueAfter>.+?)\\*\\*", RegexOptions.Compiled);

        private readonly Dictionary<Guid, string> _employerGroupMapping = new Dictionary<Guid, string>();

        private readonly Guid _organizationId;

        public EmployerGroupParser(Guid organizationId)
        {
            _organizationId = organizationId;
        }

        public void AddEmployerGroups(string description)
        {
            var matches = _linePattern.Matches(description);
            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;

                var field = match.Groups["FieldName"];
                if (field.Value != "EmployerGroupIdentifier")
                    continue;

                if (Guid.TryParse(match.Groups["ValueBefore"].Value, out var valueBefore) && !_employerGroupMapping.ContainsKey(valueBefore))
                    _employerGroupMapping.Add(valueBefore, null);

                if (Guid.TryParse(match.Groups["ValueAfter"].Value, out var valueAfter) && !_employerGroupMapping.ContainsKey(valueAfter))
                    _employerGroupMapping.Add(valueAfter, null);
            }
        }

        public void LoadEmployerGroupNames()
        {
            if (_employerGroupMapping.Count == 0)
                return;

            var idFilter = _employerGroupMapping.Keys.ToArray();
            var values = ServiceLocator.GroupSearch.BindGroups(
                x => new
                {
                    x.GroupIdentifier,
                    x.GroupName,
                    x.OrganizationIdentifier,
                    OrganizationName = x.Organization.CompanyName
                },
                x => idFilter.Contains(x.GroupIdentifier));

            foreach (var value in values)
            {
                var name = value.GroupName;

                if (value.OrganizationIdentifier != _organizationId)
                    name += $" ({value.OrganizationName})";

                _employerGroupMapping[value.GroupIdentifier] = name;
            }
        }

        public string ReplaceGroupsWithName(string description)
        {
            return _linePattern.Replace(description, ReplaceGroupsWithName);
        }

        private string ReplaceGroupsWithName(Match m)
        {
            if (!m.Success || m.Groups["FieldName"].Value != "EmployerGroupIdentifier")
                return m.Value;

            var valueBefore = m.Groups["ValueBefore"].Value;
            var valueAfter = m.Groups["ValueAfter"].Value;

            if (Guid.TryParse(valueBefore, out var guidBefore) && _employerGroupMapping.TryGetValue(guidBefore, out string groupBefore) && groupBefore.IsNotEmpty())
                valueBefore = groupBefore;

            if (Guid.TryParse(valueAfter, out var guidAfter) && _employerGroupMapping.TryGetValue(guidAfter, out string groupAfter) && groupAfter.IsNotEmpty())
                valueAfter = groupAfter;

            return $"{m.Groups[1].Value}- EmployerGroup changed from *{valueBefore}* to **{valueAfter}**";
        }
    }

}