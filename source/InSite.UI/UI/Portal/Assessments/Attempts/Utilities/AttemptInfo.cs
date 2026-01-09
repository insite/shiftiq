using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AttemptInfo
    {
        public BankState Bank => _bank;
        public Form BankForm => _bankForm;

        public QAttempt Attempt => _attempt;

        public BankState _bank;
        private Form _bankForm;
        private QAttempt _attempt;
        private GlossaryHelper _glossary;

        private List<QAttemptSection> _sections;
        private List<QAttemptQuestion> _questions;
        private List<QAttemptOption> _options;
        private List<QAttemptMatch> _matches;
        private List<QAttemptPin> _pins;
        private List<QAttemptSolution> _solutions;

        public AttemptInfo(Form bankForm, QAttempt attempt, GlossaryHelper glossary)
        {
            _bank = bankForm.Specification.Bank;
            _bankForm = bankForm;
            _attempt = attempt;
            _glossary = glossary;
        }

        public List<QAttemptSection> GetAllSections()
        {
            return _sections ?? (_sections = ServiceLocator.AttemptSearch.GetAttemptSections(_attempt.AttemptIdentifier));
        }

        public List<QAttemptQuestion> GetAllQuestions()
        {
            return _questions ?? (_questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(_attempt.AttemptIdentifier));
        }

        public List<QAttemptOption> GetAllOptions()
        {
            return _options ?? (_options = ServiceLocator.AttemptSearch.GetAttemptOptions(_attempt.AttemptIdentifier));
        }

        public List<QAttemptMatch> GetAllMatches()
        {
            return _matches ?? (_matches = ServiceLocator.AttemptSearch.GetAttemptMatches(_attempt.AttemptIdentifier, null));
        }

        public List<QAttemptPin> GetAllPins()
        {
            return _pins ?? (_pins = ServiceLocator.AttemptSearch.GetAttemptPins(_attempt.AttemptIdentifier, null, null));
        }

        public List<QAttemptSolution> GetAllSolutions()
        {
            return _solutions ?? (_solutions = ServiceLocator.AttemptSearch.GetAttemptSolutions(_attempt.AttemptIdentifier, null));
        }


        public IEnumerable<AttemptSectionInfo> GetSections()
        {
            if (_bankForm.Specification.Type == SpecificationType.Dynamic)
                return null;

            var result = new List<AttemptSectionInfo>();
            var sections = GetAllSections();
            var questions = GetQuestions();

            if (sections.IsEmpty())
            {
                var sectionMapping = _bankForm.Sections.SelectMany(x => x.Fields).ToDictionary(x => x.QuestionIdentifier, x => x.Section);
                var prevSectionId = Guid.Empty;
                AttemptSectionInfo sectionItem = null;

                foreach (var question in questions)
                {
                    var section = sectionMapping.GetOrDefault(question.QuestionIdentifier);
                    var sectionFound = section != null;
                    var sectionId = sectionFound ? section.Identifier : Guid.Empty;

                    if (sectionItem == null || prevSectionId != sectionId)
                    {
                        result.Add(sectionItem = new AttemptSectionInfo { BankSection = section });
                        prevSectionId = sectionId;
                    }

                    sectionItem.Questions.Add(question);
                }
            }
            else
            {
                foreach (var section in sections)
                {
                    var item = new AttemptSectionInfo { AttemptSection = section };

                    if (section.SectionIdentifier.HasValue)
                    {
                        var sectionId = section.SectionIdentifier.Value;
                        item.BankSection = _bankForm.Sections.FirstOrDefault(y => y.Identifier == sectionId);
                    }
                    else if (section.SectionIndex < _bankForm.Sections.Count)
                    {
                        item.BankSection = _bankForm.Sections[section.SectionIndex];
                    }

                    foreach (var question in questions)
                    {
                        if (question.SectionIndex.Value == section.SectionIndex)
                            item.Questions.Add(question);
                    }

                    result.Add(item);
                }
            }

            return result;
        }

        public IEnumerable<QAttemptQuestion> GetQuestions()
        {
            return GetAllQuestions().Where(x => !x.ParentQuestionIdentifier.HasValue);
        }

        public IEnumerable<QAttemptQuestion> GetSubQuestions(Guid questionId)
        {
            return GetAllQuestions().Where(x => x.ParentQuestionIdentifier == questionId);
        }

        public IEnumerable<QAttemptOption> GetQuestionOptions(Guid questionId)
        {
            return GetAllOptions().Where(x => x.QuestionIdentifier == questionId);
        }

        public IEnumerable<QAttemptMatch> GetQuestionMatches(Guid questionId)
        {
            return GetAllMatches().Where(x => x.QuestionIdentifier == questionId);
        }

        public IEnumerable<QAttemptPin> GetQuestionPins(Guid questionId)
        {
            return GetAllPins().Where(x => x.QuestionIdentifier == questionId);
        }

        public string GetHtml(QAttemptQuestion question, string text)
        {
            if (text.IsEmpty())
                return string.Empty;

            var result = question != null
                ? _glossary.Process(question.QuestionIdentifier, "Title", text)
                : text;

            return Markdown.ToHtml(result);
        }
    }
}