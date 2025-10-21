using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Admin.Uploads.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<UploadFilter>
    {
        protected override int SelectCount(UploadFilter filter)
        {
            return UploadSearch.CountByFilter(filter);
        }

        protected override IListSource SelectData(UploadFilter filter)
        {
            return ToDataTable(UploadSearch.SelectByFilter(filter));
        }

        private static DataTable ToDataTable(SearchResultList list)
        {
            var table = new DataTable();

            table.Columns.Add("UploadIdentifier", typeof(Guid));
            table.Columns.Add("ContainerType", typeof(string));
            table.Columns.Add("UploadType", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Uploaded", typeof(DateTimeOffset));

            foreach (var dataItem in list.GetList())
            {
                var row = table.NewRow();

                var uploadType = (string)DataBinder.Eval(dataItem, "UploadType");
                if (uploadType == UploadType.CmdsFile)
                    uploadType = "File";

                row["UploadIdentifier"] = DataBinder.Eval(dataItem, "UploadIdentifier") ?? DBNull.Value;
                row["ContainerType"] = DataBinder.Eval(dataItem, "ContainerType") ?? DBNull.Value;
                row["UploadType"] = (object)uploadType ?? DBNull.Value;
                row["Title"] = DataBinder.Eval(dataItem, "Title") ?? DBNull.Value;
                row["Description"] = DataBinder.Eval(dataItem, "Description") ?? DBNull.Value;
                row["Uploaded"] = DataBinder.Eval(dataItem, "Uploaded") ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}