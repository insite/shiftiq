using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Cmds.Admin.Competencies.Forms
{
    public partial class Search : SearchPage<CompetencyFilter>
    {
        public override string EntityName => "Competency";

        public override void ApplyAccessControlForCmds()
        {
            Access = Access.SetRead(true);
            Access = Access.SetCreate(Access.Write);
            Access = Access.SetDelete(Access.Create);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                PageHelper.AutoBindHeader(
                    this,
                    new BreadcrumbItem("Add New Competency", "/ui/cmds/admin/standards/competencies/create", null, null));
        }
    }
}