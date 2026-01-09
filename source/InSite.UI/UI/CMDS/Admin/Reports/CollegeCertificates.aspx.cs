using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class CollegeCertificates : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        protected class SearchParameters
        {
            public Guid[] Departments { get; set; }
            public Guid[] Employees { get; set; }
            public string AuthorityName { get; set; }
            public string CertificateType { get; set; }
        }

        private class DepartmentGroupInfo
        {
            #region Properties

            public string CompanyName { get; private set; }

            public string DepartmentName { get; private set; }

            public IEnumerable<CertificateGroupInfo> Certificates { get; set; }

            #endregion

            #region Construction

            public DepartmentGroupInfo(CmdsReportHelper.CollegeCertificate row)
            {
                CompanyName = row.CompanyName;
                DepartmentName = row.DepartmentName;
            }

            #endregion
        }

        private class CertificateGroupInfo
        {
            #region Properties

            public string Name { get; private set; }

            public IEnumerable<EmployeeInfo> Employees { get; set; }

            #endregion

            #region Construction

            public CertificateGroupInfo(CmdsReportHelper.CollegeCertificate row)
            {
                Name = row.CertificateType;
            }

            #endregion
        }

        private class EmployeeInfoComparer : IEqualityComparer<EmployeeInfo>
        {
            public int Compare(EmployeeInfo x, EmployeeInfo y)
            {
                if (x == null && y == null)
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return x.FullName.CompareTo(y.FullName)
                     + x.ProfileTitle.CompareTo(y.ProfileTitle)
                     + x.InstitutionName.CompareTo(y.InstitutionName);
            }

            public bool Equals(EmployeeInfo x, EmployeeInfo y)
            {
                return Compare(x, y) == 0;
            }

            public int GetHashCode(EmployeeInfo obj)
            {
                unchecked // Allow arithmetic overflow; numbers will wrap around.
                {
                    int hashcode = 1430287;
                    if (obj.FullName != null)
                        hashcode = hashcode * 7302013 ^ obj.FullName.GetHashCode();
                    if (obj.ProfileTitle != null)
                        hashcode = hashcode * 7302013 ^ obj.ProfileTitle.GetHashCode();
                    if (obj.InstitutionName != null)
                        hashcode = hashcode * 7302013 ^ obj.InstitutionName.GetHashCode();
                    return hashcode;
                }
            }
        }

        private class EmployeeInfo
        {
            #region Properties

            public string FullName { get; private set; }

            public string ProfileTitle { get; private set; }

            public string InstitutionName { get; private set; }

            #endregion

            #region Construction

            public EmployeeInfo(CmdsReportHelper.CollegeCertificate row)
            {
                FullName = row.FullName;
                ProfileTitle = row.ProfileTitle;
                InstitutionName = row.InstitutionName;
            }

            #endregion
        }

        #endregion

        #region Properties

        protected SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifierValidator.ServerValidate += (s, a) => a.IsValid = DepartmentIdentifier.Enabled;
            EmployeeIdentifierValidataor.ServerValidate += (s, a) => a.IsValid = EmployeeIdentifier.Enabled;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => UpdateEmployeeIdentifier();

            MembershipTypeChanged.Click += (s, a) => UpdateEmployeeIdentifier();

            ReportButton.Click += ReportButton_Click;

            DownloadButton.Click += DownloadButton_Click;

            DepartmentRepeater.ItemCreated += DepartmentRepeater_ItemCreated;
            DepartmentRepeater.ItemDataBound += DepartmentRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.Enabled = hasDepartments;
            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";

            UpdateEmployeeIdentifier();
        }

        private void UpdateEmployeeIdentifier()
        {
            EmployeeIdentifier.Values = null;
            EmployeeIdentifier.Filter.Departments = DepartmentIdentifier.Values;

            if (EmployeeIdentifier.Filter.Departments.Length == 0)
                EmployeeIdentifier.Filter.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            EmployeeIdentifier.Filter.RoleType = MembershipType.Items
                .Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();

            var hasData = EmployeeIdentifier.Filter.Departments.Length > 0
                && EmployeeIdentifier.Filter.RoleType.Length > 0
                && EmployeeIdentifier.GetCount() > 0;

            EmployeeIdentifier.Enabled = hasData;
            EmployeeIdentifier.EmptyMessage = hasData ? "All Employees" : "None";
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var reportHtml = GetControlHtml(DepartmentRepeater);
            if (reportHtml.IsEmpty())
                return;

            var bodyHtml = GetFileContent("~/UI/CMDS/Admin/Reports/CollegeCertificates_PdfBody.html");
            bodyHtml = bodyHtml
                .Replace("<!-- TITLE -->", Route.Title)
                .Replace("<!-- BODY -->", reportHtml);
            bodyHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, bodyHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),

                MarginTop = 22,

                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/CMDS/Admin/Reports/CollegeCertificates_PdfHeader.html"),
                HeaderSpacing = 7,
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            var filename = StringHelper.Sanitize(Route.Title, '-', false);

            Response.SendFile(filename, "pdf", data);

            string GetFileContent(string virtualPath)
            {
                var physPath = MapPath(virtualPath);

                return File.ReadAllText(physPath);
            }

            string GetControlHtml(Control ctrl)
            {
                var sb = new StringBuilder();

                using (var stringWriter = new StringWriter(sb))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    {
                        ctrl.RenderControl(htmlWriter);
                    }
                }

                return sb.ToString();
            }
        }

        private void DepartmentRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var certificateRepeater = (Repeater)e.Item.FindControl("CertificateRepeater");
            certificateRepeater.ItemDataBound += CertificateRepeater_ItemDataBound;
        }

        private void DepartmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var departmentInfo = (DepartmentGroupInfo)e.Item.DataItem;

            var certificateRepeater = (Repeater)e.Item.FindControl("CertificateRepeater");
            certificateRepeater.DataSource = departmentInfo.Certificates;
            certificateRepeater.DataBind();
        }

        private void CertificateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var certificateInfo = (CertificateGroupInfo)e.Item.DataItem;

            var employeeRepeater = (Repeater)e.Item.FindControl("EmployeeRepeater");
            employeeRepeater.DataSource = certificateInfo.Employees;
            employeeRepeater.DataBind();
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                Departments = DepartmentIdentifier.Values,
                Employees = EmployeeIdentifier.Values,
                AuthorityName = InstitutionIdentifier.Value.NullIfEmpty(),
                CertificateType = CertificateType.Value == "All" ? null : CertificateType.Value
            };

            if (CurrentParameters.Departments.Length == 0)
                CurrentParameters.Departments = DepartmentIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            if (CurrentParameters.Employees.Length == 0)
                CurrentParameters.Employees = EmployeeIdentifier.GetDataItems().Select(x => x.Value).ToArray();

            if (CurrentParameters.Departments.Length == 0 || CurrentParameters.Employees.Length == 0)
                return;

            var dataSource = GetReportDataSource();

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;
            DownloadButton.Visible = dataSource.Any();

            DepartmentRepeater.DataSource = dataSource;
            DepartmentRepeater.DataBind();
        }

        private IEnumerable<DepartmentGroupInfo> GetReportDataSource()
        {
            var rows = CmdsReportHelper.SelectReport024(CurrentParameters.Departments, CurrentParameters.Employees, CurrentParameters.AuthorityName, CurrentParameters.CertificateType);

            return rows
                .GroupBy(x => x.DepartmentIdentifier)
                .Select(departmentGroup => new DepartmentGroupInfo(departmentGroup.First())
                {
                    Certificates = departmentGroup
                        .GroupBy(x => x.CertificateType)
                        .Select(certificateGroup => new CertificateGroupInfo(certificateGroup.First())
                        {
                            Employees = certificateGroup
                                .Select(x => new EmployeeInfo(x))
                                .Distinct(new EmployeeInfoComparer())
                                .OrderBy(x => x.FullName)
                                .ToArray()
                        })
                        .OrderBy(x => x.Name)
                        .ToArray()
                })
                .OrderBy(x => x.DepartmentName)
                .ToArray();
        }

        #endregion
    }
}