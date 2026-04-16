using System.ComponentModel;
using System.Web.UI;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class TaxonomyComboBox : ComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TaxonomyComboBoxSettings Settings { get; }

        public TaxonomyComboBox()
        {
            Settings = new TaxonomyComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    public class TaxonomyMultiComboBox : MultiComboBox
    {
        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TaxonomyComboBoxSettings Settings { get; }

        public TaxonomyMultiComboBox()
        {
            Settings = new TaxonomyComboBoxSettings(nameof(Settings), ViewState);
        }

        protected override ListItemArray CreateDataSource() => Settings.CreateDataSource();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TaxonomyComboBoxSettings : StateBagProxy
    {
        public TaxonomyComboBoxSettings(string prefix, StateBag viewState)
            : base(prefix, viewState)
        {
        }

        public ListItemArray CreateDataSource()
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