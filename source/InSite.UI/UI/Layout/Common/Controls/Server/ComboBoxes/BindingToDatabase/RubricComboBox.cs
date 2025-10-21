using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class RubricComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = ServiceLocator.RubricSearch.GetRubrics(new QRubricFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier 
            });

            foreach (var item in data)
                list.Add(item.RubricIdentifier.ToString(), item.RubricTitle);

            return list;
        }
    }
}