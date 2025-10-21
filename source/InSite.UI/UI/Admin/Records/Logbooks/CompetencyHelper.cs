using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks
{
    public static class CompetencyHelper
    {
        public class CompetencyItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public decimal? Hours { get; set; }
            public int? JournalItems { get; set; }
            public int? SkillRating { get; set; }
        }

        public class AreaItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public List<CompetencyItem> Competencies { get; set; }
        }

        public static List<AreaItem> GetAreas(Guid journalSetupIdentifier, Guid frameworkIdentifier, bool includeRequirements)
        {
            var requirements = ServiceLocator.JournalSearch.GetCompetencyRequirements(journalSetupIdentifier);
            var standards = SelectAreasAndCompetencies(frameworkIdentifier);

            List<Standard> competencies;

            if (includeRequirements)
            {
                competencies = standards
                    .Where(x => string.Equals(x.StandardType, StandardType.Competency, StringComparison.OrdinalIgnoreCase)
                                && requirements.Any(y => y.CompetencyStandardIdentifier == x.StandardIdentifier)
                        )
                    .ToList();
            }
            else
            {
                competencies = standards
                    .Where(x => string.Equals(x.StandardType, StandardType.Competency, StringComparison.OrdinalIgnoreCase)
                                && !requirements.Any(y => y.CompetencyStandardIdentifier == x.StandardIdentifier)
                        )
                    .ToList();
            }

            if (competencies.Count == 0)
                return null;

            var areas = new List<AreaItem>();

            var classifications = StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            foreach (var competency in competencies)
                AddCompetency(areas, requirements, standards, classifications, competency);

            areas = areas.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();
            foreach (var area in areas)
                area.Competencies = area.Competencies.OrderBy(x => x.Sequence).ThenBy(x => x.Name).ToList();

            return areas;
        }

        private static List<Standard> SelectAreasAndCompetencies(Guid frameworkIdentifier)
        {
            var standards = StandardSearch.Select(new StandardFilter { ParentStandardIdentifier = frameworkIdentifier });

            if (standards.Count > 0)
            {
                var areaIds = standards.Select(x => x.StandardIdentifier).ToArray();
                var competencies = StandardSearch.Select(new StandardFilter { ParentStandardIdentifiers = areaIds });

                standards.AddRange(competencies);
            }

            return standards;
        }

        private static void AddCompetency(
            List<AreaItem> areas,
            List<QCompetencyRequirement> requirements,
            List<Standard> standards,
            string[] classifications,
            Standard competency
            )
        {
            var area = areas.Find(x => x.Identifier == competency.ParentStandardIdentifier);
            if (area == null)
            {
                var parentStandard = standards.Find(x => x.StandardIdentifier == competency.ParentStandardIdentifier);
                if (parentStandard == null)
                    return;

                area = new AreaItem
                {
                    Identifier = competency.ParentStandardIdentifier.Value,
                    Sequence = parentStandard.Sequence,
                    Name = GetStandardName(parentStandard),
                    Competencies = new List<CompetencyItem>()
                };

                areas.Add(area);
            }

            var requirement = requirements.Find(x => x.CompetencyStandardIdentifier == competency.StandardIdentifier);

            var competencyItem = new CompetencyItem
            {
                Identifier = competency.StandardIdentifier,
                Sequence = competency.Sequence,
                Name = GetStandardName(competency),
                Hours = requirement?.CompetencyHours,
                JournalItems = requirement?.JournalItems,
                SkillRating = requirement?.SkillRating
            };

            area.Competencies.Add(competencyItem);
        }

        public static string GetStandardName(Standard standard)
        {
            return GetStandardName(
                standard.StandardIdentifier,
                standard.AssetNumber,
                standard.StandardLabel,
                standard.Code,
                standard.StandardType
                );
        }

        public static string GetStandardName(
            Guid standardIdentifier,
            int assetNumber,
            string standardLabel,
            string standardCode,
            string classification
            )
        {
            var content = ServiceLocator.ContentSearch.GetBlock(
                standardIdentifier,
                Shift.Common.ContentContainer.DefaultLanguage,
                new[] { ContentLabel.Title }
            );

            var title = content.Title.Text.Default;
            var typeName = standardLabel.IfNullOrEmpty(classification);
            var name = $"{title} {typeName} Asset #{assetNumber}";

            return standardCode.IsNotEmpty()
                ? $"{standardCode}. {name}"
                : name;
        }
    }
}