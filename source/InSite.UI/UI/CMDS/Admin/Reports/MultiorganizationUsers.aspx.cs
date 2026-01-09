using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class MultiorganizationUsers : AdminBasePage, ICmdsUserControl
    {
        private class CompanyDataItem
        {
            public string CompanyName { get; set; }
            public CompanyUserDataItem[] Items { get; set; }

            public int ItemsCount => Items.Length;
            public CompanyUserDataItem FirstItem => Items[0];
        }

        private class CompanyUserDataItem
        {
            public string FullName { get; set; }
            public bool IsFirst { get; set; }
            public bool IsLast { get; set; }
        }

        private List<Guid> CompanySelectorValues
        {
            get => (List<Guid>)ViewState[nameof(CompanySelectorValues)];
            set => ViewState[nameof(CompanySelectorValues)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompanyIdentifier.AutoPostBack = true;
            CompanyIdentifier.ValueChanged += (s, a) => OnRefresh();

            CompanyRepeater.ItemDataBound += CompanyRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CompanyIdentifier.Values = null;

            OnRefresh();
        }

        private void CompanyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("UserRepeater");
            repeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Items");
            repeater.DataBind();
        }

        private void OnRefresh()
        {
            UserTab.Visible = true;
            OrganizationTab.Visible = true;

            UserTab.IsSelected = true;

            LoadReport();
        }

        private void LoadReport()
        {
            var identifiers = CompanyIdentifier.Values.NullIfEmpty();

            UserRepeater.DataSource = CmdsReportHelper.SelectMultiOrganizationUser(identifiers);
            UserRepeater.DataBind();

            CompanyRepeater.DataSource = CmdsReportHelper.SelectMultiOrganizationCompany(identifiers)
                .GroupBy(x => x.CompanyName)
                .Select(x =>
                {
                    var items = x.ToArray();

                    return new CompanyDataItem
                    {
                        CompanyName = x.Key,
                        Items = items.Select((y, i) => new CompanyUserDataItem
                        {
                            FullName = y.FullName,
                            IsFirst = i == 0,
                            IsLast = i == items.Length - 1,
                        }).ToArray()
                    };
                });
            CompanyRepeater.DataBind();
        }

        protected string GetCompanyUserClass()
        {
            var dataItem = (CompanyUserDataItem)Page.GetDataItem();

            return dataItem.IsFirst
                ? "border-top-0"
                : dataItem.IsLast
                    ? "border-bottom-0"
                    : null;
        }
    }
}