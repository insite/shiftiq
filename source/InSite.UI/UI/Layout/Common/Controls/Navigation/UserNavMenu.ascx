<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserNavMenu.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.UserNavMenu" %>

<a runat="server" id="UserName" href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"></a>

<ul class="dropdown-menu dropdown-menu-end">
    <li runat="server" id="MyDashboardItem">
        <a runat="server" class="dropdown-item" id="MyDashboardLink"></a>
    </li>
    <li runat="server" id="MyProfileItem">
        <a class="dropdown-item" href="/ui/portal/profile">
            <%= GetDisplayText("My Profile") %>
        </a>
    </li>
    <li runat="server" id="SelectEnvironmentItem" visible="false">
        <a class="dropdown-item" href="/ui/portal/platform/environments">
            <%= GetDisplayText("Select Environment") %>
        </a>
    </li>
    <li runat="server" id="SelectOrganizationItem" visible="false">
        <a runat="server" id="SelectOrganizationLink" class="dropdown-item" href="/ui/portal/security/organizations">
            <%= GetDisplayText("Select Organization") %>
        </a>
    </li>
    <li runat="server" id="MyBackgrounds">
        <a class="dropdown-item" href="/ui/portal/identity/backgrounds">
            <%= GetDisplayText("Select Background") %>
        </a>
    </li>
    <li>
        <a class="dropdown-item" href="/ui/portal/identity/password">
            <%= GetDisplayText("Change Password") %>
        </a>
    </li>
    <li runat="server" visible="false">
        <a class="dropdown-item" href="/ui/portal/identity/authenticate">
            <%= GetDisplayText("Multi-Factor Authentication") %>
        </a>
    </li>
    <li runat="server" visible="false">
        <a class="dropdown-item" href="/ui/portal/web3">
            <%= GetDisplayText("Connect Your Wallet") %>
        </a>
    </li>
    <li runat="server" id="SignOutItem">
        <a class="dropdown-item" runat="server" id="SignOutLink">Sign Out</a>
    </li>
</ul>
