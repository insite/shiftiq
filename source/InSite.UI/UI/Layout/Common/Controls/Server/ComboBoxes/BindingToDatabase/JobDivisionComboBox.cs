using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class JobDivisionComboBox : ComboBox
    {
        public JobDivisionComboBox()
        {
            EmptyMessage = "Job Division";
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var organizationId = CurrentSessionState.Identity.OrganizationId;

            if (organizationId != null)
            {
                var jobDivisions = ServiceLocator.PersonSearch.GetJobDivisions(organizationId.Value);

                foreach (var jobDivision in jobDivisions)
                {
                    var item = new ListItem { Value = jobDivision, Text = jobDivision };

                    list.Items.Add(item);
                }
            }

            return list;
        }
    }
}
