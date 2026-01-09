using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using CalculationPartData = InSite.Domain.Records.CalculationPart;

namespace InSite.Common.Web
{
    public static class GradebookHelper
    {
        private class CalculationPart
        {
            public string ItemKey { get; set; }
            public decimal Score { get; set; }
        }

        private class GradeItemModel
        {
            public string Key { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Format { get; set; }
            public string Type { get; set; }
            public string Weighting { get; set; }
            public bool IncludeToReport { get; set; }
            public decimal? PassPercent { get; set; }
            public string Achievement { get; set; }
            public TriggerCauseChange? AchievementWhenChange { get; set; }
            public TriggerCauseGrade? AchievementWhenGrade { get; set; }
            public TriggerEffectCommand? AchievementThenCommand { get; set; }
            public TriggerEffectCommand? AchievementElseCommand { get; set; }
            public string AchievementFixedDate { get; set; }
            public string[] Standards { get; set; }

            public List<GradeItemModel> Children { get; set; }
            public CalculationPart[] Parts { get; set; }
        }

        private class Gradebook
        {
            public string Class { get; set; }
            public string Achievement { get; set; }
            public string Framework { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }

            public List<GradeItemModel> Items { get; set; }
        }

        public class GradebookCommands
        {
            public Guid GradebookIdentifier { get; set; }
            public string GradebookTitle { get; set; }
            public List<ICommand> Commands { get; set; }
            public List<string> Warnings { get; set; }
        }

        public class DeserializeException : Exception
        {
            public DeserializeException(string message)
                : base(message)
            {
            }
        }

        public static int CreateMissingScores(Guid gradebookIdentifier)
        {
            var scoreType = GradeItemType.Score.ToString();

            var scoreItems = ServiceLocator.RecordSearch
                .GetGradeItems(gradebookIdentifier)
                .Where(x => x.GradeItemType == scoreType)
                .ToList();

            var students = ServiceLocator.RecordSearch.GetEnrollments(new QEnrollmentFilter { GradebookIdentifier = gradebookIdentifier });
            var progresses = ServiceLocator.RecordSearch.GetGradebookScores(new QProgressFilter { GradebookIdentifier = gradebookIdentifier });

            var count = 0;

            foreach (var scoreItem in scoreItems)
            {
                var existing = progresses
                    .Where(x => x.GradeItemIdentifier == scoreItem.GradeItemIdentifier)
                    .Select(x => x.UserIdentifier)
                    .ToHashSet();

                foreach (var student in students)
                {
                    if (existing.Contains(student.LearnerIdentifier))
                        continue;

                    ServiceLocator.SendCommand(new AddProgress(UniqueIdentifier.Create(), gradebookIdentifier, scoreItem.GradeItemIdentifier, student.LearnerIdentifier));

                    count++;
                }
            }

            return count;
        }

        public static string GetScoreValue(QProgress score, string format, bool showPercentSign = true)
        {
            return Enum.TryParse<GradeItemFormat>(format, out var itemFormat)
                ? GetScoreValue(score, itemFormat, showPercentSign)
                : null;
        }

        public static string GetScoreValue(QProgress score, GradeItem item, bool showPercentSign = true)
            => GetScoreValue(score, item.Format, showPercentSign);

        private static string GetScoreValue(QProgress score, GradeItemFormat format, bool showPercentSign)
        {
            if (score == null)
                return null;

            if (format == GradeItemFormat.Boolean)
                return score.ProgressStatus;

            if (score.ProgressNumber.HasValue)
                return $"{score.ProgressNumber:n1}";

            if (score.ProgressText.IsNotEmpty())
                return score.ProgressText;

            if (score.ProgressPercent.HasValue)
                return showPercentSign ? $"{score.ProgressPercent:p1}" : $"{score.ProgressPercent * 100m:n1}";

            if (score.ProgressPoints.HasValue)
                return score.ProgressMaxPoints == null ? $"{score.ProgressPoints:n2}" : $"{score.ProgressPoints:n2} / {score.ProgressMaxPoints:n2}";

            return null;
        }

        public static byte[] Serialize(Guid gradebookIdentifier)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(gradebookIdentifier);
            var classTitle = gradebook.PrimaryEvent.HasValue ? ServiceLocator.EventSearch.GetEvent(gradebook.PrimaryEvent.Value)?.EventTitle : null;
            var achievementTitle = gradebook.Achievement.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(gradebook.Achievement.Value)?.AchievementTitle : null;
            var frameworkTitle = gradebook.Framework.HasValue ? StandardSearch.SelectFirst(x => x.StandardIdentifier == gradebook.Framework)?.ContentTitle : null;

