using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class DifficultyComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var difficulties = TCollectionItemCache.Select(new TCollectionItemFilter 
            { 
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Assessments_Questions_Classification_Difficulty
            });

            foreach (var difficulty in difficulties)
                list.Add(difficulty.ItemSequence.ToString(), $"{difficulty.ItemSequence}. {difficulty.ItemName}");

            return list;
        }
    }
}