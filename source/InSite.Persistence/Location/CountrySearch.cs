using System;
using System.Linq;

using Shift.Common;
using Shift.Common.Integration.Google;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class CountrySearch
    {
        private readonly string _engineApiUrl;

        private Country[] _countries;

        public CountrySearch(string engineApiUrl)
        {
            _engineApiUrl = engineApiUrl;
        }

        private Country[] GetCountries()
        {
            Refresh();
            return _countries;
        }

        private void Refresh()
        {
            if (_countries != null)
                return;

            var client = new LocationClient();

            _countries = client.CollectCountries(_engineApiUrl);
        }

        public int Count(string searchText)
        {
            var query = GetCountries().AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));

            return query.Count();
        }

        public ListItemArray Select(string searchText, Paging paging, bool includeNA = true, bool valueAsIdentifier = true, bool canadaFirst = false, bool valueAsCode = false)
        {
            var list = new ListItemArray();

            if (includeNA)
            {
                if (!valueAsCode)
                {
                    list.Add(new ListItem() { Text = "Canada", Value = "Canada" });
                    list.Add(new ListItem() { Text = "United States", Value = "United States" });
                }
                else
                {
                    list.Add(new ListItem() { Text = "Canada", Value = "CA" });
                    list.Add(new ListItem() { Text = "United States", Value = "US" });
                }

                var separator = list.Add("- - - - - - -");
                separator.Enabled = false;
            }

            var query = GetCountries().AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));

            if (canadaFirst)
            {
                query = query
                    .OrderBy(x => x.Name == "Canada" ? 0 : (x.Name == "United States" ? 1 : 2))
                    .ThenBy(x => x.Name);
            }
            else
                query = query.OrderBy(x => x.Name);

            query = query.ApplyPaging(paging);

            if (valueAsCode)
                return GetValueAsCode(list, query);

            return GetValue(valueAsIdentifier, list, query);
        }

        private static ListItemArray GetValueAsCode(ListItemArray list, IQueryable<Country> query)
        {
            foreach (var info in query)
                list.Add(new ListItem()
                {
                    Text = info.Name,
                    Value = info.Code
                });

            return list;
        }

        private static ListItemArray GetValue(bool valueAsIdentifier, ListItemArray list, IQueryable<Country> query)
        {
            foreach (var info in query)
                list.Add(new ListItem()
                {
                    Text = info.Name,
                    Value = (valueAsIdentifier ? info.Identifier.ToString() : info.Name)
                });

            return list;
        }

        public Country SelectByCode(string code) =>
            GetCountries().FirstOrDefault(x => StringHelper.Equals(x.Code, code));

        public Country SelectByName(string name) =>
            GetCountries().FirstOrDefault(x => StringHelper.Equals(x.Name, name));

        public Country SelectById(Guid? id)
        {
            if (!id.HasValue)
                return null;

            return GetCountries().FirstOrDefault(x => x.Identifier == id.Value);
        }
    }
}
