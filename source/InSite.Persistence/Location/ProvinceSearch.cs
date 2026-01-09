using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Common.Integration.Google;

namespace InSite.Persistence
{
    public class ProvinceSearch
    {
        private readonly string _googleApiUrl;

        private readonly CountrySearch _countrySearch;

        private Province[] _provinces;

        public ProvinceSearch(string engineApiUrl, CountrySearch countrySearch)
        {
            _googleApiUrl = engineApiUrl;

            _countrySearch = countrySearch;
        }

        private Province[] GetProvinces()
        {
            Refresh();

            return _provinces;
        }

        private void Refresh()
        {
            if (_provinces != null)
                return;

            var client = new LocationClient();

            var ca = client.CollectProvinces(_googleApiUrl, "ca");

            var us = client.CollectProvinces(_googleApiUrl, "us");

            _provinces = ca.Union(us)
                .OrderBy(x => x.Country)
                .ThenBy(x => x.Name)
                .ToArray();
        }

        public string Abbreviate(string name)
        {
            return GetProvinces().FirstOrDefault(x => x.Name == name)?.Code;
        }

        public int Count(string searchText)
        {
            var query = GetProvinces().AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(x => x.Name.StartsWith(searchText));

            return query.Count();
        }

        public IReadOnlyList<Province> SelectByCountryCode(string code)
        {
            return GetProvinces().Where(x => StringHelper.Equals(x.Country, code)).OrderBy(x => x.Name)
                .ToArray();
        }

        public string[] SelectByCountryName(string name)
        {
            var code = _countrySearch.SelectByName(name)?.Code;

            var query = GetProvinces()
                .Where(x => StringHelper.Equals(x.Country, code))
                .OrderBy(x => x.Name);

            return query.Select(info => info.Name).ToArray();
        }

        public Province[] SelectEntitiesByCountryName(string name)
        {
            var code = _countrySearch.SelectByName(name)?.Code;

            return GetProvinces()
                .Where(x => StringHelper.Equals(x.Country, code))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public ListItemArray SelectListItemArray(string country, string language)
        {
            var array = new ListItemArray();

            var list = GetProvinces().Where(x => x.Country == country).ToList();

            foreach (var item in list)
            {
                var text = item.Name;
                if (!string.IsNullOrEmpty(item.Translations))
                {
                    var translations = MultilingualString.Deserialize(item.Translations);
                    var translation = translations[language];
                    if (!string.IsNullOrEmpty(translation))
                        text = translation;
                }

                array.Add(item.Code, text);
            }

            array.Items.Sort();

            return array;
        }

        public string Unabbreviate(string abbreviation)
        {
            var province = GetProvinces().FirstOrDefault(x => x.Code == abbreviation);
            var name = province?.Name;
            return string.IsNullOrEmpty(name) ? abbreviation : name;
        }
    }
}
