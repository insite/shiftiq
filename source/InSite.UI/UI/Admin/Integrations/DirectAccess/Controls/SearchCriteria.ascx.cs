using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Persistence.Integration.DirectAccess;

using Shift.Common;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<IndividualFilter>
    {
        public override IndividualFilter Filter
        {
            get
            {
                var filter = new IndividualFilter
                {
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    Email = Email.Text,
                };

                if (!string.IsNullOrEmpty(IndividualKeys.Text))
                {
                    var list = new List<int>();
                    var keys = StringHelper.Split(IndividualKeys.Text);
                    foreach (var key in keys)
                        if (int.TryParse(key, out int result))
                            list.Add(result);
                    filter.IndividualKeys = list.ToArray();
                    IndividualKeys.Text = CsvConverter.ConvertListToCsvText(filter.IndividualKeys);
                }

                return filter;
            }
            set
            {
                if (value.IndividualKeys.IsNotEmpty())
                    IndividualKeys.Text = CsvConverter.ConvertListToCsvText(value.IndividualKeys);

                FirstName.Text = value.FirstName;
                LastName.Text = value.LastName;
                Email.Text = value.Email;
            }
        }

        public override void Clear()
        {
            IndividualKeys.Text = null;
            FirstName.Text = null;
            LastName.Text = null;
            Email.Text = null;
            Program.Text = null;
        }

        public void BindSearchCriteria(string keys, string first, string last, string email, string program)
        {
            IndividualKeys.Text = keys;
            FirstName.Text = first;
            LastName.Text = last;
            Email.Text = email;
            Program.Text = program;

            OnSearching();
        }
    }
}