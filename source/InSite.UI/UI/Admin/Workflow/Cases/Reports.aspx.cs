using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Issues.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.UI.Desktops.Admin.Reports
{
    public partial class Issues : AdminBasePage
    {
        private class IssueStatisticGroup
        {
            public string Name { get; set; }

            public IEnumerable<IssueStatisticItem> Items { get; set; }
        }

        private class IssueStatisticItem
        {
            public string Text { get; set; }
            public int Count { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StatisticDateType.AutoPostBack = true;
            StatisticDateType.ValueChanged += StatisticDateType_ValueChanged;

            StatisticDateShortcut.AutoPostBack = true;
            StatisticDateShortcut.ValueChanged += StatisticDateShortcut_ValueChanged;

            StatisticRepeater.ItemDataBound += StatisticRepeater_ItemDataBound;

            StatisticFilterButton.Click += StatisticFilterButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            StatisticDateShortcut.LoadItems(
                DateRangeShortcut.Today,
                DateRangeShortcut.Yesterday,
                DateRangeShortcut.ThisWeek,
                DateRangeShortcut.LastWeek,
                DateRangeShortcut.ThisMonth,
                DateRangeShortcut.LastMonth,
                DateRangeShortcut.ThisYear,
                DateRangeShortcut.LastYear);
            StatisticDateShortcut.Items.Add(new ComboBoxOption("Custom Dates", "Custom"));

            StatisticDateShortcut.Value = "Custom";
            StatisticCustomDateInputs.Visible = true;
            StatisticDateSince.Value = DateTime.Today.AddMonths(-3);
            StatisticDateBefore.Value = DateTime.Today.AddMonths(3);

            LoadStatistic();
        }

        private void StatisticDateType_ValueChanged(object sender, EventArgs e)
        {
            LoadStatistic();
        }

        private void StatisticDateShortcut_ValueChanged(object sender, EventArgs e)
        {
            StatisticCustomDateInputs.Visible = StatisticDateShortcut.Value == "Custom";

            LoadStatistic();
        }

        private void StatisticFilterButton_Click(object sender, EventArgs e)
        {
            LoadStatistic();
        }

        private void StatisticRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var groupData = (IssueStatisticGroup)e.Item.DataItem;
            var hasData = groupData.Items.Any();

            var itemsRepeater = (Repeater)e.Item.FindControl("ItemsRepeater");
            var noDataMessage = e.Item.FindControl("NoDataMessage");

            itemsRepeater.Visible = hasData;
            itemsRepeater.DataSource = groupData.Items;
            itemsRepeater.DataBind();

            noDataMessage.Visible = !hasData;
        }

        private void LoadStatistic()
        {
            var range = StatisticDateShortcut.Value == "Custom"
                ? new DateTimeRange(StatisticDateSince.Value, StatisticDateBefore.Value)
                : Shift.Common.Calendar.GetDateTimeRange(StatisticDateShortcut.Value.ToEnum<DateRangeShortcut>(false));

            StatisticDateSince.Value = range.Since;
            StatisticDateBefore.Value = range.Before;

            var filter = CreateFilter();
            var dateType = StatisticDateType.Value;

            if (dateType == "Reported")
            {
                filter.DateReportedSince = range.Since;
                filter.DateReportedBefore = range.Before;
            }
            else if (dateType == "Opened")
            {
                filter.DateOpenedSince = range.Since;
                filter.DateOpenedBefore = range.Before;
            }
            else if (dateType == "Closed")
            {
                filter.DateClosedSince = range.Since;
                filter.DateClosedBefore = range.Before;
            }

            var groups = new List<IssueStatisticGroup>();
            var appointments = ServiceLocator.IssueSearch.GetIssues(filter);

            { // Statuses
                var items = appointments
                    .GroupBy(x => x.IssueStatusCategory)
                    .Select(x => new IssueStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();

                groups.Add(new IssueStatisticGroup
                {
                    Name = "Statuses",
                    Items = items.OrderBy(x => x.Text)
                });
            }

            { // Types
                var items = appointments
                    .GroupBy(x => x.IssueType)
                    .Select(x => new IssueStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new IssueStatisticGroup
                {
                    Name = "Types",
                    Items = items
                });
            }

            { // Administrators
                var items = appointments
                    .GroupBy(x => x.AdministratorUserName)
                    .Select(x => new IssueStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new IssueStatisticGroup
                {
                    Name = "Administrators",
                    Items = items
                });
            }

            var comments = ServiceLocator.IssueSearch.GetComments(new QIssueCommentFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                DatePostedSince = range.Since,
                DatePostedBefore = range.Before
            });

            { // Comments
                var items = comments
                    .GroupBy(x => x.AuthorUserName)
                    .Select(x => new IssueStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new IssueStatisticGroup
                {
                    Name = "Comments",
                    Items = items
                });
            }

            StatisticRepeater.DataSource = groups;
            StatisticRepeater.DataBind();
        }

        private QIssueFilter CreateFilter()
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            };
            return filter;
        }
    }
}
