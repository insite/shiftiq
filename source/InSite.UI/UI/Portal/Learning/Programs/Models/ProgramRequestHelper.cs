using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Cases.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Records.Read;
using InSite.UI.Admin.Records.Programs.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Learning.Programs.Models
{
    static class ProgramRequestHelper
    {
        private const string CaseType = "Program Requested";
        private const string CaseStatus = "New Request";

        public class SubmitResult
        {
            public string Error { get; set; }
            public List<string> ProgramNames { get; set; }
        }

        private static Guid UserId => CurrentSessionState.Identity.User.Identifier;
        private static Guid OrganizationId => CurrentSessionState.Identity.Organization.Identifier;
        private static Guid? ParentOrganizationId => CurrentSessionState.Identity.Organization.ParentOrganizationIdentifier;

        public static SubmitResult Submit(List<Guid> programIds, string language)
        {
            var supervisorUserId = GetSupervisorUserId();
            if (supervisorUserId == null)
            {
                return new SubmitResult
                {
                    Error = "You cannot submit the program request without having a supervisor."
                };
            }

            var caseStatusId = GetCaseStatusId();
            if (caseStatusId == null)
            {
                return new SubmitResult
                {
                    Error = $"Case status '{CaseStatus}' is not found for the case type '{CaseType}'"
                };
            }

            var programs = ServiceLocator.ProgramSearch.GetProgramsForSubmit(OrganizationId, programIds, language);

            var commands = CreateCommands(programs, supervisorUserId.Value, caseStatusId.Value);

            ServiceLocator.SendCommands(commands);

            return new SubmitResult
            {
                ProgramNames = programs.Select(x => x.ProgramName).ToList()
            };
        }

        private static List<ICommand> CreateCommands(List<SubmittedProgram> programs, Guid supervisorUserId, Guid caseStatusId)
        {
            var caseId = UniqueIdentifier.Create();
            var number = ServiceLocator.IssueSearch.GetNextIssueNumber(OrganizationId);
            var employerGroupId = GetEmployerGroupId();
            var comment = CreateComment(programs);

            var commands = new List<ICommand>
            {
                new OpenIssue(caseId, OrganizationId, number, CaseType, null, DateTimeOffset.UtcNow, null, CaseType, DateTimeOffset.UtcNow),
                new AssignUser(caseId, supervisorUserId, "Administrator"),
                new AssignUser(caseId, UserId, "Topic"),
                new AssignUser(caseId, supervisorUserId, "Owner"),
                new ChangeIssueStatus(caseId, caseStatusId, DateTimeOffset.UtcNow),
            };

            if (employerGroupId.HasValue)
                commands.Add(new AssignGroup(caseId, employerGroupId.Value, "Employer"));

            commands.Add(new AuthorComment(
                caseId,
                UniqueIdentifier.Create(),
                comment,
                "Programs",
                null,
                UserIdentifiers.Someone,
                null,
                null,
                null,
                null,
                null,
                DateTimeOffset.UtcNow,
                null,
                null,
                null
            ));

            return commands;
        }

        private static string CreateComment(List<SubmittedProgram> programs)
        {
            var comment = new StringBuilder();
            comment.Append("The requested program(s):");

            foreach (var program in programs)
            {
                comment.AppendLine();
                comment.AppendLine();
                comment.AppendLine($"Program: {program.ProgramName}");
                comment.Append($"Categories: {string.Join(", ", program.CategoryNames)}");

                if (!string.IsNullOrEmpty(program.ProgramSummary))
                {
                    comment.AppendLine();
                    comment.AppendLine("Summary:");
                    comment.Append(program.ProgramSummary);
                }

                var achievements = GetAchievements(program.ProgramId);
                if (achievements.Count > 0)
                {
                    var achievementsText = string.Join("", achievements.Select(x => "\n - " + x));

                    comment.AppendLine();
                    comment.AppendLine();
                    comment.AppendLine($"Achievements: {achievementsText}");
                }
            }

            return comment.ToString();
        }

        private static List<string> GetAchievements(Guid programId)
        {
            var (_, items) = ProgramHelper.GetTasksAndItems(programId, "Achievement", OrganizationId, ParentOrganizationId);
            return items.Select(x => x.TaskName).ToList();
        }

        private static Guid? GetSupervisorUserId()
        {
            var filter = new QUserConnectionFilter
            {
                ToUserId = UserId,
                FromUserOrganizationId = OrganizationId,
                IsSupervisor = true,
            };

            var connections = ServiceLocator.UserSearch.GetConnections(filter);

            return connections.Count > 0 ? connections[0].FromUserIdentifier : (Guid?)null;
        }

        private static Guid? GetCaseStatusId()
        {
            var statuses = ServiceLocator.IssueSearch.GetStatuses(OrganizationId, CaseType);
            return statuses.Find(x => string.Equals(x.StatusName, CaseStatus, StringComparison.OrdinalIgnoreCase))?.StatusIdentifier;
        }

        private static Guid? GetEmployerGroupId()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserId, OrganizationId);
            if (person == null)
                throw new ArgumentException($"The user {UserId} is not found in the organization {OrganizationId}");

            return person.EmployerGroupIdentifier;
        }
    }
}