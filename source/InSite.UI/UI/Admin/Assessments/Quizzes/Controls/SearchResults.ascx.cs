using System.ComponentModel;

using InSite.Application.Quizzes.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Assessments.Quizzes.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TQuizFilter>
    {
        protected override int SelectCount(TQuizFilter filter)
        {
            return ServiceLocator.QuizSearch.Count(filter);
        }

        protected override IListSource SelectData(TQuizFilter filter)
        {
            return ServiceLocator.QuizSearch.Select(filter).ToSearchResult();
        }

        protected string GetEditUrl()
        {
            var item = (TQuiz)Page.GetDataItem();
            return Edit.GetNavigateUrl(item.QuizIdentifier);
        }
    }
}