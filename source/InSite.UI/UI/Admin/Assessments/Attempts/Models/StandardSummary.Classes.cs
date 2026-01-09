using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;

namespace InSite.Admin.Assessments.Attempts.Models
{
    public partial class StandardSummary
    {
        [Serializable]
        public sealed class Occupation : StandardSummary
        {
            public int? Sequence { get; set; }
            public override bool HasData => Frameworks.Any(x => x.HasData);

            public List<Framework> Frameworks { get; } = new List<Framework>();

            public Occupation(Guid id, string name) : base(id, name, "") { }
            public Occupation(Guid id, string type, string code, string title) : base(id, type, code, title) { }

            protected override IEnumerable<DataItem> GetDataItems() => Frameworks.SelectMany(x => x.GetDataItems());
        }

        [Serializable]
        public sealed class Framework : StandardSummary
        {
            public int? Sequence { get; set; }
            public override bool HasData => Gacs.Any(x => x.HasData);

            public List<Gac> Gacs { get; } = new List<Gac>();

            public string FrameworkTitle { get; }
            public decimal PassingScore { get; }

            public Framework(Guid id, string name) : base(id, name, "") { }
            public Framework(Guid id, string type, string code, string title, decimal passingScore) : base(id, type, code, title)
            {
                FrameworkTitle = title;
                PassingScore = passingScore;
            }

            protected override IEnumerable<DataItem> GetDataItems() => Gacs.SelectMany(x => x.GetDataItems());
        }

        [Serializable]
        public sealed class Gac : StandardSummary
        {
            public int? Sequence { get; set; }
            public override bool HasData => Competencies.Any(x => x.HasData);

            public List<Competency> Competencies { get; } = new List<Competency>();

            public Gac(Guid id, string name) : base(id, name, "") { }
            public Gac(Guid id, string type, string code, string title) : base(id, type, code, title) { }

            protected override IEnumerable<DataItem> GetDataItems() => Competencies.SelectMany(x => x.GetDataItems());
        }

        [Serializable]
        public sealed class Competency : StandardSummary
        {
            public int? Sequence { get; set; }
            public override bool HasData => _dataItems.Count > 0;

            private List<DataItem> _dataItems = new List<DataItem>();

            public Competency(Guid id, string name) : base(id, name, "") { }
            public Competency(Guid id, string type, string code, string title) : base(id, type, code, title) { }

            protected override IEnumerable<DataItem> GetDataItems() => _dataItems;

            public void AddData(Guid attempt, Guid question, int priority, decimal total, decimal answer)
            {
                _dataItems.Add(new DataItem
                {
                    Source = (attempt, question),
                    Priroty = priority,
                    TotalPoints = total,
                    AnswerPoints = answer
                });
            }
        }

        [Serializable]
        protected struct DataItem
        {
            public (Guid Attempt, Guid Question) Source { get; set; }
            public int Priroty { get; set; }
            public decimal TotalPoints { get; set; }
            public decimal AnswerPoints { get; set; }
        }

        private class MappingData
        {
            public Dictionary<Guid, Occupation> Occupation { get; }
            public Dictionary<Guid, Framework> Framework { get; }
            public Dictionary<Guid, Gac> Gac { get; }
            public Dictionary<Guid, Competency> Competency { get; }

            public MappingData()
            {
                var defaultOccupation = new Occupation(Guid.Empty, "N/A");
                var defaultFramework = new Framework(Guid.Empty, "N/A");
                var defaultGac = new Gac(Guid.Empty, "N/A");
                var defaultCompetency = new Competency(Guid.Empty, "N/A");

                defaultOccupation.Frameworks.Add(defaultFramework);
                defaultFramework.Gacs.Add(defaultGac);
                defaultGac.Competencies.Add(defaultCompetency);

                Occupation = new Dictionary<Guid, Occupation>
                {
                    { defaultOccupation.ID, defaultOccupation }
                };
                Framework = new Dictionary<Guid, Framework>
                {
                    { defaultFramework.ID, defaultFramework }
                };
                Gac = new Dictionary<Guid, Gac>
                {
                    { defaultGac.ID, defaultGac }
                };
                Competency = new Dictionary<Guid, Competency>
                {
                    { defaultCompetency.ID, defaultCompetency }
                };
            }

            public Gac GetGac(AttemptAnalysis.QuestionEntity question) =>
                GetGac(question.CompetencyAreaIdentifier, question.CompetencyAreaLabel, question.CompetencyAreaCode, question.CompetencyAreaTitle);

            public Gac GetGac(Guid? id, string label, string code, string title)
            {
                var gacId = id ?? Guid.Empty;

                if (!Gac.TryGetValue(gacId, out var gac))
                    Gac.Add(gacId, gac = new Gac(gacId, label, code, title));

                return gac;
            }

            public Competency GetCompetency(Gac gac, AttemptAnalysis.QuestionEntity question) =>
                GetCompetency(gac, question.CompetencyItemIdentifier, question.CompetencyItemLabel, question.CompetencyItemCode, question.CompetencyItemTitle);

            public Competency GetCompetency(Gac gac, Guid? id, string label, string code, string title)
            {
                var competencyId = id ?? Guid.Empty;

                if (!Competency.TryGetValue(competencyId, out var competency))
                {
                    Competency.Add(competencyId, competency = new Competency(competencyId, label, code, title));
                    gac.Competencies.Add(competency);
                }

                return competency;
            }
        }
    }
}