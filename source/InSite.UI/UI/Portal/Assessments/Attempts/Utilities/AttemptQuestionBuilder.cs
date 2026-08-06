using System;
using System.Collections.Generic;

using InSite.Admin.Assessments.Sections.Models;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    internal class AttemptQuestionBuilder
    {
        private static readonly Random _random = new Random();

        private readonly Form _bankForm;
        private readonly bool _allowRandomization;
        private readonly string _language;
        private readonly bool _isStaticSpec;

        private readonly List<Question> _questions = new List<Question>();
        private readonly Dictionary<Guid, (Set Set, List<int> Indexes)> _setMapping = new Dictionary<Guid, (Set Set, List<int> Indexes)>();
        private readonly Dictionary<Guid, (Section Section, List<Question> Questions)> _sectionMapping = new Dictionary<Guid, (Section Section, List<Question> Questions)>();
        private readonly Dictionary<Guid, int> _sectionIndexMapping = new Dictionary<Guid, int>();
        private readonly Dictionary<Guid, int> _criterionIndexMapping = new Dictionary<Guid, int>();

        private AttemptQuestionBuilder(Form bankForm, bool allowRandomization, string language)
        {
            _bankForm = bankForm;
            _allowRandomization = allowRandomization;
            _language = language;
            _isStaticSpec = bankForm.Specification.Type == SpecificationType.Static;
        }

        public static AttemptQuestion[] Build(Form bankForm, bool allowRandomization, string language)
        {
            var instance = new AttemptQuestionBuilder(bankForm, allowRandomization, language);

            return instance.Build();
        }

        private AttemptQuestion[] Build()
        {
            if (_isStaticSpec)
                CollectStaticQuestions();
            else if (_bankForm.Specification.Criteria.Count > 0)
                CollectDynamicQuestions();
            else
                CollectBankQuestions();

            var order = GetDisplayOrder();
            var hidden = GetHiddenQuestions();
            var displayCount = _bankForm.Specification.QuestionLimit > 0
                            && _bankForm.Specification.QuestionLimit < _questions.Count
                ? _bankForm.Specification.QuestionLimit
                : _questions.Count;
            var sectionMapping = _isStaticSpec ? _sectionIndexMapping : _criterionIndexMapping;

            var result = new List<AttemptQuestion>();

            for (var i = 0; i < _questions.Count && result.Count < displayCount; i++)
            {
                var index = order[i] ?? i;
                var question = _questions[index];

                if (hidden.Contains(question.Identifier))
                    continue;

                var attemptQuestion = AttemptStarter.CreateQuestion(question, _allowRandomization, _language);

                if (sectionMapping.TryGetValue(attemptQuestion.Identifier, out var sectionIndex))
                    attemptQuestion.Section = sectionIndex;

                result.Add(attemptQuestion);
            }

            return result.ToArray();
        }

        private void CollectStaticQuestions()
        {
            for (var sectionIndex = 0; sectionIndex < _bankForm.Sections.Count; sectionIndex++)
            {
                var section = _bankForm.Sections[sectionIndex];

                foreach (var field in section.Fields)
                {
                    var index = _questions.Count;
                    var question = field.Question;
                    var set = question.Set;

                    if (!_sectionMapping.ContainsKey(section.Identifier))
                        _sectionMapping.Add(section.Identifier, (section, new List<Question>()));

                    if (!_setMapping.ContainsKey(set.Identifier))
                        _setMapping.Add(set.Identifier, (set, new List<int>()));

                    _questions.Add(question);
                    _sectionMapping[section.Identifier].Questions.Add(question);
                    _sectionIndexMapping[question.Identifier] = sectionIndex;
                    _setMapping[set.Identifier].Indexes.Add(index);
                }
            }
        }

        private void CollectDynamicQuestions()
        {
            var questionFilter = new QuestionFilterHelper(_bankForm.Specification.Criteria, null, false);

            foreach (var group in questionFilter.GetResult())
            {
                var criterionIndex = group.Criterion.Sequence - 1;

                foreach (var question in group.Questions)
                {
                    _criterionIndexMapping[question.Identifier] = criterionIndex;
                    _questions.Add(question);
                }
            }
        }

        private void CollectBankQuestions()
        {
            foreach (var set in _bankForm.Specification.Bank.Sets)
            {
                if (!_setMapping.ContainsKey(set.Identifier))
                    _setMapping.Add(set.Identifier, (set, new List<int>()));

                foreach (var question in set.Questions)
                {
                    var index = _questions.Count;
                    _questions.Add(question);
                    _setMapping[set.Identifier].Indexes.Add(index);
                }
            }
        }

        private int?[] GetDisplayOrder()
        {
            var order = new int?[_questions.Count];
            if (!_allowRandomization)
                return order;

            foreach (var (set, indexes) in _setMapping.Values)
            {
                if (!set.Randomization.Enabled)
                    continue;

                var count = set.Randomization.Count <= 0 || set.Randomization.Count > indexes.Count
                    ? indexes.Count
                    : set.Randomization.Count;

                while (count > 0)
                {
                    var randomIndex = _random.Next(count--);
                    var index1 = indexes[randomIndex];
                    var index2 = indexes[count];

                    var buffer = order[index1] ?? index1;
                    order[index1] = order[index2] ?? index2;
                    order[index2] = buffer;
                }
            }

            return order;
        }

        private HashSet<Guid> GetHiddenQuestions()
        {
            var hidden = new HashSet<Guid>();

            foreach (var (section, questions) in _sectionMapping.Values)
            {
                var criterion = section.Criterion;
                if (criterion.TagFilter.IsEmpty())
                    continue;

                var filter = QuestionDisplayFilter.Parse(criterion.TagFilter);

                var sectionQuestions = questions.ToArray();
                sectionQuestions.Shuffle();

                foreach (var question in sectionQuestions)
                {
                    var tag = question.Classification.Tag != null ? filter[question.Classification.Tag] : null;

                    if (tag != null && tag.Allows)
                        tag.Increment();
                    else
                        hidden.Add(question.Identifier);
                }
            }

            return hidden;
        }
    }
}
