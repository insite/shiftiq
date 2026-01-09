using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Domain.Reports;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Messages.Messages
{
    public partial class Search : SearchPage<MessageFilter>
    {
        private string DefaultType
        {
            get
            {
                var type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(type))
                    return HttpUtility.UrlDecode(type);
                return null;
            }
        }

        public bool HasDefaultCriteria => !string.IsNullOrEmpty(DefaultType);

        public override string EntityName => "Message";

        protected override void OnGridRowCommand(SearchResultsRowCommandArgs e)
        {
            if (e.CommandName == "ArchiveRecord")
            {
                //var gridController = (IGridSearchResults) Results;
                //var dataKeys = gridController.GetDataKeys(e.Item);

                //Edit.ArchiveMessage(dataKeys[0], AdminPage);
                //{
                //    ScreenStatus.Message = "The message is now archived.";
                //    ScreenStatus.Type = AlertType.Success;

                //    gridController.SearchWithCurrentPageIndex(Results.Filter);
                //}
            }
            else
            {
                base.OnGridRowCommand(e);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack && !SearchResults.HasRows && SearchCriteria.HasDefaultCriteria)
                SearchResults.Search(SearchCriteria.Filter);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload
                .GetColumns(typeof(InSite.Admin.Messages.Messages.Controls.SearchResults.ExportDataItem))
                .OrderBy(x => x.Name);
        }

        protected override IList GetExportData(int? take = null)
        {
            return SearchResults.GetExportData(take).GetList();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string addUrl, addTitle;

            if (HasDefaultCriteria)
            {
                addUrl = $"/ui/admin/messages/create?type={DefaultType}";
                addTitle = $"Add New {DefaultType}";
            }
            else
            {
                addUrl = $"/ui/admin/messages/create";
                addTitle = $"Add New Message";
            }

            PageHelper.AutoBindHeader(this, new BreadcrumbItem(addTitle, addUrl, null, null));
        }
    }
}