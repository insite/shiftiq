using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardValidationChangeFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        private void Grid_RowCommand(object source, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRecord")
            {
                var logId = Grid.GetDataKey<Guid>(e);
                var entity = ServiceLocator.StandardValidationSearch.GetStandardValidationLog(logId);
                if (entity != null)
                    StandardValidationChangeStore.Delete(entity.StandardValidationIdentifier, entity.LogIdentifier);

                SearchWithCurrentPageIndex(Filter);
            }
        }

        protected override int SelectCount(StandardValidationChangeFilter filter)
        {
            return StandardValidationChangeSearch.Count(filter);
        }

        protected override IListSource SelectData(StandardValidationChangeFilter filter)
        {
            return StandardValidationChangeSearch.Select(filter);
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }
    }
}