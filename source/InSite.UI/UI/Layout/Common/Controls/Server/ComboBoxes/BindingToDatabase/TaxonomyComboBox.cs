using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class TaxonomyComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var taxonomies = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Assessments_Questions_Classification_Taxonomy
            });

            foreach (var taxonomy in taxonomies)
                list.Add(taxonomy.ItemSequence.ToString(), $"{taxonomy.ItemSequence}. {taxonomy.ItemName}");

            return list;
        }
    }
}