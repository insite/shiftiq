using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindAchievementList : BaseFindEntity<FindAchievementList.ListFilter>
    {
        #region Classes

        public class ListFilter : Filter
        {
            public Expression<Func<TProgram, bool>> Filter { get; set; }
        }

        #endregion

        #region Properties

        public Guid DepartmentIdentifier
        {
            get => ViewState[nameof(DepartmentIdentifier)] as Guid? ?? Guid.Empty;
            set => ViewState[nameof(DepartmentIdentifier)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Program";

        protected override ListFilter GetFilter(string keyword)
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;

            return new ListFilter
            {
                Filter = x => x.OrganizationIdentifier == organization
                    && (x.GroupIdentifier == null || x.GroupIdentifier == DepartmentIdentifier)
                    && (keyword == null || x.ProgramName.Contains(keyword))
            };
        }

        protected override int Count(ListFilter filter)
        {
            return ProgramSearch1.Count(filter.Filter);
        }

        protected override DataItem[] Select(ListFilter filter)
        {
            return ProgramSearch1
                .Bind(
                    x => new DataItem
                    {
                        Value = x.ProgramIdentifier,
                        Text = x.ProgramName
                    },
                    filter.Filter,
                    filter.Paging,
                    nameof(DataItem.Text))
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ProgramSearch1
                .Bind(
                    x => new DataItem
                    {
                        Value = x.ProgramIdentifier,
                        Text = x.ProgramName
                    },
                    x => ids.Contains(x.ProgramIdentifier));
        }
    }
}