using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Application.Standards.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks.Models
{
    public static class CompetencyHelper
    {
        public class CompetencyItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
            public decimal? Hours { get; set; }
            public int? JournalItems { get; set; }
            public int? SkillRating { get; set; }
            public bool IncludeHoursToArea { get; set; }
            public string SatisfactionLevel { get; set; }
        }

        public class AreaItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyItem> Competencies { get; set; }
        }

        public static List<AreaItem> GetAreasByExperience(Guid experienceIdentifier, Guid journalSetupIdentifier, Guid userIdentifier, string language, bool useSimpleNameConvention = false)
        {
            var experienceCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(experienceIdentifier, x => x.Competency, x => x.Experience);
            if (experienceCompetencies.Count == 0)
                return null;

            var userCompetencies =
                ServiceLocator.JournalSearch
                    .GetExperienceCompetencies(
                        new QExperienceCompetencyFilter
                        {
                            JournalSetupIdentifier = journalSetupIdentifier,
                            UserIdentifier = userIdentifier
                        },
                        x => x.Experience)
                    .GroupBy(x => x.CompetencyStandardIdentifier)
                    .ToDictionary(g => g.Key, g => (int?)g.Count());

            var areas = new List<AreaItem>();

            var classifications = StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            foreach (var experienceCompetency in experienceCompetencies)
            {
                if (!userCompetencies.TryGetValue(experienceCompetency.CompetencyStandardIdentifier, out var journalItems))
                    journalItems = null;

                AddCompetency(
                    areas,
                    classifications,
                    experienceCompetency.Competency,
                    experienceCompetency.CompetencyHours,
                    journalItems,
                    null,
                    false,
                    experienceCompetency.SatisfactionLevel,
                    language,
                    useSimpleNameConvention
                );
            }

            areas = areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            foreach (var area in areas)
                area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

            return areas;
        }

        public static List<AreaItem> GetAreas(Guid journalSetupIdentifier, string language, bool useSimpleNameConvention = false)
        {
            var requirements = ServiceLocator.JournalSearch.GetCompetencyRequirements(journalSetupIdentifier, x => x.Competency);
            if (requirements.Count == 0)
                return null;

            var areas = new List<AreaItem>();

            var classifications = StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            foreach (var requirement in requirements)
            {
                AddCompetency(
                    areas,
                    classifications,
                    requirement.Competency,
                    requirement.CompetencyHours,
                    requirement.JournalItems,
                    requirement.SkillRating,
                    requirement.IncludeHoursToArea,
                    null,
                    language,
                    useSimpleNameConvention
                );
            }

            areas = areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            foreach (var area in areas)
                area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

            return areas;
        }

        private static void AddCompetency(
            List<AreaItem> areas,
            string[] classifications,
            VCompetency competency,
            decimal? hours,
            int? journalItems,
            int? skillRating,
            bool includeHoursToArea,
            string satisfactionLevel,
            string language,
            bool useSimpleNameConvention = false
            )
        {
            var area = areas.Find(x => x.Identifier == competency.AreaIdentifier);
            if (area == null)
            {
                if (competency.AreaIdentifier == null)
                    return;

                area = new AreaItem
                {
                    Identifier = competency.AreaIdentifier.Value,
                    Sequence = competency.AreaSequence.Value,
                    Name = GetStandardName(
                        competency.AreaIdentifier.Value,
                        competency.AreaAsset.Value,
                        competency.AreaStandardType,
                        competency.AreaLabel,
                        competency.AreaCode,
                        classifications,
                        language,
                        useSimpleNameConvention
                    ),
                    Competencies = new List<CompetencyItem>()
                };

                areas.Add(area);
            }

            var competencyItem = new CompetencyItem
            {
                Identifier = competency.CompetencyIdentifier,
                Sequence = competency.CompetencySequence,
                Name = GetStandardName(
                    competency.CompetencyIdentifier,
                    competency.CompetencyAsset,
                    "Competency",
                    competency.CompetencyLabel,
                    competency.CompetencyCode,
                    classifications,
                    language,
                    useSimpleNameConvention
                ),
                Hours = hours,
                JournalItems = journalItems,
                SkillRating = skillRating,
                IncludeHoursToArea = includeHoursToArea,
                SatisfactionLevel = satisfactionLevel
            };

            area.Competencies.Add(competencyItem);
        }

        private static string GetStandardName(
            Guid standardIdentifier,
            int assetNumber,
            string standardType,
            string standardLabel,
            string standardCode,
            string[] classifications,
            string language,
            bool useSimpleNameConvention = false
            )
        {
            var classification = standardType;

            var content = ServiceLocator.ContentSearch.GetBlock(
                standardIdentifier,
                null,
                new[] { ContentLabel.Title }
            );

            var title = content.Title.Text.Get(language);
            var typeName = standardLabel.IfNullOrEmpty(standardType);
            var name = useSimpleNameConvention ? $"{title}" : $"{title} {typeName} Asset #{assetNumber}";

            return standardCode.IsNotEmpty()
                ? $"{standardCode}. {name}"
                : name;
        }
    }
}