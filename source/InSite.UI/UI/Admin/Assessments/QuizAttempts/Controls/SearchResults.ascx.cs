using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Application.QuizAttempts.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Assessments.QuizAttempts.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TQuizAttemptFilter>
    {
        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            if (name == "Delete")
            {
                var id = Grid.GetDataKey<Guid>(row);
                ServiceLocator.QuizAttemptStore.Delete(id);
                SearchWithCurrentPageIndex(Filter);
            }
            else
            {
                base.OnRowCommand(row, name, argument);
            }
        }

        protected override int SelectCount(TQuizAttemptFilter filter)
        {
            return ServiceLocator.QuizAttemptSearch.Count(filter);
        }

        protected override IListSource SelectData(TQuizAttemptFilter filter)
        {
            return ServiceLocator.QuizAttemptSearch
                .Select(filter, x => x.LearnerUser, x => x.Quiz)
                .ToSearchResult();
        }

        protected string GetViewUrl()
        {
            var item = (TQuizAttempt)Page.GetDataItem();
            return View.GetNavigateUrl(item.AttemptIdentifier);
        }
    }
}