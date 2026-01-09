using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Collections.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TCollectionFilter>
    {
        #region Properties

        public override TCollectionFilter Filter
        {
            get
            {
                var filter = new TCollectionFilter
                {
                    CollectionName = CollectionName.Text,
                    CollectionTool = CollectionTool.Text,
                    CollectionProcess = CollectionProcess.Text,
                    CollectionType = CollectionType.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CollectionName.Text = value.CollectionName;
                CollectionTool.Text = value.CollectionTool;
                CollectionProcess.Text = value.CollectionProcess;
                CollectionType.Text = value.CollectionType;
            }
        }

        #endregion

        #region Operations

        public override void Clear()
        {
            CollectionName.Text = null;
            CollectionTool.Text = null;
            CollectionProcess.Text = null;
            CollectionType.Text = null;
        }

        #endregion
    }
}