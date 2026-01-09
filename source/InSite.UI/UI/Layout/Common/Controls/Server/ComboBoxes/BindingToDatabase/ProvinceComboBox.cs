using System;
using System.Linq;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class ProvinceComboBox : ComboBox
    {
        private CountryComboBox _countrySelector;

        public string Country
        {
            get { return (string)ViewState[nameof(Country)]; }
            set { ViewState[nameof(Country)] = value; }
        }

        public string CountryControl { get; set; }

        public bool UseCodeAsValue
        {
            get { return ViewState[nameof(UseCodeAsValue)] as bool? ?? false; }
            set { ViewState[nameof(UseCodeAsValue)] = value; }
        }

        public bool OnlyCanadaAndUnitedStates
        {
            get { return ViewState[nameof(OnlyCanadaAndUnitedStates)] as bool? ?? false; }
            set { ViewState[nameof(OnlyCanadaAndUnitedStates)] = value; }
        }

        public bool OnlyCanada
        {
            get { return ViewState[nameof(OnlyCanada)] as bool? ?? false; }
            set { ViewState[nameof(OnlyCanada)] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            CountryInit();

            base.OnLoad(e);
        }

        protected override ListItemArray CreateDataSource()
        {
            CountryInit();

            if (!UseCodeAsValue)
                return new ListItemArray(ServiceLocator.ProvinceSearch.SelectByCountryName(Country));

            if (OnlyCanadaAndUnitedStates && UseCodeAsValue)
            {
                var valuesCA = ServiceLocator.ProvinceSearch.SelectByCountryCode("CA");
                var valuesUS = ServiceLocator.ProvinceSearch.SelectByCountryCode("US");

                var allValues = valuesCA.Concat(valuesUS)
                            .ToList()
                            .Select(x => new ListItem { Value = x.Code, Text = $"{x.Country} - {x.Name}" })
                            .Prepend(new ListItem
                            {
                                Value = "--",
                                Text = "Outside US/Canada"
                            });

                return new ListItemArray(allValues);
            }
            else if (OnlyCanada)
            {
                var valuesCA = ServiceLocator.ProvinceSearch.SelectByCountryCode("CA");
                var allValues = valuesCA
                            .Select(x => new ListItem { Value = x.Code, Text = $"{x.Country} - {x.Name}" })
                            .ToList();

                return new ListItemArray(allValues);
            }

            var provinces = ServiceLocator.ProvinceSearch.SelectEntitiesByCountryName(Country);

            var data = provinces.Select(x => new ListItem { Value = x.Code, Text = x.Name });

            return new ListItemArray(data);
        }

        private void CountrySelector_ValueChanged(object o, ComboBoxValueChangedEventArgs e)
        {
            if (Country != e.NewValue)
                ClearSelection();

            Country = e.NewValue ?? "";

            RefreshData();
        }

        private void CountryInit()
        {
            if (Country != null || _countrySelector != null)
                return;

            if (string.IsNullOrEmpty(CountryControl))
            {
                Country = "Canada";
                return;
            }

            _countrySelector = (CountryComboBox)Parent.FindControl(CountryControl);

            if (_countrySelector != null)
            {
                _countrySelector.AutoPostBack = true;
                _countrySelector.ValueChanged += CountrySelector_ValueChanged;
            }
        }
    }
}