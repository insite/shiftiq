using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Profiles.Certificates
{
    public partial class CertificateCompetencyList : UserControl
    {
        #region Static fields

        private static readonly Regex _nonAlphaNumeric = new Regex(@"[^a-zA-Z\d\s\-\./]");

        #endregion

        #region Fields

        private bool _isLocked;

        #endregion

        #region Properties

        private Guid ProfileStandardIdentifier
        {
            get { return (Guid)ViewState[nameof(ProfileStandardIdentifier)]; }
            set { ViewState[nameof(ProfileStandardIdentifier)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Competencies.ItemDataBound += Competencies_ItemDataBound;
            DownloadButton.Click += (s, a) => DownloadCsv();
        }

        #endregion

        #region Event handlers

        private void Competencies_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;

            var certificationHoursCoreControl = (NumericBox)e.Item.FindControl("CertificationHoursCore");
            var certificationHoursNonCoreControl = (NumericBox)e.Item.FindControl("CertificationHoursNonCore");
            var totalHoursControl = (NumericBox)e.Item.FindControl("TotalHours");

            certificationHoursCoreControl.ReadOnly = _isLocked;
            certificationHoursNonCoreControl.ReadOnly = _isLocked;

            certificationHoursCoreControl.ValueAsDecimal = row["CertificationHoursCore"] as decimal?;
            certificationHoursNonCoreControl.ValueAsDecimal = row["CertificationHoursNonCore"] as decimal?;

            var totalHours = 0m;

            if (row["CertificationHoursCore"] != DBNull.Value)
                totalHours += (decimal)row["CertificationHoursCore"];

            if (row["CertificationHoursNonCore"] != DBNull.Value)
                totalHours += (decimal)row["CertificationHoursNonCore"];

            totalHoursControl.ValueAsDecimal = totalHours == 0 ? (decimal?)null : totalHours;
        }

        #endregion

        #region Public methods

        public int LoadData(Guid profileStandardIdentifier, bool isLocked)
        {
            ProfileStandardIdentifier = profileStandardIdentifier;

            var data = CompetencyRepository.SelectProfileCompetencies(profileStandardIdentifier);

            _isLocked = isLocked;

            Competencies.DataSource = data;
            Competencies.DataBind();

            var totalHoursCore = 0m;
            var totalHoursNonCore = 0m;

            foreach (DataRow row in data.Rows)
            {
                var hoursCore = row["CertificationHoursCore"] as decimal?;
                var hoursNonCore = row["CertificationHoursNonCore"] as decimal?;

                if (hoursCore.HasValue)
                    totalHoursCore += hoursCore.Value;

                if (hoursNonCore.HasValue)
                    totalHoursNonCore += hoursNonCore.Value;
            }

            TotalHoursCore.ValueAsDecimal = totalHoursCore;
            TotalHoursNonCore.ValueAsDecimal = totalHoursNonCore;
            TotalHours.ValueAsDecimal = totalHoursCore + totalHoursNonCore;

            return data.Rows.Count;
        }

        public void SaveHours(Guid profileStandardIdentifier)
        {
            var list = new List<StandardContainment>();

            foreach (RepeaterItem item in Competencies.Items)
            {
                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var certificationHoursCoreControl = (NumericBox)item.FindControl("CertificationHoursCore");
                var certificationHoursNonCoreControl = (NumericBox)item.FindControl("CertificationHoursNonCore");

                var competencyID = Guid.Parse(competencyIDControl.Text);

                var entity = StandardContainmentSearch.BindFirst(x => x, x => x.ParentStandardIdentifier == profileStandardIdentifier && x.ChildStandardIdentifier == competencyID);

                if (entity == null)
                    continue;

                var certificationHoursCore = certificationHoursCoreControl.ValueAsDecimal;
                var certificationHoursNonCore = certificationHoursNonCoreControl.ValueAsDecimal;

                string creditHoursType;
                decimal? creditHours;

                if (certificationHoursCore.HasValue)
                {
                    creditHoursType = "Core";
                    creditHours = certificationHoursCore.Value;
                }
                else if (certificationHoursNonCore.HasValue)
                {
                    creditHoursType = "Non-Core";
                    creditHours = certificationHoursNonCore.Value;
                }
                else
                {
                    creditHoursType = null;
                    creditHours = null;
                }

                if (!string.Equals(creditHoursType, entity.CreditType, StringComparison.OrdinalIgnoreCase)
                    || creditHours != entity.CreditHours
                    )
                {
                    entity.CreditType = creditHoursType;
                    entity.CreditHours = creditHours;

                    list.Add(entity);
                }
            }

            StandardContainmentStore.Update(list);
        }

        #endregion

        #region Helper methods: export to csv

        private void DownloadCsv()
        {
            var csv = CreateCsv();
            var data = Encoding.UTF8.GetBytes(csv);

            Response.SendFile("Competencies.csv", data);
        }

        private string CreateCsv()
        {
            var data = CompetencyRepository.SelectProfileCompetencies(ProfileStandardIdentifier);
            var result = new StringBuilder();

            result.AppendLine("Number,Summary,Core Hours,Non-Core Hours");

            foreach (DataRow row in data.Rows)
            {
                var number = row["Number"] as string;
                var summary = EncloseSummary(row["Summary"] as string);
                var coreHours = row["CertificationHoursCore"] as decimal?;
                var nonCoreHours = row["CertificationHoursNonCore"] as decimal?;

                result.AppendFormat("{0},{1},{2},{3}", number, summary, coreHours, nonCoreHours);
                result.AppendLine();
            }

            return result.ToString();
        }

        private static string EncloseSummary(string summary)
        {
            if (string.IsNullOrEmpty(summary) || !_nonAlphaNumeric.IsMatch(summary))
                return summary;

            return string.Format("\"{0}\"", summary.Replace("\"", "\"\""));
        }

        #endregion
    }
}