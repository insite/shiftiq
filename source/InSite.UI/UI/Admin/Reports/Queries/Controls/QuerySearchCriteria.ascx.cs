using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Reports.Queries.Controls
{
    public partial class QuerySearchCriteria : SearchCriteriaController<SqlFilter>
    {
        private static string GetReportsPath()
            => HttpContext.Current.Server.MapPath($"~/Library/Tenants/{Organization.OrganizationCode}/Reports");

        public event BooleanValueHandler DataUpdated;

        private void OnDataUpdated(bool hasData)
            => DataUpdated?.Invoke(this, new BooleanValueArgs(hasData));

        public override SqlFilter Filter
        {
            get
            {
                var filter = new SqlFilter();

                if (ReportList.SelectedIndex > -1)
                    filter.Query = GetReportSql(ReportList.SelectedValue);

                return filter;
            }
            set
            {
                // Query.Text = value?.Query;
            }
        }

        public string SelectedReport => ReportList.SelectedValue;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportList.AutoPostBack = true;
            ReportList.SelectedIndexChanged += SearchButton_Click;

            DownloadButton.Click += DownloadButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            QueryFile.FileUploaded += QueryFile_FileUploaded;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                UploadQueryPanel.Visible = Identity.IsOperator;
                DeletePanel.Visible = Identity.IsOperator;

                LoadData();
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var folder = GetReportsPath();
            var filePath = Path.Combine(folder, ReportList.SelectedValue);

            try
            {
                if (!File.Exists(filePath))
                    return;

                var name = Path.GetFileNameWithoutExtension(filePath);
                var sql = File.ReadAllText(filePath);
                Response.SendFile(name, "sql", Encoding.UTF8.GetBytes(sql));
            }
            catch (Exception ex)
            {
                UploadAlert.AddMessage(AlertType.Error, $"Unexpected error. {ex.Message}");
                return;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ReportList.SelectedValue))
                return;

            var folder = GetReportsPath();
            var filePath = Path.Combine(folder, ReportList.SelectedValue);

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                UploadAlert.AddMessage(AlertType.Error, $"Unexpected error. {ex.Message}");
                return;
            }

            UploadAlert.AddMessage(AlertType.Success, $"Query {ReportList.SelectedValue} is deleted");

            LoadData();
        }

        private void QueryFile_FileUploaded(object sender, EventArgs e)
        {
            if (!QueryFile.HasFile)
            {
                UploadAlert.AddMessage(AlertType.Error, "Unexpected error. Please re-upload the query file");
                return;
            }

            var folder = CreateReportsFolder();
            if (folder == null)
                return;

            var fileName = QueryFile.FileName;

            try
            {
                var query = QueryFile.ReadFileText(Encoding.UTF8);
                if (!Validate(query))
                    return;

                var filePath = Path.Combine(folder, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.WriteAllText(filePath, query, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                UploadAlert.AddMessage(AlertType.Error, $"Unexpected error. {ex.Message}");
                return;
            }
            finally
            {
                QueryFile.ClearFiles();
            }

            UploadAlert.AddMessage(AlertType.Success, $"Query {fileName} is uploaded");

            LoadData();
        }

        private string CreateReportsFolder()
        {
            var folder = GetReportsPath();

            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                UploadAlert.AddMessage(AlertType.Error, $"Unexpected error. {ex.Message}");
                return null;
            }

            return folder;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            DeleteButton.Visible = true;

            OnSearching();
        }

        private void LoadData()
        {
            var list = GetReports();
            if (list != null && list.Length > 0)
                OnDataUpdated(true);

            var value = ReportList.SelectedValue;

            ReportList.DataSource = list;
            ReportList.DataBind();

            if (!string.IsNullOrEmpty(value) && list.Contains(value))
                ReportList.SelectedValue = value;

            DeleteButton.Visible = !string.IsNullOrEmpty(ReportList.SelectedValue);

            CriteriaPanel.Visible = list.Length > 0;
        }

        public static string[] GetReports()
        {
            var path = GetReportsPath();
            if (!Directory.Exists(path))
                return new string[0];

            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.sql");

            return files.Select(x => x.Name).ToArray();
        }

        private string GetReportSql(string script)
        {
            var path = Server.MapPath($"~/Library/Tenants/{Organization.OrganizationCode}/Reports/{script}");
            var sql = File.ReadAllText(path);
            return sql.Replace("@TenantIdentifier", $"'{Organization.Identifier}'");
        }

        public override void Clear()
        {
        }

        private bool Validate(string query)
        {
            if (string.IsNullOrEmpty(query))
                return false;

            if (query.Contains(";"))
            {
                UploadAlert.AddMessage(AlertType.Error, "<strong><i class='fas fa-exclamation-square'></i> Access Denied.</strong> Your query cannot include any semicolon characters.");
                return false;
            }

            if (!StringHelper.StartsWith(query, "select"))
            {
                UploadAlert.AddMessage(AlertType.Error, "<strong><i class='fas fa-exclamation-square'></i> Access Denied.</strong> Your query must start with the SQL keyword <strong>SELECT</strong>.");
                return false;
            }

            return true;
        }
    }
}