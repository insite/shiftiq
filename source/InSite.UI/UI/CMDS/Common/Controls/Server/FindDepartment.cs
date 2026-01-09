using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class FindDepartment : BaseFindEntity<FindDepartment.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public DepartmentFilter Filter { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public string CompanyControl
        {
            get { return (string)ViewState[nameof(CompanyControl)]; }
            set { ViewState[nameof(CompanyControl)] = value; }
        }

        public DepartmentFilter Filter => (DepartmentFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new DepartmentFilter()));

        public bool GroupByDivision
        {
            get => (bool)(ViewState[nameof(GroupByDivision)] ?? false);
            set => ViewState[nameof(GroupByDivision)] = value;
        }

        #endregion

        #region Fields

        private FindCompany _companySelector;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (CompanyControl != null)
            {
                System.Web.UI.Control company = NamingContainer.FindControl(CompanyControl);

                _companySelector = company as FindCompany;

                if (_companySelector != null)
                {
                    _companySelector.AutoPostBack = true;
                    _companySelector.ValueChanged += (x, y) => Value = null;
                }
            }
        }

        #endregion

        protected override string GetEntityName() => "Department";

        protected override DataFilter GetFilter(string keyword)
        {
            var filter = new DataFilter
            {
                Filter = Filter.Clone(),
                Keyword = keyword,
            };

            if (_companySelector != null)
                filter.Filter.OrganizationIdentifier = _companySelector.Value ?? Guid.Empty;

            if (filter.Filter.OrganizationIdentifier == Guid.Empty)
                filter.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            return filter;
        }

        protected override int Count(DataFilter filter)
        {
            return ContactRepository3.CountDepartments(filter.Filter, filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            filter.Filter.Paging = filter.Paging;

            var dataTable = ContactRepository3.SelectDepartmentsForSelector(
                filter.Filter, filter.Keyword, GroupByDivision);

            return GroupByDivision
                ? dataTable.Rows.Cast<DataRow>()
                    .GroupBy(x => x["DivisionIdentifier"] as Guid?).Select(g => new GroupItem
                    {
                        Text = (g.First()["DivisionName"] as string).IfNullOrEmpty("No Division"),
                        DataItems = g.Select(GetDataItem).ToArray()
                    })
                    .ToArray()
                : dataTable.Rows.Cast<DataRow>().Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ContactRepository3.SelectDepartmentsForSelector(ids)
                .Rows.Cast<DataRow>()
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(DataRow x) => new DataItem
        {
            Value = (Guid)x["DepartmentIdentifier"],
            Text = (string)x["DepartmentName"],
        };

        public DataItem[] GetDataItems() => Select(GetFilter((string)null));

        public int GetCount() => Count(GetFilter((string)null));
    }
}
