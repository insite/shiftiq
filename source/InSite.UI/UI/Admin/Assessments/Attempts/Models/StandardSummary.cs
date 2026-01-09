using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attempts.Models
{
    [Serializable]
    public abstract partial class StandardSummary
    {
        #region Properties

        public Guid ID { get; }
        public string Name { get; }
        public string Code { get; }
        public int Count { get; set; }

        public decimal QuestionPoints
        {
            get
            {
                CalculatePoints();
                return _questionPoints;
            }
        }

        public decimal AnswerPoints
        {
            get
            {
                CalculatePoints();
                return _answerPoints;
            }
        }

        public decimal Score
        {
            get
            {
                CalculatePoints();
                return _questionPoints == 0 ? 0 : Calculator.GetPercentDecimal(_answerPoints, _questionPoints);
            }
        }

        public abstract bool HasData { get; }

        #endregion

        #region Fields

        private decimal _answerPoints;
        private decimal _questionPoints;
        private bool _isCalculated = false;

        #endregion

        #region Construction

        public StandardSummary(Guid id, string name, string code)
        {
            ID = id;
            Name = name;
            Code = code;
        }

        public StandardSummary(Guid id, string type, string code, string title)
            : this(id, type + (code.IsEmpty() ? " " : $" {code}. ") + title, (code.IsEmpty() ? " " : $"{code}"))
        {
        }

        #endregion

        #region Methods

        protected abstract IEnumerable<DataItem> GetDataItems();

        private void CalculatePoints()
        {
            if (_isCalculated)
                return;

            _isCalculated = true;

            _answerPoints = 0m;
            _questionPoints = 0m;

            if (!HasData)
                return;

            foreach (var attempt in GetDataItems().GroupBy(x => x.Source.Attempt).ToArray())
            {
                foreach (var question in attempt.GroupBy(x => x.Source.Question).ToArray())
                {
                    var priority = question.Min(x => x.Priroty);

                    foreach (var item in question.Where(x => x.Priroty == priority))
                    {
                        _answerPoints += item.AnswerPoints;
                        _questionPoints += item.TotalPoints;
                    }
                }
            }
        }

        public static Occupation[] GetData(AttemptAnalysis analysis, bool includeOptions, string language = "en")
        {
            var mapping = new MappingData();

            GetCompetenciesMapping(mapping, analysis, includeOptions);

            var gacAssets = StandardSearch.Bind(x => new
            {
                OccupationID = (Guid?)x.Parent.Parent.StandardIdentifier,
                OccupationSubtype = x.Parent.Parent.StandardType,
                OccupationCode = x.Parent.Parent.Code,
                OccupationTitle = CoreFunctions.GetContentText(x.Parent.Parent.StandardIdentifier, ContentLabel.Title, language)
                    ?? CoreFunctions.GetContentTextEn(x.Parent.Parent.StandardIdentifier, ContentLabel.Title),
                OccupationSequence = (int?)x.Parent.Parent.Sequence,
                FrameworkID = (Guid?)x.Parent.StandardIdentifier,
                FrameworkSubtype = x.Parent.StandardType,
                FrameworkCode = x.Parent.Code,
                FrameworkTitle = CoreFunctions.GetContentText(x.Parent.StandardIdentifier, ContentLabel.Title, language)
                    ?? CoreFunctions.GetContentTextEn(x.Parent.StandardIdentifier, ContentLabel.Title),
                FrameworkSequence = (int?)x.Parent.Sequence,
                FrameworkPassingScore = x.Parent.PassingScore,
                GacID = x.StandardIdentifier,
                GacSequence = x.Sequence
            }, x => mapping.Gac.Keys.Contains(x.StandardIdentifier));

            foreach (var asset in gacAssets)
            {
                var gac = mapping.Gac[asset.GacID];

                var occupationId = asset.OccupationID ?? Guid.Empty;
                if (!mapping.Occupation.TryGetValue(occupationId, out var occupation))
                    mapping.Occupation.Add(occupationId, occupation = new Occupation(
                        occupationId, 
                        asset.OccupationSubtype, 
                        asset.OccupationCode, 
                        asset.OccupationTitle)
                        {
                            Sequence = asset.OccupationSequence
                        });

                var frameworkId = asset.FrameworkID ?? Guid.Empty;
                if (!mapping.Framework.TryGetValue(frameworkId, out var framework))
                {
                    mapping.Framework.Add(frameworkId, framework = new Framework(
                        frameworkId,
                        asset.FrameworkSubtype,
                        asset.FrameworkCode,
                        asset.FrameworkTitle,
                        asset.FrameworkPassingScore ?? 0.75m)
                        {
                            Sequence = asset.FrameworkSequence
                        });
                    occupation.Frameworks.Add(framework);
                }

                framework.Gacs.Add(gac);
            }

            return mapping.Occupation.Values
                .Where(x => x.HasData)
                .OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.Name)
                .ToArray();
        }

        private static void GetCompetenciesMapping(MappingData mapping, AttemptAnalysis analysis, bool includeOptions)
        {
            foreach (var attempt in analysis.Attempts)
            {
                foreach (var question in attempt.Questions)
                {
                    var gac = mapping.GetGac(question);
                    var competency = mapping.GetCompetency(gac, question);

                    competency.AddData(
                        attempt.AttemptIdentifier, question.QuestionIdentifier, 0,
                        question.QuestionPoints ?? 0, question.AnswerPoints ?? 0);

                    foreach (var subQuestion in question.SubQuestions)
                    {
                        var subQuestionGac = mapping.GetGac(subQuestion);
                        var subQuestionCompetency = mapping.GetCompetency(subQuestionGac, subQuestion);

                        subQuestionCompetency.AddData(
                            attempt.AttemptIdentifier, subQuestion.QuestionIdentifier, 1,
                            subQuestion.QuestionPoints ?? 0, subQuestion.AnswerPoints ?? 0);
                    }
                }

                if (includeOptions)
                {
                    foreach (var option in attempt.Options)
                    {
                        var gac = mapping.GetGac(option.CompetencyAreaIdentifier, option.CompetencyAreaLabel, option.CompetencyAreaCode, option.CompetencyAreaTitle);
                        var competency = mapping.GetCompetency(gac, option.CompetencyItemIdentifier, option.CompetencyItemLabel, option.CompetencyItemCode, option.CompetencyItemTitle);

                        competency.AddData(attempt.AttemptIdentifier, option.QuestionIdentifier, 1, option.OptionPoints, option.AnswerIsSelected == true ? option.OptionPoints : 0);
                    }
                }
            }
        }

        #endregion
    }
}