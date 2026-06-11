using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence.Plugin.NCSHA
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChartDataModel
    {
        #region Classes

        public class Filter
        {
            public string[] Code { get; }
            public string[] Region { get; }
            public int? FromYear { get; }
            public int? ToYear { get; }
            public string AggregateFunction { get; }

            public Filter(ChartDataGetModel input)
            {
                Code = input.Code;
                Region = input.Region;
                FromYear = input.FromYear;
                ToYear = input.ToYear;
                AggregateFunction = input.Func;
            }
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; private set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; private set; }

        [JsonProperty(PropertyName = "data")]
        public IDictionary<int, decimal> Points { get; private set; }

        #endregion

        #region Construction

        private ChartDataModel()
        {

        }

        #endregion

        #region Initialization

        public static IEnumerable<ChartDataModel> Create(Filter filter)
        {
            var result = new List<ChartDataModel>();

            foreach (var code in filter.Code)
            {
                var info = CounterRepository.BindFirst(x => new { x.Name, x.Unit }, x => x.Code == code);

                if (filter.AggregateFunction == "Actual")
                {
                    var regionData = CounterRepository
                        .Bind(x => new { x.Scope, x.Year, x.Value }, GetExpression(code))
                        .AsQueryable()
                        .GroupBy(x => x.Scope)
                        .Select(x => new
                        {
                            Scope = x.Key,
                            Points = x.ToDictionary(y => y.Year, y => y.Value)
                        })
                        .OrderBy(x => x.Scope);

                    foreach (var region in regionData)
                    {
                        var model = new ChartDataModel
                        {
                            Name = info.Name + $" ({region.Scope})",
                            Unit = info.Unit,
                            Color = null,
                            Points = region.Points
                        };

                        result.Add(model);
                    }
                }
                else
                {
                    var yearData = CounterRepository
                        .Bind(x => new { x.Year, x.Value }, GetExpression(code))
                        .AsQueryable()
                        .GroupBy(x => x.Year);

                    IDictionary<int, decimal> points;

                    if (filter.AggregateFunction == "Average")
                        points = yearData.ToDictionary(x => x.Key, x => x.Average(y => y.Value));
                    else if (filter.AggregateFunction == "Minimum")
                        points = yearData.ToDictionary(x => x.Key, x => x.Min(y => y.Value));
                    else if (filter.AggregateFunction == "Maximum")
                        points = yearData.ToDictionary(x => x.Key, x => x.Max(y => y.Value));
                    else
                        points = yearData.ToDictionary(x => x.Key, x => x.Sum(y => y.Value));

                    var model = new ChartDataModel
                    {
                        Name = info.Name
                            + (filter.Region.IsEmpty()
                                ? $" ({filter.AggregateFunction})"
                                : (filter.Region.Length == 1
                                    ? $" ({filter.Region[0]})"
                                    : $" ({filter.AggregateFunction}: {string.Join(", ", filter.Region)})"
                                )
                            ),
                        Unit = info.Unit,
                        Color = null,
                        Points = points
                    };

                    result.Add(model);
                }
            }

            return result;

            Expression<Func<Counter, bool>> GetExpression(string code)
            {
                var filterExpr = LinqExtensions1.Expr((Counter x) => x.Code == code);

                if (filter.Region.IsNotEmpty())
                    filterExpr = filterExpr.And(x => filter.Region.Contains(x.Scope));

                if (filter.FromYear.HasValue)
                    filterExpr = filterExpr.And(x => x.Year >= filter.FromYear.Value);

                if (filter.ToYear.HasValue)
                    filterExpr = filterExpr.And(x => x.Year <= filter.ToYear.Value);

                return filterExpr.Expand();
            }
        }

        #endregion
    }
}