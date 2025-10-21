using System;
using System.Data;
using System.Web.UI;

using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.User
{
    public partial class ExpiredCompetencies : UserControl
    {
        #region Public methods

        public Boolean LoadData(UserProfileKey key)
        {
            var competencies = UserCompetencyRepository.SelectExpiringCompetencies(key);

            rpCompetencies.DataSource = competencies;
            rpCompetencies.DataBind();

            return competencies.Rows.Count > 0;
        }

        #endregion

        #region Data binding evaluation methods

        protected static String GetCompetencyText(Object dataItem)
        {
            var row = (DataRowView)dataItem;

            var competencyStandardIdentifier = (Guid)row["CompetencyStandardIdentifier"];
            var userKey = (Guid)row["UserIdentifier"];
            var name = string.Format("Competency #{0}", row["Number"]);
            var expirationDate = row["ExpirationDate"] as DateTimeOffset?;

            var verb = expirationDate.HasValue && expirationDate.Value.Date > DateTime.UtcNow.Date
                ? "will expire"
                : "expired";

            var url = string.Format("/ui/cmds/portal/validations/competencies/edit?id={0}&userID={1}", competencyStandardIdentifier, userKey);

            return string.Format("<a href='{0}'>{1}</a> {2} {3:MMM d, yyyy}", url, name, verb, expirationDate);
        }

        protected static CmdsFlagType GetCompetencyFlag(Object dataItem)
        {
            if (dataItem == null || dataItem == DBNull.Value)
                throw new ArgumentNullException("dataItem");

            var row = (DataRowView)dataItem;
            var expirationDate = row["ExpirationDate"] as DateTimeOffset?;
            var expirationStatus = row["ValidationStatus"] as String;

            return (expirationDate.HasValue && expirationDate.Value.Date > DateTime.UtcNow.Date) || (expirationStatus == ValidationStatuses.SubmittedForValidation)
                ? CmdsFlagType.Yellow
                : CmdsFlagType.Red;
        }

        #endregion
    }
}