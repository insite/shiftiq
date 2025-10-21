using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class FindDepartment : BaseFindEntity<FindDepartment.ListFilter>
    {
        #region Classes

        public class ListFilter : Filter
        {
            public Guid OrganizationIdentifier { get; set; }
            public string Keyword { get; set; }
        }

        private class DepartmentInfo
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentCodeOrName { get; set; }

            public Guid DivisionIdentifier { get; set; }
            public string DivisionCodeOrName { get; set; }

            public static readonly Expression<Func<Department, DepartmentInfo>> DefaultBinder = LinqExtensions1.Expr((Department d) => new DepartmentInfo
            {
                DepartmentIdentifier = d.DepartmentIdentifier,
                DepartmentCodeOrName = d.DepartmentCode ?? d.DepartmentName
            });

            public static readonly Expression<Func<Department, DepartmentInfo>> GroupBinder = LinqExtensions1.Expr((Department d) => new DepartmentInfo
            {
                DepartmentIdentifier = d.DepartmentIdentifier,
                DepartmentCodeOrName = d.DepartmentCode ?? d.DepartmentName,

                DivisionIdentifier = d.Division.DivisionIdentifier,
                DivisionCodeOrName = d.Division.DivisionCode ?? d.Division.DivisionName
            });
        }

        #endregion

        #region Properties

        public Guid OrganizationIdentifier
        {
            get => (Guid)(ViewState[nameof(OrganizationIdentifier)] ?? (ViewState[nameof(OrganizationIdentifier)] = OrganizationIdentifiers.CMDS));
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        public bool GroupByDivision
        {
            get => (bool)(ViewState[nameof(GroupByDivision)] ?? false);
            set => ViewState[nameof(GroupByDivision)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Department";

        protected override ListFilter GetFilter(string keyword)
        {
            return new ListFilter
            {
                OrganizationIdentifier = OrganizationIdentifier,
                Keyword = keyword
            };
        }

        protected override int Count(ListFilter filter)
        {
            var entityFilter = BuildFilter(filter);

            return DepartmentSearch.Count(entityFilter);
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return DepartmentSearch.Bind(DepartmentInfo.DefaultBinder, x => ids.Contains(x.DepartmentIdentifier)).Select(GetDataItem);
        }

        protected override DataItem[] Select(ListFilter filter)
        {
            var entityFilter = BuildFilter(filter);

            return GroupByDivision
                ? DepartmentSearch
                    .Bind(
                        DepartmentInfo.GroupBinder,
                        entityFilter, filter.Paging,
                        nameof(DepartmentInfo.DivisionCodeOrName) + "," + nameof(DepartmentInfo.DepartmentCodeOrName))
                    .GroupBy(x => x.DivisionIdentifier).Select(g => new GroupItem
                    {
                        Text = g.First().DivisionCodeOrName.IfNullOrEmpty("No Division"),
                        DataItems = g.Select(GetDataItem).ToArray()
                    })
                    .ToArray()
                : DepartmentSearch
                    .Bind(DepartmentInfo.DefaultBinder, entityFilter, filter.Paging, nameof(DepartmentInfo.DepartmentCodeOrName))
                    .Select(GetDataItem).ToArray();
        }

        private static Expression<Func<Department, bool>> BuildFilter(ListFilter filter)
        {
            var result = LinqExtensions1.Expr((Department x) => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.Keyword.HasValue())
                result = result.And(x => x.DepartmentCode.Contains(filter.Keyword) || x.DepartmentName.Contains(filter.Keyword));

            return result.Expand();
        }

        private static DataItem GetDataItem(DepartmentInfo entity) => new DataItem
        {
            Value = entity.DepartmentIdentifier,
            Text = entity.DepartmentCodeOrName
        };
    }
}