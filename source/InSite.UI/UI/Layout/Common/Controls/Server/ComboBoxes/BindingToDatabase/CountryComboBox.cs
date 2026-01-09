using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CountryComboBox : ComboBox
    {
        public bool OnlyNorthAmerica
        {
            get => (bool)(ViewState[nameof(OnlyNorthAmerica)] ?? false);
            set => ViewState[nameof(OnlyNorthAmerica)] = value;
        }

        public bool ValueAsCode
        {
            get => (bool)(ViewState[nameof(ValueAsCode)] ?? false);
            set => ViewState[nameof(ValueAsCode)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            if (!OnlyNorthAmerica)
                return ServiceLocator.CountrySearch.Select(null, null, true, false, false, ValueAsCode);

            return new ListItemArray
            {
                "Canada",
                "United States"
            };
        }
    }
}
