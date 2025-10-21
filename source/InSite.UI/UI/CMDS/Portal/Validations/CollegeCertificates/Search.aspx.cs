using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using RadioButtonList = System.Web.UI.WebControls.RadioButtonList;

namespace InSite.Cmds.Portal.Validations.CollegeCertificates
{
    public partial class Search : AdminBasePage, ICmdsUserControl
    {
        #region Properties

        private DataTable _institutions;

        private Guid? ProfileStandardIdentifier => Guid.TryParse(Request["profile"], out var key) ? key : (Guid?)null;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Certifications.ItemDataBound += Certifications_ItemDataBound;
            Certifications.ItemCommand += Certifications_ItemCommand;

            ExportButton.Click += ExportButton_Click;

            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            LoadData();
        }

        #endregion

        #region Event handlers

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var row = (DataRowView)e.Row.DataItem;
            var certificationHoursCore = row["CertificationHoursCore"] == DBNull.Value ? 0 : (decimal)row["CertificationHoursCore"];
            var certificationHoursNonCore = row["CertificationHoursNonCore"] == DBNull.Value ? 0 : (decimal)row["CertificationHoursNonCore"];
            var totalHours = certificationHoursCore + certificationHoursNonCore;

            var totalHoursLabel = (ITextControl)e.Row.FindControl("TotalHours");
            totalHoursLabel.Text = string.Format("{0:n2}", totalHours);
        }

        private void Certifications_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var profileStandardIdentifier = (Guid)row["ProfileStandardIdentifier"];
            var isSatisfied = (bool)row["CoreIsSatisfied"] && (bool)row["NonCoreIsSatisfied"];
            var dateRequested = row["DateRequested"] as DateTimeOffset?;
            var dateGranted = row["DateGranted"] as DateTimeOffset?;
            var dateSubmitted = row["DateSubmitted"] as DateTimeOffset?;

            var viewMissingCompetenciesRow = (HtmlTableRow)e.Item.FindControl("ViewMissingCompetenciesRow");
            viewMissingCompetenciesRow.Visible = !(bool)row["CoreIsSatisfied"] || !(bool)row["NonCoreIsSatisfied"];

            var viewMissingCompetenciesButton = (LinkButton)e.Item.FindControl("ViewMissingCompetenciesButton");
            viewMissingCompetenciesButton.OnClientClick = string.Format(
                "showMissingCompetencies('{0}'); return false;", profileStandardIdentifier);

            //Send Request
            var requestNotAllowedPanel = (PlaceHolder)e.Item.FindControl("RequestNotAllowedPanel");
            var requestPanel = (PlaceHolder)e.Item.FindControl("RequestPanel");
            var statusPanel = (PlaceHolder)e.Item.FindControl("StatusPanel");

            requestNotAllowedPanel.Visible = !isSatisfied;
            requestPanel.Visible = isSatisfied && dateRequested == null && dateGranted == null && dateSubmitted == null;
            statusPanel.Visible = dateRequested.HasValue || dateGranted.HasValue || dateSubmitted.HasValue;

            //Status text
            var statusTextLabel = (ITextControl)e.Item.FindControl("StatusText");

            var status = new StringBuilder();

            if (dateRequested.HasValue)
                status.AppendFormat("<br/>Requested on {0:MMM d, yyyy}", dateRequested);

            if (dateSubmitted.HasValue)
                status.AppendFormat("<br/>Submitted on {0:MMM d, yyyy}", dateSubmitted);

            if (dateGranted.HasValue)
                status.AppendFormat("<br/>Granted on {0:MMM d, yyyy}", dateGranted);

            statusTextLabel.Text = status.Length == 0 ? null : status.ToString().Substring("<br/>".Length);

            var institutionRadioButtonList = (RadioButtonList)e.Item.FindControl("Institution");
            institutionRadioButtonList.DataTextField = "Text";
            institutionRadioButtonList.DataValueField = "Value";
            institutionRadioButtonList.DataSource = GetSchools();
            institutionRadioButtonList.DataBind();
        }

        private void Certifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SendRequest")
            {
                var profileStandardIdentifier = Guid.Parse((string)e.CommandArgument);
                var institutionRadioButtonList = (RadioButtonList)e.Item.FindControl("Institution");

                if (!string.IsNullOrEmpty(institutionRadioButtonList.SelectedValue))
                    SendRequest(profileStandardIdentifier, institutionRadioButtonList.SelectedItem.Text);

                LoadData();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            var csv = CreateCsvFile();

            Response.SendFile("Certification Missing Competencies.csv", Encoding.UTF8.GetBytes(csv));
        }

        #endregion

        #region Helper methods

        private void LoadData()
        {
            var table = UserCompetencyRepository.SelectCertifications(User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, null);

            table.Columns.Add("CorePercentage", typeof(decimal));
            table.Columns.Add("NonCorePercentage", typeof(decimal));
            table.Columns.Add("CoreIsSatisfied", typeof(bool));
            table.Columns.Add("NonCoreIsSatisfied", typeof(bool));

            foreach (DataRow row in table.Rows)
            {
                var coreHoursCompleted = (decimal)row["CoreHours"];
                var nonCoreHoursCompleted = (decimal)row["NonCoreHours"];

                var coreHoursRequired = (decimal)row["CoreHoursRequired"];
                var nonCoreHoursRequired = (decimal)row["NonCoreHoursRequired"];

                var coreHoursTotal = (decimal)row["CoreHoursTotal"];
                var nonCoreHoursTotal = (decimal)row["NonCoreHoursTotal"];

                var corePercentage = coreHoursTotal != 0 ? coreHoursCompleted / coreHoursTotal : 1;
                var nonCorePercentage = nonCoreHoursTotal != 0 ? nonCoreHoursCompleted / nonCoreHoursTotal : 1;

                var coreIsSatisfied = coreHoursCompleted >= coreHoursRequired;
                var nonCoreIsSatisfied = nonCoreHoursCompleted >= nonCoreHoursRequired;

                row["CorePercentage"] = corePercentage;
                row["NonCorePercentage"] = nonCorePercentage;

                row["CoreIsSatisfied"] = coreIsSatisfied;
                row["NonCoreIsSatisfied"] = nonCoreIsSatisfied;

                row.AcceptChanges();
            }

            Certifications.DataSource = table;
            Certifications.DataBind();

            NoCertifications.Visible = table.Rows.Count == 0;

            if (ProfileStandardIdentifier.HasValue)
                LoadResults();
        }

        private void LoadResults()
        {
            var table = UserCompetencyRepository.SelectCertificationMissingCompetencies(ProfileStandardIdentifier.Value, User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);

            Grid.Visible = table.Rows.Count > 0;
            Grid.EnablePaging = false;

            Grid.DataSource = table;
            Grid.DataBind();

            NoDataPanel.Visible = table.Rows.Count == 0;

            TabResults.Visible = true;
            TabResults.IsSelected = true;
        }

        private DataTable GetSchools()
        {
            if (_institutions == null)
                _institutions = ContactRepository3.SelectForSchoolSelector();

            return _institutions;
        }

        private void SendRequest(Guid profileStandardIdentifier, string institutionName)
        {
            SaveRequest(profileStandardIdentifier, institutionName);

            var profile = StandardSearch.Select(profileStandardIdentifier);

            var requested = new CmdsCollegeCertificationRequested
            {
                Name = User.FullName,
                Email = User.Email,
                ProfileNumber = profile.Code,
                ProfileTitle = profile.ContentTitle,
                Institution = institutionName
            };

            AddHours(requested, User.UserIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier, profile.StandardIdentifier);

            ServiceLocator.ChangeQueue.Publish(requested);

            ScreenStatus.AddMessage(AlertType.Success,
                string.Format(
                    "Thank you for submitting your request for {0} Certification. The CMDS administrator will contact you with the application form.",
                    institutionName));
        }

        private void AddHours(CmdsCollegeCertificationRequested request, Guid userKey, Guid organizationId, Guid profileStandardIdentifier)
        {
            var certifications = UserCompetencyRepository.SelectCertifications(userKey, organizationId, profileStandardIdentifier);
            var certificationRow = certifications.Rows[0];

            var coreHoursCompleted = (decimal)certificationRow["CoreHours"];
            var nonCoreHoursCompleted = (decimal)certificationRow["NonCoreHours"];
            var coreHoursRequired = (decimal)certificationRow["CoreHoursRequired"];
            var nonCoreHoursRequired = (decimal)certificationRow["NonCoreHoursRequired"];

            request.CoreHoursRequired = string.Format("{0:n2}", coreHoursRequired);
            request.CoreHoursCompleted = string.Format("{0:n2}", coreHoursCompleted);
            request.NonCoreHoursRequired = string.Format("{0:n2}", nonCoreHoursRequired);
            request.NonCoreHoursCompleted = string.Format("{0:n2}", nonCoreHoursCompleted);
        }

        private void SaveRequest(Guid profile, string authority)
        {
            bool isNew = false;

            var certificate = TCollegeCertificateSearch.Select(User.UserIdentifier, profile);
            isNew = certificate == null;

            if (isNew)
                certificate = new TCollegeCertificate
                {
                    CertificateIdentifier = UniqueIdentifier.Create(),
                    LearnerIdentifier = User.UserIdentifier,
                    ProfileIdentifier = profile
                };

            certificate.CertificateTitle = StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == profile) ?? "Missing Title";
            certificate.CertificateAuthority = authority;
            certificate.DateRequested = DateTime.UtcNow;

            if (isNew)
                TCollegeCertificateStore.Insert(certificate);
            else
                TCollegeCertificateStore.Update(certificate);
        }

        private string CreateCsvFile()
        {
            if (ProfileStandardIdentifier.HasValue)
            {
                var csv = new StringBuilder();

                csv.AppendFormat("Number{0}Summary{0}Core Hours{0}Non-Core Hours{0}Total Hours{0}Status", ',');

                var table = UserCompetencyRepository.SelectCertificationMissingCompetencies(ProfileStandardIdentifier.Value, User.Identifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);

                foreach (DataRow row in table.Rows)
                {
                    var certificationHoursCore = row["CertificationHoursCore"] == DBNull.Value ? 0 : (decimal)row["CertificationHoursCore"];
                    var certificationHoursNonCore = row["CertificationHoursNonCore"] == DBNull.Value ? 0 : (decimal)row["CertificationHoursNonCore"];
                    var totalHours = certificationHoursCore + certificationHoursNonCore;

                    csv.Append("\r\n");
                    csv.Append(PrepareValueForCsv(row["CompetencyNumber"] as string));
                    csv.Append(','.ToString());
                    csv.Append(PrepareValueForCsv(row["CompetencySummary"] as string));
                    csv.Append(','.ToString());
                    csv.Append(PrepareValueForCsv(string.Format("{0:n2}", row["CertificationHoursCore"])));
                    csv.Append(','.ToString());
                    csv.Append(PrepareValueForCsv(string.Format("{0:n2}", row["CertificationHoursNonCore"])));
                    csv.Append(','.ToString());
                    csv.Append(PrepareValueForCsv(string.Format("{0:n2}", totalHours)));
                    csv.Append(','.ToString());
                    csv.Append(row["ValidationStatus"] == DBNull.Value ? "Not Assigned" : (string)row["ValidationStatus"]);
                }

                return csv.ToString();
            }

            return null;
        }

        private string PrepareValueForCsv(String value)
        {
            return string.IsNullOrEmpty(value) ? null : string.Format("\"{0}\"", value.Replace("\"", "\"\""));
        }

        #endregion
    }
}