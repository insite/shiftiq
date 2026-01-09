using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

namespace InSite.Cmds.Admin.Profiles.Forms
{
    public partial class Search2 : SearchPage<ProfileFilter>
    {
        public override string EntityName => "Field Profile";

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            Access = Access.SetCreate(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}