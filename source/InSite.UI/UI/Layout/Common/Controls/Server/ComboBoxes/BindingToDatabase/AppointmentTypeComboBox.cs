using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class AppointmentTypeComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var org = CurrentSessionState.Identity.Organization.Identifier;
            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = org,
                CollectionName = CollectionName.Activities_Appointments_Appointment_Type
            });

            var names = items.Select(x => x.ItemName).ToList();

            return new ListItemArray(names);
        }
    }
}