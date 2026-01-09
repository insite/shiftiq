using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Talents.Competencies
{
    public partial class CompetencySearchCriteria : SearchCriteriaController<CompetencyFilter>
    {
        public override CompetencyFilter Filter
        {
            get
            {
                var filter = new CompetencyFilter
                {
                    Number = Number.Text,
                    Summary = Summary.Text,
                    NumberOld = NumberOld.Text,
                    CategoryIdentifier = Category.ValueAsGuid,
                    Description = Description.Text,
                    IsDeleted = DeletedSelector.ValueAsBoolean.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Number.Text = value.Number;
                Summary.Text = value.Summary;
                NumberOld.Text = value.NumberOld;
                Category.ValueAsGuid = value.CategoryIdentifier;
                Description.Text = value.Description;
                DeletedSelector.ValueAsBoolean = value.IsDeleted;
            }
        }

        public override void Clear()
        {
            Number.Text = null;
            Summary.Text = null;
            NumberOld.Text = null;
            Category.ValueAsGuid = null;
            Description.Text = null;
            DeletedSelector.ValueAsBoolean = false;
        }
    }
}