            var serialized = new Gradebook
            {
                Class = classTitle,
                Achievement = achievementTitle,
                Framework = frameworkTitle,
                Title = gradebook.Name,
                Type = gradebook.Type.ToString(),
                Items = new List<GradeItemModel>()
            };

            int key = 0;
            var itemMap = gradebook.AllItems.ToDictionary(x => x.Identifier, x => (++key).ToString());

            AddItems(gradebookIdentifier, gradebook.RootItems, serialized.Items, itemMap);

            var json = JsonConvert.SerializeObject(serialized);

            return Encoding.UTF8.GetBytes(json);
        }

        private static void AddItems(Guid gradebookIdentifier, List<GradeItem> srcList, List<GradeItemModel> dstList, Dictionary<Guid, string> itemMap)
        {
            foreach (var src in srcList)
            {
                var achievementTitle = src.Achievement != null ? ServiceLocator.AchievementSearch.GetAchievement(src.Achievement.Achievement)?.AchievementTitle : null;

                var dst = new GradeItemModel
                {
                    Key = itemMap[src.Identifier],
                    Code = src.Code,
                    Name = src.Name,
                    ShortName = src.ShortName,
                    Format = src.Format.ToString(),
                    Type = src.Type.ToString(),
                    Weighting = src.Weighting.ToString(),
                    IncludeToReport = src.IsReported,
                    PassPercent = src.PassPercent,
                    Achievement = achievementTitle,
                    AchievementWhenChange = src.Achievement?.WhenChange,
                    AchievementWhenGrade = src.Achievement?.WhenGrade,
                    AchievementThenCommand = src.Achievement?.ThenCommand,
                    AchievementElseCommand = src.Achievement?.ElseCommand,
                    AchievementFixedDate = src.Achievement?.AchievementFixedDate != null ? $"{src.Achievement.AchievementFixedDate:yyyy-MM-dd}" : null,
                    Children = src.Children.IsNotEmpty() ? new List<GradeItemModel>() : null,
                    Parts = src.Parts.IsNotEmpty()
                        ? src.Parts.Select(x => new CalculationPart { ItemKey = itemMap[x.Item], Score = x.Score }).ToArray()
                        : null
                };

                if (src.Competencies.IsNotEmpty())
                {
                    var standards = new List<string>();
                    foreach (var standardIdentifier in src.Competencies)
                    {
                        var standard = StandardSearch.SelectFirst(x => x.StandardIdentifier == standardIdentifier);
                        if (standard != null && !string.IsNullOrEmpty(standard.ContentTitle))
                            standards.Add(standard.ContentTitle);
                    }
                    dst.Standards = standards.ToArray();
                }

                if (src.Children.IsNotEmpty())
                    AddItems(gradebookIdentifier, src.Children, dst.Children, itemMap);

                dstList.Add(dst);
            }
        }

        public static GradebookCommands Deserialize(Guid organization, string json)
        {
            var gradebook = JsonConvert.DeserializeObject<Gradebook>(json);
            var commands = new List<ICommand>();
            var partCommands = new List<ICommand>();
            var warnings = new HashSet<string>();
            var gradebookIdentifier = UniqueIdentifier.Create();
            var standards = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            AddGradebook(organization, gradebookIdentifier, gradebook, commands, standards, warnings);

            if (gradebook.Items.IsNotEmpty())
            {
                var itemMap = new Dictionary<string, Guid>();

                AddGradebookItems(organization, gradebookIdentifier, null, gradebook.Items, commands, partCommands, standards, itemMap, warnings);
                commands.AddRange(partCommands);
            }

            return new GradebookCommands
            {
                GradebookIdentifier = gradebookIdentifier,
                GradebookTitle = gradebook.Title,
                Commands = commands,
                Warnings = warnings.ToList().OrderBy(x => x).ToList()
            };
        }

        private static void AddGradebook(
            Guid organization,
            Guid gradebookIdentifier,
            Gradebook gradebook,
            List<ICommand> commands,
            Dictionary<string, Guid> standards,
            HashSet<string> warnings
            )
        {
            var gradebookType = gradebook.Type.ToEnum<GradebookType>();
            var @class = GetClass(organization, gradebook.Class, warnings);
            var achievement = GetAchievement(organization, gradebook.Achievement, warnings);
            var framework = GetFramework(organization, gradebook.Framework, warnings);

            commands.Add(new CreateGradebook(gradebookIdentifier, organization, gradebook.Title, gradebookType, @class, achievement, framework));

            if (framework.HasValue)
                GetFrameworkStandards(framework.Value, standards);
        }

