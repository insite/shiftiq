using System;
using System.ComponentModel;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<PersonFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var personCodeColumn = Grid.Columns.FindByName("PersonCode");
            personCodeColumn.HeaderText = LabelHelper.GetLabelContentText("Person Code");
        }

        protected override int SelectCount(PersonFilter filter)
        {
            return PersonCriteria.Count(filter);
        }

        protected override IListSource SelectData(PersonFilter filter)
        {
            return PersonCriteria.SelectSearchResults(filter).ToSearchResult();
        }
    }
}