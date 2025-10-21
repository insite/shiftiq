using System;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Learning.Categories.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<TCollectionItemFilter>
    {
        protected override int SelectCount(TCollectionItemFilter filter)
        {
            return TCollectionItemSearch.Count(filter);
        }

        protected override IListSource SelectData(TCollectionItemFilter filter)
        {
            filter.OrderBy = "ItemFolder, ItemName";

            return TCollectionItemSearch
                .Select(filter, x => x.Achievements, x => x.Courses, x => x.Programs)
                .Select(x => new CategorySearchResult
                {
                    ItemFolder = x.ItemFolder,
                    ItemName = x.ItemName,
                    ItemIdentifier = x.ItemIdentifier,
                    AchievementCount = x.Achievements.Count,
                    CourseCount = x.Courses.Count,
                    ProgramCount = x.Programs.Count
                })
                .ToList()
                .ToSearchResult();
        }

        protected string GetEditUrl()
        {
            var item = (CategorySearchResult)Page.GetDataItem();
            return Edit.GetNavigateUrl(item.ItemIdentifier);
        }
    }

    public class CategorySearchResult
    {
        public string ItemFolder { get; set; }
        public string ItemName { get; set; }
        public Guid ItemIdentifier { get; set; }
        public int AchievementCount { get; set; }
        public int CourseCount { get; set; }
        public int ProgramCount { get; set; }
    }
}