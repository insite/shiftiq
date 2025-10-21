using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ExamAttemptTagMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
                        
            foreach (var item in ServiceLocator.AttemptSearch.GetAttemptTags(CurrentSessionState.Identity.Organization.OrganizationIdentifier))
                list.Add(item);

            return list;
        }
    }
}