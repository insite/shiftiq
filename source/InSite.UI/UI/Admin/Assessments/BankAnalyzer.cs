using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;

namespace InSite.Admin.Assessments
{
    public class BankAnalyzer
    {
        private Dictionary<Guid, Dictionary<Guid, int>> _questionIndexes = new Dictionary<Guid, Dictionary<Guid, int>>();

        public BankAnalyzer(BankState bank)
        {
            foreach(var spec in bank.Specifications)
                foreach(var form in spec.EnumerateAllForms())
                    GetQuestionIndexes(form);
        }

        public BankAnalyzer(Form form)
        {
            GetQuestionIndexes(form);
        }

        public int GetQuestionIndex(Question question, Form form)
        {
            var indexes = GetQuestionIndexes(form);
            if (indexes.ContainsKey(question.Identifier))
                return indexes[question.Identifier];
            return 0;
        }

        public Dictionary<Guid, int> GetQuestionIndexes(Form form)
        {
            if (!_questionIndexes.ContainsKey(form.Identifier))
            {
                var indexes = new Dictionary<Guid, int>();

                foreach (var field in form.Sections.SelectMany(x => x.Fields))
                {
                    // if (indexes.Any(x => x.Key == field.QuestionIdentifier))
                    //    throw new DuplicateQuestionException("The form can't contain more than one reference to a question.");

                    if (!indexes.Any(x => x.Key == field.QuestionIdentifier))
                        indexes.Add(field.QuestionIdentifier, indexes.Count);
                }

                _questionIndexes.Add(form.Identifier, indexes);
            }

            return _questionIndexes[form.Identifier];
        }
    }
}