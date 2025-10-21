using System;
using System.Collections.Generic;

using InSite.Admin.Assessments.Attempts.Models;
using InSite.Application.Attempts.Read;

namespace InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport
{
    public class SkillsCheckReportData
    {
        public enum StandardStatus { Satisfied, NotSatisfied, PartiallySatisfied }

        public class Competency
        {
            public string Title { get; set; }
            public StandardStatus Status { get; set; }
        }

        public class Area
        {
            public string Title { get; set; }
            public StandardStatus Status { get; set; }
            public List<Competency> Competencies { get; set; }
        }

        public class Framework
        {
            public string Title { get; set; }
            public StandardStatus Status { get; set; }
            public List<Area> Areas { get; set; }
        }

        public class Occupation
        {
            public string Title { get; set; }
            public decimal Score { get; set; }
            public StandardStatus Status { get; set; }
            public List<Framework> Frameworks { get; set; }
        }

        public class Attempt
        {
            public string FormTitle { get; set; }
            public string UserName { get; set; }
            public DateTimeOffset Completed { get; set; }
            public decimal Score { get; set; }
            public bool IsPassing { get; set; }
            public List<Occupation> Occupations { get; set; }
        }

        public class Manager
        {
            public string UserName { get; set; }
            public string EmployerName { get; set; }
        }

        public static Manager GetManager(Guid managerUserId, Guid organizationId)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(managerUserId, organizationId, x => x.User, x => x.EmployerGroup);
            return new Manager
            {
                UserName = person?.User?.FullName ?? "Unknown User",
                EmployerName = person?.EmployerGroup?.GroupName ?? "Unknown Company"
            };
        }

        public static Attempt GetAttempt(Guid attemptId)
        {
            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
            settings.Filter = new QAttemptFilter { AttemptIdentifier = attemptId };

            var analysis = AttemptAnalysis.Create(settings);
            var sourceAttempt = analysis.HasData ? analysis.Attempts[0] : null;

            if (sourceAttempt == null || sourceAttempt.AttemptSubmitted == null || sourceAttempt.AttemptGraded == null)
                return null;

            var resultAttempt = new Attempt
            {
                Completed = sourceAttempt.AttemptSubmitted.Value,
                UserName = ServiceLocator.UserSearch.GetUser(sourceAttempt.LearnerUserIdentifier)?.FullName ?? "Unknown User",
                FormTitle = ServiceLocator.BankSearch.GetForm(sourceAttempt.FormIdentifier)?.FormTitle ?? "Unknown Assessment",
                Score = sourceAttempt.AttemptScore ?? 0,
                IsPassing = sourceAttempt.AttemptIsPassing,
                Occupations = new List<Occupation>()
            };

            AddOccupations(analysis, resultAttempt.Occupations);

            return resultAttempt;
        }

        private static void AddOccupations(AttemptAnalysis analysis, List<Occupation> occupations)
        {
            var sourceOccupations = StandardSummary.GetData(analysis, false);

            foreach (var sourceOccupation in sourceOccupations)
            {
                var occupation = new Occupation
                {
                    Title = sourceOccupation.Name,
                    Score = sourceOccupation.Score,
                    Frameworks = new List<Framework>()
                };
                occupations.Add(occupation);

                occupation.Status = AddFrameworks(sourceOccupation.Frameworks, occupation.Frameworks);
            }
        }

        private static StandardStatus AddFrameworks(List<StandardSummary.Framework> sourceFrameworks, List<Framework> frameworks)
        {
            var hasSatisfied = false;
            var hasNotSatisfied = false;
            var hasPartiallySatisfied = false;

            foreach (var sourceFramework in sourceFrameworks)
            {
                var framework = new Framework
                {
                    Title = sourceFramework.Name,
                    Areas = new List<Area>()
                };
                frameworks.Add(framework);

                framework.Status = AddAreas(sourceFramework.Gacs, framework.Areas);

                switch (framework.Status)
                {
                    case StandardStatus.Satisfied:
                        hasSatisfied = true;
                        break;
                    case StandardStatus.NotSatisfied:
                        hasNotSatisfied = true;
                        break;
                    default:
                        hasPartiallySatisfied = true;
                        break;
                }
            }

            return CalcStatus(hasSatisfied, hasNotSatisfied, hasPartiallySatisfied);
        }

        private static StandardStatus AddAreas(List<StandardSummary.Gac> sourceAreas, List<Area> areas)
        {
            var hasSatisfied = false;
            var hasNotSatisfied = false;
            var hasPartiallySatisfied = false;

            foreach (var sourceArea in sourceAreas)
            {
                var area = new Area
                {
                    Title = sourceArea.Name,
                    Competencies = new List<Competency>()
                };
                areas.Add(area);

                area.Status = AddCompetencies(sourceArea.Competencies, area.Competencies);

                switch (area.Status)
                {
                    case StandardStatus.Satisfied:
                        hasSatisfied = true;
                        break;
                    case StandardStatus.NotSatisfied:
                        hasNotSatisfied = true;
                        break;
                    default:
                        hasPartiallySatisfied = true;
                        break;
                }
            }

            return CalcStatus(hasSatisfied, hasNotSatisfied, hasPartiallySatisfied);
        }

        private static StandardStatus AddCompetencies(List<StandardSummary.Competency> sourceCompetencies, List<Competency> competencies)
        {
            var hasSatisfied = false;
            var hasNotSatisfied = false;

            foreach (var sourceCompetency in sourceCompetencies)
            {
                var competency = new Competency
                {
                    Title = sourceCompetency.Name,
                    Status = sourceCompetency.AnswerPoints >= sourceCompetency.QuestionPoints ? StandardStatus.Satisfied : StandardStatus.NotSatisfied
                };
                competencies.Add(competency);

                if (competency.Status == StandardStatus.Satisfied)
                    hasSatisfied = true;
                else if (competency.Status == StandardStatus.NotSatisfied)
                    hasNotSatisfied = true;
            }

            return CalcStatus(hasSatisfied, hasNotSatisfied, false);
        }

        private static StandardStatus CalcStatus(bool hasSatisfied, bool hasNotSatisfied, bool hasPartiallySatisfied)
        {
            if (hasSatisfied)
                return hasNotSatisfied || hasPartiallySatisfied ? StandardStatus.PartiallySatisfied : StandardStatus.Satisfied;

            return hasPartiallySatisfied ? StandardStatus.PartiallySatisfied : StandardStatus.NotSatisfied;
        }
    }
}