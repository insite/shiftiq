using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class IssueStatusComboBox : ComboBox
    {
        public string IssueType
        {
            get => (string)ViewState[nameof(IssueType)];
            set => ViewState[nameof(IssueType)] = value;
        }

        public string StatusCategory
        {
            get => (string)ViewState[nameof(StatusCategory)];
            set => ViewState[nameof(StatusCategory)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var org = CurrentSessionState.Identity.Organization;

            if (StatusCategory != null && IssueType == null)
                return GetSubCategoryStatusList(org);

            return GetStatusList(org);
        }

        private ListItemArray GetStatusList(Domain.Organizations.OrganizationState org)
        {
            var items = StatusCategory.HasNoValue()
                ? ServiceLocator.IssueSearch.GetStatuses(org.Identifier, IssueType)
                : ServiceLocator.IssueSearch.GetStatuses(org.Identifier, IssueType, StatusCategory);

            var list = new ListItemArray();
            foreach (var i in items)
                list.Add(i.StatusIdentifier.ToString(), i.StatusName);
            return list;
        }

        private ListItemArray GetSubCategoryStatusList(Domain.Organizations.OrganizationState org)
        {
            var items = ServiceLocator.IssueSearch.GetStatusNamesByCategory(org.Identifier, StatusCategory);

            var list = new ListItemArray();
            foreach (var i in items)
                list.Add(i, i);
            return list;
        }
    }
}