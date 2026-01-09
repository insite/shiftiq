using System;
using System.Linq;

using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home
{
    public partial class Programs : PortalBasePage
    {
        private CatalogItemFilter Filter
        {
            get
            {
                if (ViewState[nameof(CatalogItemFilter)] == null)
                    ViewState[nameof(CatalogItemFilter)] = new CatalogItemFilter();

                return (CatalogItemFilter)ViewState[nameof(CatalogItemFilter)];
            }
        }

        private VCatalogProgramSearch _programSearch;
        private VCatalogProgramSearch ProgramSearch
        {
            get
            {
                if (_programSearch == null)
                {
                    var groupIds = Identity.Groups.Select(x => x.Identifier).ToArray();
                    _programSearch = VCatalogProgramSearch.Create(Identity.Organization.Identifier, null, groupIds, true);
                }

                return _programSearch;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ProgramSearchControl.SetSearch(ProgramSearch, Filter);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            ProgramSearchControl.LoadData(null, null, true);
        }
    }
}