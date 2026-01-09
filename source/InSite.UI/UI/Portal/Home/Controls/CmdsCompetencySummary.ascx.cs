using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class CmdsCompetencySummary : UserControl
    {
        #region CompetencySummaryItem class

        private class CompetencySummaryItem
        {
            public string Status { get; set; }
            public int Width { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region Fields

        private int _maxBarWidth = 300;

        private Guid _personId;
        private Guid _organizationId;
        private Guid? _profileId;
        private Guid? _departmentId;

        #endregion

        #region Properties

        public int MaxBarWidth
        {
            get { return _maxBarWidth; }
            set { _maxBarWidth = value; }
        }

        private CompetencySummaryType SummaryType
        {
            get { return (CompetencySummaryType)ViewState[nameof(SummaryType)]; }
            set { ViewState[nameof(SummaryType)] = value; }
        }

        #endregion

        #region Public methods

        public void BindModelToControls(Guid personId, Guid organizationId, CompetencySummaryType summaryType)
        {
            _personId = personId;
            _organizationId = organizationId;
            _profileId = null;
            _departmentId = null;

            SummaryType = summaryType;

            LoadSummary();
        }

        public void BindModelToControls(Guid personID, Guid profileID, Guid department)
        {
            _personId = personID;
            _organizationId = Guid.Empty;
            _profileId = profileID;
            _departmentId = department;

            SummaryType = CompetencySummaryType.EmployeeProfile;

            LoadSummary();
        }

        #endregion

        #region Helper methods

        private void LoadSummary()
        {
            DataTable counts;

            switch (SummaryType)
            {
                case CompetencySummaryType.ManagerGroup:
                    counts = UserCompetencyRepository.SelectStatusCountsForManager(_personId, _organizationId);
                    break;
                case CompetencySummaryType.EmployeePrimaryProfile:
                    counts = UserCompetencyRepository.SelectStatusCountsForEmployeePrimaryProfile(_personId, _organizationId);
                    break;
                case CompetencySummaryType.EmployeeProfile:
                    counts = UserCompetencyRepository.SelectStatusCountsForEmployeeProfile(_personId, _profileId.Value, _departmentId.Value);
                    break;
                case CompetencySummaryType.Employee:
                    counts = UserCompetencyRepository.SelectStatusCountsForEmployee(_personId, _organizationId);
                    break;
                case CompetencySummaryType.EmployeeComplianceProfiles:
                    counts = UserCompetencyRepository.SelectStatusCountsForComplianceProfiles(_personId, _organizationId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown SummaryType: " + SummaryType.ToString());
            }

            var maxCount = GetMaxCount(counts);

            var statuses = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.CMDS,
                CollectionName = CollectionName.Validations_Verification_Status
            });

            var list = new List<CompetencySummaryItem>();

            foreach (var item in statuses)
            {
                var count = GetCountForStatus(counts, item.ItemName);

                list.Add(new CompetencySummaryItem
                {
                    Status = item.ItemName,
                    Value = string.Format("{0:n0}", count),
                    Width = MaxBarWidth * count / maxCount
                });
            }

            Summary.DataSource = list;
            Summary.DataBind();
        }

        private static int GetMaxCount(DataTable table)
        {
            var maxCount = 1;

            foreach (DataRow row in table.Rows)
            {
                if (maxCount < (int)row["Count"])
                    maxCount = (int)row["Count"];
            }

            return maxCount;
        }

        private static int GetCountForStatus(DataTable table, string status)
        {
            foreach (DataRow row in table.Rows)
            {
                if (StringHelper.Equals((string)row["ValidationStatus"], status))
                    return (int)row["Count"];
            }

            return 0;
        }

        protected string GetSelfAssessmentFinderUrl(object status)
        {
            switch (SummaryType)
            {
                case CompetencySummaryType.EmployeeProfile:
                    return string.Format("/ui/cmds/portal/validations/competencies/search?userID={0}&status={1}&profile={2}&department={3}", _personId, status, _profileId, _departmentId);
                case CompetencySummaryType.ManagerGroup:
                    return string.Format("/ui/cmds/portal/validations/competencies/search?userID={0}&status={1}&mode=group&organization={2}", _personId, status, _organizationId);
                default:
                    return string.Format("/ui/cmds/portal/validations/competencies/search?userID={0}&status={1}", _personId, status);
            }
        }

        #endregion

        #region Data binding evaluation methods

        protected string GetBarColor(object objStatus)
        {
            var status = (string)objStatus;

            switch (status)
            {
                case ValidationStatuses.NotCompleted:
                    return "#808080";
                case ValidationStatuses.NotApplicable:
                    return "#337ab7";
                case ValidationStatuses.NeedsTraining:
                    return "Orange";
                case ValidationStatuses.SelfAssessed:
                    return "#f74f78";
                case ValidationStatuses.SubmittedForValidation:
                    return "#ffb15c";
                case ValidationStatuses.Validated:
                    return "#16c995";
                case ValidationStatuses.Expired:
                    return "Black";
                default:
                    return "#8F9EC3";
            }
        }

        #endregion
    }
}