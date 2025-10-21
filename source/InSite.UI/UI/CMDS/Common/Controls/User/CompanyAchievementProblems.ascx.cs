using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;

namespace InSite.Cmds.Controls.Contacts.Companies.Files
{
    public partial class CompanyAchievementProblems : UserControl
    {
        #region Events

        public event EventHandler Fixed;

        private void OnFixed() => Fixed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid CompanyThumbrint
        {
            get { return (Guid)ViewState[nameof(CompanyThumbrint)]; }
            set { ViewState[nameof(CompanyThumbrint)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FixButton.Click += FixButton_Click;
        }

        #endregion

        #region Event handlers

        private void FixButton_Click(object sender, EventArgs e)
        {
            var table = UploadRepository.SelectCompanyPotentialDataIssues(CompanyThumbrint);
            var list = new List<TAchievementStandard>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new TAchievementStandard
                {
                    AchievementIdentifier = (Guid)row["AchievementIdentifier"],
                    StandardIdentifier = (Guid)row["CompetencyStandardIdentifier"]
                });
            }

            TAchievementStandardStore.Insert(list);

            OnFixed();
        }

        #endregion

        #region Public methods

        public Boolean LoadData(Guid thumbprint)
        {
            CompanyThumbrint = thumbprint;

            var table = UploadRepository.SelectCompanyPotentialDataIssues(CompanyThumbrint);
            if (table.Rows.Count == 0)
                return false;

            IssueText.Visible = table.Rows.Count == 1;
            IssueList.Visible = table.Rows.Count > 1;

            if (table.Rows.Count == 1)
            {
                var row = table.Rows[0];
                var type = (string)row["UploadType"];
                var name = type == UploadType.Link ? "link" : "file";

                IssueText.Text = string.Format(
                    "The {0} named \"{1}\" is assigned to competency {2} and to the achievement titled \"{3}\". However, the achievement is not assigned to the competency.",
                    name,
                    row["UploadName"],
                    row["CompetencyNumber"],
                    row["AchievementTitle"]);
            }
            else
            {
                IssueList.DataSource = table;
                IssueList.DataBind();
            }

            return true;
        }

        #endregion
    }
}