        private static void AddGradebookItems(
            Guid organization,
            Guid gradebookIdentifier,
            Guid? parentItemKey,
            List<GradeItemModel> items,
            List<ICommand> commands,
            List<ICommand> partCommands,
            Dictionary<string, Guid> standards,
            Dictionary<string, Guid> itemMap,
            HashSet<string> warnings
            )
        {
            foreach (var item in items)
            {
                var itemType = item.Type.ToEnum<GradeItemType>();
                var achievementIdentifier = GetAchievement(organization, item.Achievement, warnings);

                var format = itemType == GradeItemType.Score
                    ? item.Format.ToEnum<GradeItemFormat>()
                    : GradeItemFormat.None;

                var weighting = itemType != GradeItemType.Score
                    ? item.Weighting.ToEnum<GradeItemWeighting>()
                    : GradeItemWeighting.None;

                var identifier = GetItemIdentifier(item.Key, itemMap);

                commands.Add(new AddGradeItem(
                    gradebookIdentifier,
                    identifier,
                    item.Key.ToString(),
                    item.Name,
                    item.ShortName,
                    item.IncludeToReport,
                    format,
                    itemType,
                    weighting,
                    item.PassPercent,
                    parentItemKey
                    ));

                if (item.Parts.IsNotEmpty())
                {
                    var parts = item.Parts.Select(x => new CalculationPartData { Item = GetItemIdentifier(x.ItemKey, itemMap), Score = x.Score }).ToArray();
                    partCommands.Add(new ChangeGradeItemCalculation(gradebookIdentifier, identifier, parts));
                }

                if (item.PassPercent.HasValue)
                    commands.Add(new ChangeGradeItemPassPercent(gradebookIdentifier, identifier, item.PassPercent));

                if (achievementIdentifier != null)
                {
                    var achievement = new GradeItemAchievement
                    {
                        WhenChange = item.AchievementWhenChange.Value,
                        WhenGrade = item.AchievementWhenGrade.Value,
                        ThenCommand = item.AchievementThenCommand.Value,
                        ElseCommand = item.AchievementElseCommand.Value,
                        Achievement = achievementIdentifier.Value,
                        AchievementFixedDate = DateTimeOffset.Parse(item.AchievementFixedDate)
                    };

                    commands.Add(new ChangeGradeItemAchievement(gradebookIdentifier, identifier, achievement));
                }

                if (item.Standards.IsNotEmpty())
                {
                    var itemStandards = new List<Guid>();
                    foreach (var title in item.Standards)
                    {
                        if (standards.TryGetValue(title, out var standard))
                            itemStandards.Add(standard);
                        else
                            warnings.Add($"Standard is not found: <b>{title}</b>");
                    }

                    if (standards.Count > 0)
                        commands.Add(new ChangeGradeItemCompetencies(gradebookIdentifier, identifier, itemStandards.ToArray()));
                }

                if (item.Children.IsNotEmpty())
                    AddGradebookItems(organization, gradebookIdentifier, identifier, item.Children, commands, partCommands, standards, itemMap, warnings);
            }
        }

        private static Guid GetItemIdentifier(string key, Dictionary<string, Guid> itemMap)
        {
            if (itemMap.TryGetValue(key, out var identifier))
                return identifier;

            identifier = UniqueIdentifier.Create();

            itemMap.Add(key, identifier);

            return identifier;
        }

        private static Guid? GetClass(Guid organization, string title, HashSet<string> warnings)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            var @event = ServiceLocator.EventSearch.GetEvent(organization, title);

            if (@event != null)
                return @event.EventIdentifier;

            warnings.Add($"Class is not found: <b>{title}</b>");

            return null;
        }

        private static Guid? GetAchievement(Guid organization, string title, HashSet<string> warnings)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(organization, title);

            if (achievement != null)
                return achievement.AchievementIdentifier;

            warnings.Add($"Achievement is not found: <b>{title}</b>");

            return null;
        }

        private static Guid? GetFramework(Guid organization, string title, HashSet<string> warnings)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            var standard = StandardSearch.SelectFirst(x =>
                x.Organization.OrganizationIdentifier == organization
                && x.StandardType == StandardType.Framework
                && x.ContentTitle == title
            );

            if (standard != null)
                return standard.StandardIdentifier;

            warnings.Add($"Framework is not found: <b>{title}</b>");

            return null;
        }

        private static void GetFrameworkStandards(Guid parentIdentifier, Dictionary<string, Guid> standards)
        {
            var children = StandardSearch.Select(x => x.ParentStandardIdentifier == parentIdentifier);
            foreach (var child in children)
            {
                if (!string.IsNullOrEmpty(child.ContentTitle) && !standards.ContainsKey(child.ContentTitle))
                    standards.Add(child.ContentTitle, child.StandardIdentifier);

                GetFrameworkStandards(child.StandardIdentifier, standards);
            }
        }
    }
}