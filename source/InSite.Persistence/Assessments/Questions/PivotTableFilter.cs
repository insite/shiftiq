using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class PivotTableFilter
    {
        private class PivotQuestion
        {
            public int? Difficulty { get; set; }
            public int? Taxonomy { get; set; }
            public int? Competency { get; set; }
            public List<Question> Questions { get; set; }
        }

        private List<PivotQuestion> _pivotQuestions;
        private HashSet<Guid> _existQuestions;
        private List<Tuple<int, Question[]>> _resultGroups;

        private PivotTableFilter() 
        {
            _resultGroups = new List<Tuple<int, Question[]>>();
            _existQuestions = new HashSet<Guid>();
        }

        #region Methods (filtering)

        public static Tuple<int, Question[]>[] ApplyFilter(List<Set> sets, List<Question> questions, PivotTable pivotTable)
        {
            var competencyMapping = GetCompetencyMapping(sets);

            var filter = new PivotTableFilter
            {
                _pivotQuestions = questions
                    .GroupBy(x => new { x.Standard, x.Classification.Difficulty, x.Classification.Taxonomy })
                    .Select(x => new PivotQuestion
                    {
                        Difficulty = x.Key.Difficulty,
                        Taxonomy = x.Key.Taxonomy,
                        Competency = competencyMapping.ContainsKey(x.Key.Standard) ? competencyMapping[x.Key.Standard] : (int?)null,
                        Questions = x.ToList()
                    })
                    .ToList()
            };

            var cols = pivotTable.Columns.GetIndexes();
            var rows = pivotTable.Rows.GetIndexes();

            foreach (var row in rows)
            {
                foreach (var col in cols)
                    filter.AddQuestions(row, col);
            }

            return filter._resultGroups.ToArray();
        }

        private void AddQuestions(IPivotDimensionNode row, IPivotDimensionNode col)
        {
            var cell = row.GetCell(col);
            if (cell.Value == null || cell.Value.Value <= 0)
                return;

            var query = _pivotQuestions.AsQueryable();
            query = AddFilter(row, query);
            query = AddFilter(col, query);

            var groupQuestions = new List<Question>();

            foreach (var question in query.SelectMany(x => x.Questions))
            {
                if (_existQuestions.Contains(question.Identifier))
                    continue;

                _existQuestions.Add(question.Identifier);
                groupQuestions.Add(question);
            }

            if (groupQuestions.Count == 0)
                return;

            _resultGroups.Add(new Tuple<int, Question[]>(cell.Value.Value, groupQuestions.ToArray()));
        }

        private static IQueryable<PivotQuestion> AddFilter(IPivotDimensionNode node, IQueryable<PivotQuestion> query)
        {
            do
            {
                if (int.TryParse(node.Unit, out var unit))
                {
                    var name = node.Dimension;

                    if (name.Equals("Difficulty", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Difficulty == unit);
                    else if (name.Equals("Taxonomy", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Taxonomy == unit);
                    else if (name.Equals("Competency", StringComparison.OrdinalIgnoreCase))
                        query = query.Where(x => x.Competency == unit);
                }

                node = node.Parent;
            }
            while (!node.IsRoot);

            return query;
        }

        #endregion

        #region Methods (helpers)

        private static Dictionary<Guid, int> GetCompetencyMapping(IEnumerable<Set> sets)
        {
            var standards = sets.Select(x => x.Standard);

            var competencies = StandardSearch
                .Bind(
                    x => new
                    {
                        x.StandardIdentifier,
                        x.AssetNumber
                    },
                    x => standards.Contains(x.Parent.StandardIdentifier) && x.StandardType == StandardType.Competency
                );

            return competencies.ToDictionary(x => x.StandardIdentifier, x => x.AssetNumber);
        }

        #endregion
    }
}
