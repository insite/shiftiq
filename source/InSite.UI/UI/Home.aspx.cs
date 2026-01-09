using System;

using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.CMDS
{
    public partial class Home : AdminBasePage
    {
        protected BaseUserControl HomeControl { get; set; }

        protected string ControlType { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Navigator.DefaultToCmds(Identity))
            {
                HomeControl = (BaseUserControl)LoadControl("./HomeCmds.ascx");
            }
            else if (Navigator.DefaultToAdmin(Identity))
            {
                HomeControl = (BaseUserControl)LoadControl("./HomeAdmin.ascx");
            }
            else
            {
                HomeControl = (BaseUserControl)LoadControl("./HomePortal.ascx");
            }

            HomeContainer.Controls.Add(HomeControl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);

            if (HomeControl is HomeCmds)
            {
                PageHelper.OverrideTitle(this, "Competency Management and Development System");
            }
            else if (HomeControl is HomeAdmin)
            {
                PageHelper.OverrideTitle(this, "Toolkits");
            }
            else
            {
                PageHelper.OverrideTitle(this, "Portal");

                if (!IsPostBack && Page.Master is PortalMaster m)
                {
                    m.HideBreadcrumbsAndTitle();
                    m.SidebarVisible(false);
                }
            }
        }
    }
}