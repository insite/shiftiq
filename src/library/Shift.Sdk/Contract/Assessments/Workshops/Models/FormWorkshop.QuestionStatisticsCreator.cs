using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Common;

namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public static class QuestionStatisticsCreator
        {
            public static QuestionStatistics Create(IEnumerable<Question> questions, Dictionary<Guid, string> bankStandardCodes, bool includeSubCompetencies)
            {
                var questionPerTagAndTaxonomy = CountQuestionPerTagAndTaxonomy(questions);
                var questionPerTaxonomy = CountQuestionPerTaxonomy(questions);
                var taxonomies = LoadTaxonomies(questions);

                return new QuestionStatistics
                {
                    QuestionPerTagAndTaxonomyArray = questionPerTagAndTaxonomy,
                    QuestionPerTaxonomyArray = questionPerTaxonomy,
                    QuestionPerDifficultyArray = CountQuestionPerDifficulty(questions),
                    QuestionPerGACArray = CountQuestionPerGAC(questions, bankStandardCodes),
                    QuestionPerCodeArray = CountQuestionPerCode(questions),
                    QuestionPerLIGArray = CountQuestionPerLIG(questions),
                    Taxonomies = taxonomies,
                    Standards = LoadStandards(questions, bankStandardCodes, taxonomies),
                    SubCompetencies = includeSubCompetencies ? LoadSubCompetencies(questions, bankStandardCodes) : new QuestionStatistics.SubCompetency[0],
                    TagAndTaxonomyArray = CreateTagAndTaxonomyArray(questionPerTagAndTaxonomy, questionPerTaxonomy)
                };
            }

            private static QuestionStatistics.QuestionPerTagAndTaxonomy[] CountQuestionPerTagAndTaxonomy(IEnumerable<Question> questions)
            {
                var tags = new SortedDictionary<string, int>();

                foreach (var question in questions)
                {
                    var tagTax = $"{question.Classification.Tag ?? "None"}|{question.Classification.Taxonomy ?? 0}";
                    if (!tags.ContainsKey(tagTax))
                        tags.Add(tagTax, 0);
                    tags[tagTax]++;
                }

                var dt = new List<QuestionStatistics.QuestionPerTagAndTaxonomy>();
                foreach (var key in tags.Keys)
                {
                    var row = new QuestionStatistics.QuestionPerTagAndTaxonomy
                    {
                        Tag = key.Split('|')[0],
                        Taxonomy = int.Parse(key.Split('|')[1]),
                        Count = tags[key]
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static QuestionStatistics.QuestionPerIntItem[] CountQuestionPerTaxonomy(IEnumerable<Question> questions)
            {
                var taxonomies = new SortedDictionary<int, int>();

                foreach (var question in questions)
                {
                    var taxonomy = question.Classification.Taxonomy ?? 0;
                    if (!taxonomies.ContainsKey(taxonomy))
                        taxonomies.Add(taxonomy, 0);
                    taxonomies[taxonomy]++;
                }

                var dt = new List<QuestionStatistics.QuestionPerIntItem>();
                foreach (var key in taxonomies.Keys)
                {
                    var row = new QuestionStatistics.QuestionPerIntItem
                    {
                        Item = key,
                        Count = taxonomies[key]
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static QuestionStatistics.QuestionPerIntItem[] CountQuestionPerDifficulty(IEnumerable<Question> questions)
            {
                var difficulties = new SortedDictionary<int, int>();

                foreach (var question in questions)
                {
                    var difficulty = question.Classification.Difficulty ?? 0;
                    if (!difficulties.ContainsKey(difficulty))
                        difficulties.Add(difficulty, 0);
                    difficulties[difficulty]++;
                }

                var dt = new List<QuestionStatistics.QuestionPerIntItem>();
                foreach (var key in difficulties.Keys)
                {
                    var row = new QuestionStatistics.QuestionPerIntItem
                    {
                        Item = key,
                        Count = difficulties[key]
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static QuestionStatistics.QuestionPerStringItem[] CountQuestionPerGAC(IEnumerable<Question> questions, Dictionary<Guid, string> bankStandardCodes)
            {
                var standards = new SortedDictionary<string, int>();

                foreach (var question in questions)
                {
                    var gac = "None";
                    if (bankStandardCodes.TryGetValue(question.Set.Standard, out var code) && !string.IsNullOrEmpty(code))
                        gac = code;

                    if (standards.ContainsKey(gac))
                        standards[gac] += 1;
                    else
                        standards.Add(gac, 1);
                }

                var dt = new List<QuestionStatistics.QuestionPerStringItem>();
                foreach (var key in standards.Keys)
                {
                    var row = new QuestionStatistics.QuestionPerStringItem
                    {
                        Item = key,
                        Count = standards[key]
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static QuestionStatistics.QuestionPerStringItem[] CountQuestionPerCode(IEnumerable<Question> questions)
            {
                var dataSource = questions
                    .Select(x => string.IsNullOrEmpty(x.Classification.Code) ? "None" : x.Classification.Code)
                    .GroupBy(x => x)
                    .Select(x => (Code: x.Key, Count: x.Count()))
                    .OrderBy(x => x.Code == "None" ? 0 : 1)
                    .ThenBy(x => x.Code)
                    .ToArray();

                var dt = new List<QuestionStatistics.QuestionPerStringItem>();
                foreach (var item in dataSource)
                {
                    var row = new QuestionStatistics.QuestionPerStringItem
                    {
                        Item = item.Code,
                        Count = item.Count
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static QuestionStatistics.QuestionPerStringItem[] CountQuestionPerLIG(IEnumerable<Question> questions)
            {
                var groups = new SortedDictionary<string, int>();

                foreach (var question in questions)
                {
                    var group = question.Classification.LikeItemGroup ?? "None";
                    if (!groups.ContainsKey(group))
                        groups.Add(group, 0);
                    groups[group]++;
                }

                var dt = new List<QuestionStatistics.QuestionPerStringItem>();
                foreach (var key in groups.Keys)
                {
                    var row = new QuestionStatistics.QuestionPerStringItem
                    {
                        Item = key,
                        Count = groups[key]
                    };
                    dt.Add(row);
                }

                return dt.ToArray();
            }

            private static int[] LoadTaxonomies(IEnumerable<Question> questions)
            {
                var taxonomiesIndex = new HashSet<int>();

                foreach (var question in questions)
                {
                    var taxonomy = question.Classification.Taxonomy ?? 0;
                    if (!taxonomiesIndex.Contains(taxonomy))
                        taxonomiesIndex.Add(taxonomy);
                }

                return taxonomiesIndex
                    .OrderBy(x => x)
                    .ToArray();
            }

            private static QuestionStatistics.AssessmentStandard[] LoadStandards(
                IEnumerable<Question> questions,
                Dictionary<Guid, string> bankStandardCodes,
                int[] taxonomies
                )
            {
                var groupedQuestions = new Dictionary<MultiKey<Guid, Guid>, List<Question>>();

                foreach (var question in questions)
                {
                    var key = new MultiKey<Guid, Guid>(question.Set.Standard, question.Standard);
                    if (!groupedQuestions.ContainsKey(key))
                        groupedQuestions.Add(key, new List<Question>());

                    groupedQuestions[key].Add(question);
                }

                var standards = groupedQuestions.Select(standardItem =>
                    {
                        var questionTaxonomies = standardItem.Value.GroupBy(x => x.Classification.Taxonomy ?? 0).ToDictionary(x => x.Key, x => x.Count());

                        return new QuestionStatistics.AssessmentStandard
                        {
                            SetStandardCode = bankStandardCodes.TryGetValue(standardItem.Key.Key1, out var setStandardCode) ? setStandardCode : null,
                            QuestionStandardCode = bankStandardCodes.TryGetValue(standardItem.Key.Key2, out var questionStandardCode) ? questionStandardCode : null,
                            Questions = standardItem.Value.Count,
                            Taxonomies = taxonomies.Select(x => questionTaxonomies.ContainsKey(x) ? questionTaxonomies[x] : (int?)null).ToArray()
                        };
                    })
                    .OrderBy(x => x.SetStandardCode)
                    .ThenBy(x => x.QuestionStandardCode)
                    .ThenBy(x => x.Questions)
                    .ToArray();

                return standards;
            }

            private static QuestionStatistics.SubCompetency[] LoadSubCompetencies(IEnumerable<Question> questions, Dictionary<Guid, string> bankStandardCodes)
            {
                var subCompetencies = new Dictionary<MultiKey<Guid, Guid, Guid>, int>();

                foreach (var question in questions)
                {
                    if (question.SubStandards == null)
                        continue;

                    foreach (var sub in question.SubStandards)
                    {
                        var key = new MultiKey<Guid, Guid, Guid>(question.Set.Standard, question.Standard, sub);

                        subCompetencies.TryGetValue(key, out var count);

                        subCompetencies[key] = count + 1;
                    }
                }

                var result = subCompetencies.Select(standardItem =>
                    new QuestionStatistics.SubCompetency
                    {
                        SetStandardCode = bankStandardCodes.TryGetValue(standardItem.Key.Key1, out var setStadardCode) ? setStadardCode : null,
                        QuestionStandardCode = bankStandardCodes.TryGetValue(standardItem.Key.Key2, out var questionStandardCode) ? questionStandardCode : null,
                        QuestionSubCode = bankStandardCodes.TryGetValue(standardItem.Key.Key3, out var questionSubCode) ? questionSubCode : null,
                        Questions = standardItem.Value
                    })
                    .OrderBy(x => x.SetStandardCode)
                    .ThenBy(x => x.QuestionStandardCode)
                    .ThenBy(x => x.QuestionSubCode)
                    .ThenBy(x => x.Questions)
                    .ToArray();

                return result;
            }

            private static QuestionStatistics.TagAndTaxonomy[] CreateTagAndTaxonomyArray(
                QuestionStatistics.QuestionPerTagAndTaxonomy[] questionPerTagAndTaxonomy,
                QuestionStatistics.QuestionPerIntItem[] questionPerTaxonomy)
            {
                var taxonomyMap = new Dictionary<int, int>();
                for (var i = 0; i < questionPerTaxonomy.Length; i++)
                    taxonomyMap.Add(questionPerTaxonomy[i].Item, taxonomyMap.Count);

                var list = new List<QuestionStatistics.TagAndTaxonomy>();
                for (var i = 0; i < questionPerTagAndTaxonomy.Length; i++)
                {
                    var tag = questionPerTagAndTaxonomy[i].Tag;
                    var taxonomy = questionPerTagAndTaxonomy[i].Taxonomy;
                    var count = questionPerTagAndTaxonomy[i].Count;
                    var item = list.Count > 0 ? list[list.Count - 1] : null;

                    if (item == null || !string.Equals(item.Tag, tag, StringComparison.OrdinalIgnoreCase))
                        list.Add(item = new QuestionStatistics.TagAndTaxonomy { Tag = tag, CountPerTaxonomy = new int[taxonomyMap.Count] });

                    var taxonomyIndex = taxonomyMap[taxonomy];

                    item.CountPerTaxonomy[taxonomyIndex] = count;
                }

                return list.ToArray();
            }
        }
    }
}
