using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sections.Models
{
    public class QuestionFilterHelper
    {
        private readonly IEnumerable<Criterion> _criteria;
        private readonly bool _mutuallyExclusiveLigs;
        private readonly HashSet<Guid> _questionsToExclude;
        private readonly HashSet<string> _ligsToExclude;
        private readonly HashSet<string> _statuses;

        public QuestionFilterHelper(IEnumerable<Criterion> criteria, IEnumerable<string> statuses, bool mutuallyExclusiveLigs)
        {
            _criteria = criteria;
            _mutuallyExclusiveLigs = mutuallyExclusiveLigs;

            if (_mutuallyExclusiveLigs)
            {
                _questionsToExclude = new HashSet<Guid>();

                var fieldQuery = _criteria.Select(sieve => sieve.Specification).Distinct().SelectMany(
                    spec => spec.EnumerateAllForms().SelectMany(
                        form => form.Sections.SelectMany(
                            section => section.Fields)));

                foreach (var field in fieldQuery)
                {
                    if (!_questionsToExclude.Contains(field.QuestionIdentifier))
                        _questionsToExclude.Add(field.QuestionIdentifier);
                }

                _ligsToExclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (statuses != null)
                _statuses = new HashSet<string>(statuses.Distinct().Select(x => string.IsNullOrEmpty(x) ? string.Empty : x));
        }

        public static List<Question> GetAvailableQuestions(Criterion criterion)
        {
            // Get all non-archived questions of the latest version (i.e. with max AssetVersion)

            var list = new List<Question>();

            foreach (var set in criterion.Sets)
            {
                if (set.Questions.Count == 0)
                    continue;

                if (set.Randomization.Enabled)
                {
                    var shuffleCount = set.Randomization.Count < set.Questions.Count ? set.Randomization.Count : set.Questions.Count;

                    if (shuffleCount <= 0)
                        shuffleCount = set.Questions.Count;

                    set.Questions.Shuffle(0, shuffleCount);
                }

                foreach (var question in set.Questions)
                    if (question.PublicationStatus != PublicationStatus.Archived)
                        list.Add(question);
            }

            return list;
        }

        public static List<Question> GetFilteredQuestions(Criterion criterion)
        {
            var filterHelper = new QuestionFilterHelper(new List<Criterion> { criterion }, null, false);
            return filterHelper.GetResult()[0].Item2;
        }

        public List<Tuple<Criterion, List<Question>>> GetResult()
        {
            var list = new List<Tuple<Criterion, List<Question>>>();

            foreach (var sieve in _criteria)
            {
                var questions = GetResultQuestions(sieve);

                var result = new Tuple<Criterion, List<Question>>(sieve, questions);

                list.Add(result);
            }

            if (_ligsToExclude != null)
                _ligsToExclude.Clear();

            return list;
        }

        private List<Question> GetResultQuestions(Criterion criterion)
        {
            var availableQuestions = GetAvailableQuestions(criterion);
            var satisfies = new List<Question>();

            switch (criterion.FilterType)
            {
                case CriterionFilterType.All:
                    {
                        for (var i = 0; i < availableQuestions.Count; i++)
                        {
                            if (criterion.QuestionLimit == 0 || i < criterion.QuestionLimit)
                            {
                                var question = availableQuestions[i];
                                TryAdd(satisfies, question);
                            }
                        }
                        break;
                    }

                case CriterionFilterType.Tag:
                    {
                        var filter = QuestionDisplayFilter.Parse(criterion.TagFilter);

                        foreach (var question in availableQuestions)
                        {
                            var tag = filter[question.Classification.Tag];
                            if (tag != null && tag.Allows && TryAdd(satisfies, question))
                                tag.Increment();
                        }
                        break;
                    }

                case CriterionFilterType.Pivot:
                    {
                        AddPivotQuestions(criterion, availableQuestions, satisfies);
                        break;
                    }
            }

            // Randomized questions should not be sorted!
            // satisfies.Sort((x1, x2) => x1.BankIndex.CompareTo(x2.BankIndex));

            return satisfies;
        }

        private void AddPivotQuestions(Criterion criterion, List<Question> availableQuestions, List<Question> satisfies)
        {
            var groups = PivotTableFilter.ApplyFilter(criterion.Sets, availableQuestions, criterion.PivotFilter);

            foreach (var group in groups)
            {
                IEnumerable<Question> groupQuestions;

                if (_mutuallyExclusiveLigs)
                {
                    var hasLig = group.Item2.Where(x => x.Classification.LikeItemGroup != null).ToArray();
                    hasLig.Shuffle();

                    var noLig = group.Item2.Where(x => x.Classification.LikeItemGroup == null).ToArray();
                    noLig.Shuffle();

                    groupQuestions = hasLig.Concat(noLig);
                }
                else
                {
                    var allQuestions = group.Item2.ToArray();
                    allQuestions.Shuffle();

                    groupQuestions = allQuestions;
                }

                var questionsCount = group.Item1;

                foreach (var question in groupQuestions)
                {
                    if (TryAdd(satisfies, question))
                        questionsCount--;

                    if (questionsCount == 0)
                        break;
                }
            }
        }

        private bool TryAdd(List<Question> satisfies, Question question)
        {
            if (_statuses != null && !_statuses.Contains(question.Condition ?? string.Empty))
                return false;

            if (_mutuallyExclusiveLigs)
            {
                if (_questionsToExclude.Contains(question.Identifier))
                    return false;

                var hasLig = question.Classification.LikeItemGroup != null;
                if (hasLig && _ligsToExclude.Contains(question.Classification.LikeItemGroup))
                    return false;

                _questionsToExclude.Add(question.Identifier);

                if (hasLig)
                    _ligsToExclude.Add(question.Classification.LikeItemGroup);
            }

            satisfies.Add(question);

            return true;
        }
    }
}