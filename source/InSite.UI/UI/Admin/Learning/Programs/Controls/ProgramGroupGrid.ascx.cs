using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Learning.Programs.Controls
{
    public partial class ProgramGroupGrid : SearchResultsGridViewController<ProgramGroupGrid.GridFilter>
    {
        [Serializable]
        public class GridFilter : Filter
        {
            public Guid ProgramIdentifier { get; set; }
            public string Keyword { get; set; }
        }

        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDeleting += Grid_DeleteCommand;
        }

        public bool LoadData(Guid programId)
        {
            Search(new GridFilter
            {
                ProgramIdentifier = programId
            });
            return HasRows;
        }

        public void SearchByKeyword(string keyword)
        {
            Search(new GridFilter
            {
                ProgramIdentifier = Filter.ProgramIdentifier,
                Keyword = keyword
            });
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var groupId = grid.GetDataKey<Guid>(e);

            ProgramStore.DeleteGroupEnrollment(Filter.ProgramIdentifier, groupId);

            SearchWithCurrentPageIndex(Filter);
        }

        protected override int SelectCount(GridFilter filter)
        {
            return ServiceLocator.ProgramSearch.CountProgramGroups(filter.ProgramIdentifier, filter.Keyword);
        }

        protected override IListSource SelectData(GridFilter filter)
        {
            return ServiceLocator.ProgramSearch
                .GetProgramGroups(filter.ProgramIdentifier, filter.Keyword, filter.Paging)
                .ToSearchResult();
        }
    }
}