using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class StandardTypeComboBox : ComboBox
    {
        public bool OnlyForCurrentOrganization
        {
            get => (bool)(ViewState[nameof(OnlyForCurrentOrganization)] ?? false);
            set => ViewState[nameof(OnlyForCurrentOrganization)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var query = OnlyForCurrentOrganization
                ? StandardSearch.SelectStandardTypes(CurrentSessionState.Identity.Organization.Identifier)
                : StandardSearch.GetAllTypeNames(CurrentSessionState.Identity.Organization.Identifier);

            var list = new ListItemArray();

            foreach (var type in query)
            {
                if (type != StandardType.Collection)
                    list.Add(type, type);
            }

            return list;
        }
    }
}