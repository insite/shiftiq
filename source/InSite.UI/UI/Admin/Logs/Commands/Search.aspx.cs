using System;

using InSite.Application.Logs.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Logs.Commands
{
    public partial class Search : SearchPage<CommandFilter>
    {
        public override string EntityName => "Command";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Command", "/ui/admin/logs/commands/create", null, null));

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);

            TestButton.Click += TestButton_Click;
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            SearchResults.SearchWithCurrentPageIndex(SearchResults.Filter);
        }
    }
}