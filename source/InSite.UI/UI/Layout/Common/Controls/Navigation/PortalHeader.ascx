<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalHeader.ascx.cs" Inherits="InSite.UI.Layout.Portal.Controls.PortalHeader" %>

<%@ Register Src="UserNavMenu.ascx" TagName="UserNavMenu" TagPrefix="uc" %>
<%@ Register Src="AdminMenu.ascx" TagName="AdminMenu" TagPrefix="uc" %>

<style>
  .cart-badge{
    font-size:.75rem;
    min-width:1.25rem;
    line-height:1.0;
    padding:.25em .4em;
    box-shadow: 0 0 0 2px #fff;
  }
</style>

<header class="navbar portal-navbar navbar-expand-lg justify-content-end fixed-top flex-nowrap shadow-sm bg-white" data-scroll-header="">
    <div class="container">

        <a runat="server" id="HomeLink" class="navbar-brand"></a>

        <div class="ms-auto">
            <div runat="server" id="NavMenu" class="offcanvas offcanvas-end">
                <div class="offcanvas-header align-items-center border-bottom">
                    <div class="d-flex align-items-center">
                        <h4 class="offcanvas-title">Menu</h4>
                    </div>
                    <button class="btn-close" type="button" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                </div>
                <div class="offcanvas-body">
                    <ul class="navbar-nav p-0">
                        <li runat="server" id="CmdsHomeItem" visible="false" class="nav-item fs-sm">
                            <a runat="server" id="CmdsHomeLink" class="nav-link" href="#">
                                <i class="fa-regular fa-home me-1"></i>
                                CMDS
                            </a>
                        </li>
                        <li runat="server" id="PortalHomeItem" class="nav-item fs-sm">
                            <a runat="server" id="PortalHomeLink" class="nav-link" href="/ui/portal/home">
                                <i class="fa-regular fa-chalkboard-user me-1"></i>
                                <%= Translate("Portal") %>
                            </a>
                        </li>

                        <uc:AdminMenu runat="server" ID="AdminMenu" />

                        <li runat="server" id="CartItem" class="nav-item fs-sm pe-2" visible="false">
                            <a runat="server" id="CartLink" class="nav-link position-relative" href="#" title="Cart">
                                <i class="fas fa-shopping-cart fs-5"></i>

                                <span runat="server" id="CartBadge"
                                        class="badge rounded-pill bg-success position-absolute top-0 start-100 translate-middle-x mt-2 cart-badge"
                                        visible="false">0</span>
                            </a>
                        </li>
                        <li runat="server" id="UserNavItem" class="nav-item fs-sm dropdown">
                            <uc:UserNavMenu runat="server" ID="UserNav"/>
                        </li>

                        <li runat="server" id="ImpersonatorItem" class="nav-item fs-sm">
                            <a runat="server" id="ImpersonatorAnchor" class="nav-link text-danger">
                                <i class="fa-regular fa-user-secret me-2 fa-width-auto"></i>
                                <asp:Literal runat="server" ID="ImpersonatorName" />
                            </a>
                        </li>

                        <li runat="server" id="HelpMenuItem" class="nav-item fs-sm dropdown">
    
                            <a runat="server" id="HelpMenuAnchor" href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i class="fa-regular fa-question-circle me-1"></i>Help</a>

                            <ul class="dropdown-menu">
                                <insite:Container runat="server" ID="ResourcesGroupContainer">
                                    <li><h6 class="dropdown-header pt-2 pb-1">Resources</h6></li>
                                    <li><a runat="server" id="HelpAnchor" class="dropdown-item ms-2" href="#" data-bs-toggle="modal" data-bs-target="#modal-help"><%= Translate("Help") %></a></li>
                                </insite:Container>
                                <insite:Container runat="server" ID="GetHelpGroupContainer">
                                    <li><h6 class="dropdown-header pb-1">Get Help</h6></li>
                                    <li><a runat="server" id="SupportAnchor" class="dropdown-item ms-2" href="/ui/portal/support"><%= Translate("Support") %></a></li>
                                </insite:Container>
                            </ul>

                        </li>
                        <li runat="server" id="LoginItem" class="nav-item fs-sm" visible="false">
                            <a runat="server" id="LoginLink" class="nav-link" href="#">
                                <i class="fa-regular fa-sign-in-alt me-1"></i>Login
                            </a>
                        </li>
                        <li runat="server" id="LanguageItem" class="nav-item fs-sm dropdown">
            
                            <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                                <i class="fa-regular fa-globe me-1"></i>
                                <asp:Literal runat="server" ID="CurrentLanguageOutput" />
                            </a>

                            <ul class="dropdown-menu dropdown-menu-end">
                                <asp:Repeater runat="server" ID="LanguageMenuItems">
                                    <ItemTemplate>
                                        <li>
                                            <a class="dropdown-item" href='<%# Eval("Url") %>'><%# Eval("Name") %></a>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

        <button type="button" class="navbar-toggler" data-bs-toggle="offcanvas" data-bs-target="<%= "#" + NavMenu.ClientID %>" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
    </div>
</header>
