using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Controls.Contacts.Companies
{
    public partial class CompanySearchCriteria : SearchCriteriaController<CompanyFilter>
    {
        public override CompanyFilter Filter
        {
            get
            {
                var filter = new CompanyFilter 
                { 
                    Name = Name.Text,
                    Archived = Archived.Value.ToEnum(InclusionType.Include)
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                var filter = value;

                Name.Text = filter.Name;
                Archived.Value = filter.Archived.GetName(InclusionType.Include);
            }
        }

        public override void Clear()
        {
            Name.Text = null;

            // Admins are almost always looking for organizations that are still open, so the
            // cleared state selects Open/Active rather than the blank any-status option. This
            // matches the default CompanyFilter already carries in its own constructor.

            Archived.Value = InclusionType.Exclude.GetName();
        }
    }
}