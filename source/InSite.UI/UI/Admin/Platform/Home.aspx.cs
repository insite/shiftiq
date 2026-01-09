using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Settings
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LdcMonitorEnabled.AutoPostBack = true;
            LdcMonitorEnabled.CheckedChanged += (s, a) =>
            {
                ServiceLocator.Partition.DatabaseMonitorLargeCommandSize = LdcMonitorEnabled.Checked ? 8192 : 0;

                AppSentry.UpdateDatabaseCommandMonitor();

                BindLdcMonitor();
            };

            LdcMonitorIncludeStackTrace.AutoPostBack = true;
            LdcMonitorIncludeStackTrace.CheckedChanged += (s, a) =>
            {
                ServiceLocator.Partition.DatabaseMonitorIncludeStackTrace = LdcMonitorIncludeStackTrace.Checked;

                AppSentry.UpdateDatabaseCommandMonitor();

                BindLdcMonitor();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            ConfigurationProviderRepeater.DataSource = ServiceLocator.AppSettings.ConfigurationProviders;

            ConfigurationProviderRepeater.DataBind();

            var insite = CurrentSessionState.Identity.IsOperator;

            var actionCount = TActionSearch.Count(new TActionFilter());
            LoadCounter(ActionCounter, ActionCount, insite, actionCount, ActionLink, "/ui/admin/platform/routes/search");

            var collectionCount = TCollectionSearch.Count(new TCollectionFilter());
            LoadCounter(CollectionCounter, CollectionCount, insite, collectionCount, CollectionLink, "/ui/admin/assets/collections/search");

            var procStartTime = Shift.Common.Clock.GetProcessStartTime();
            var appStartTime = Shift.Common.Clock.GetApplicationStartTime();

            MaintenanceUptime.Text =
                $"The application pool was last recycled <span title='{procStartTime:MMM d, yyyy} {procStartTime:hh:mm tt} UTC'>{procStartTime.Humanize()}</span>. "
              + $"The web site was last recycled <span title='{appStartTime:MMM d, yyyy} {appStartTime:hh:mm tt} UTC'>{appStartTime.Humanize()}</span>.";

            MaintenanceIntervalRepeater.DataSource = ServiceLocator.AppSettings.Platform.Maintenance.Lockouts;
            MaintenanceIntervalRepeater.DataBind();

            BindLdcMonitor();
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }

        protected string DescribeRecurrence(object item)
        {
            var lockout = (Lockout)item;
            if (lockout.Interval.IsRecurring())
                return $"Every {string.Join(", ", lockout.Interval.Recurrences)}";
            else
                return $"Not Recurring (one-time only)";
        }

        protected string DescribeFilter(object item)
        {
            var sb = new StringBuilder();

            var lockout = (Lockout)item;

            if (lockout.FilterPartitions())
            {
                sb.Append("<span class='badge bg-info me-2'>");
                sb.Append(string.Join(", ", lockout.Partitions));
                sb.Append("</span>");
            }

            if (lockout.FilterEnvironments())
            {
                sb.Append("<span class='badge bg-primary me-2'>");
                sb.Append(string.Join(", ", lockout.Environments));
                sb.Append("</span>");
            }

            return sb.ToString();
        }

        private void BindLdcMonitor()
        {
            var size = ServiceLocator.Partition.DatabaseMonitorLargeCommandSize;
            var trace = ServiceLocator.Partition.DatabaseMonitorIncludeStackTrace;

            LdcMonitorEnabled.Checked = size > 0;
            LdcReportStatus.InnerText = AppSentry.IsDatabaseCommandMonitorEnabled ? "Working" : "Stopped";
            LdcMonitorDatabaseCommandSize.InnerText = $"{size:N0}";
            LdcMonitorIncludeStackTrace.Checked = trace;
        }
    }
}