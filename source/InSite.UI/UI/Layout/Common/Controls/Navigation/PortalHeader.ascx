<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalHeader.ascx.cs" Inherits="InSite.UI.Layout.Portal.Controls.PortalHeader" %>

<%@ Register Src="UserNavMenu.ascx" TagName="UserNavMenu" TagPrefix="uc" %>

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
                        <li runat="server" id="AdminMenu" class="nav-item dropdown fs-sm">

                            <a runat="server" id="AdminMenuAnchor" href="#" class="nav-link dropdown-toggle"
                                data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i class="far fa-grid-round me-1"></i><%= Translate("Admin") %></a>

                            <div class="dropdown-menu dropdown-menu-end p-0">
                                <div class="d-lg-flex">
                                    <div class="mega-dropdown-column pt-1 pt-lg-3 pb-lg-4">

                                        <ul class="list-unstyled mb-0">
                                            <li><span class="fs-lg ms-3 text-secondary">Communication</span></li>
                                            <li><a runat="server" id="A1" class="dropdown-item" href="/ui/admin/home"><i class="me-1 far fa-grid-round"></i>Toolkits (Apps)</a></li>
                                            <li><a runat="server" id="AdminContactsLink" class="dropdown-item" href="/ui/admin/contacts/home"><i class="me-1 far fa-file-chart-line"></i>Contacts</a></li>
                                            <li><a runat="server" id="AdminSurveysLink" class="dropdown-item" href="/ui/admin/workflow/forms/home"><i class="me-1 far fa-check-square"></i>Forms (Surveys)</a></li>
                                            <li><a runat="server" id="AdminMessagesLink" class="dropdown-item" href="/ui/admin/messages/home"><i class="me-1 far fa-paper-plane"></i>Messages</a></li>
                                            <li><a runat="server" id="AdminSitesLink" class="dropdown-item" href="/ui/admin/sites/home"><i class="me-1 far fa-clouds"></i>Sites</a></li>
                                        </ul>

                                    </div>
                                    <div class="mega-dropdown-column pb-2 pt-lg-3 pb-lg-4">

                                        <ul class="list-unstyled mb-0">
                                            <li><span class="fs-lg ms-3 text-secondary">Learning</span></li>
                                            <li><a runat="server" id="CmdsAssessmentsLink" class="dropdown-item" href="/ui/admin/assessments/home"><i class="me-1 far fa-balance-scale"></i>Assessments</a></li>
                                            <li><a runat="server" id="CmdsCoursesLink" class="dropdown-item" href="/ui/admin/courses/home"><i class="me-1 far fa-users-class"></i>Courses</a></li>
                                            <li><a runat="server" id="CmdsEventsLink" class="dropdown-item" href="/ui/admin/events/home"><i class="me-1 far fa-calendar-alt"></i>Events</a></li>
                                            <li><a runat="server" id="CmdsRecordsLink" class="dropdown-item" href="/ui/admin/records/home"><i class="me-1 far fa-pencil-ruler"></i>Records</a></li>
                                            <li><a runat="server" id="AdminStandardsLink" class="dropdown-item" href="/ui/admin/standards/home"><i class="me-1 far fa-ruler-triangle"></i>Standards</a></li>
                                        </ul>

                                    </div>
                                    <div class="mega-dropdown-column pb-2 pt-lg-3 pb-lg-4">

                                        <ul class="list-unstyled mb-0">
                                            <li><span class="fs-lg ms-3 text-secondary">Management</span></li>
                                            <li><a runat="server" id="AdminAssetsLink" class="dropdown-item" href="/ui/admin/assets/home"><i class="me-1 far fa-inventory"></i>Assets</a></li>
                                            <li><a runat="server" id="AdminReportsLink" class="dropdown-item" href="/ui/admin/reporting"><i class="me-1 far fa-file-chart-line"></i>Reports</a></li>
                                            <li><a runat="server" id="AdminSalesLink" class="dropdown-item" href="/ui/admin/sales/home"><i class="me-1 far fa-file-invoice-dollar"></i>Sales</a></li>
                                            <li><a runat="server" id="AdminWorkflowsLink" class="dropdown-item" href="/ui/admin/workflow/home"><i class="me-1 far fa-arrow-progress"></i>Workflows</a></li>
                                        </ul>

                                    </div>
                                    <div runat="server" id="AdminMenuUtilities" class="mega-dropdown-column pb-2 pt-lg-3 pb-lg-4">

                                        <ul class="list-unstyled mb-0">
                                            <li><span class="fs-lg ms-3 text-secondary">Utilities</span></li>
                                            <li><a runat="server" id="AdminSetupLink" class="dropdown-item" href="/ui/admin/setup/home" visible="false"><i class="me-1 far fa-cogs"></i>Platform</a></li>
                                            <li><a runat="server" id="AdminSecurityLink" class="dropdown-item" href="/ui/admin/security/home" visible="false"><i class="me-1 far fa-shield"></i>Security</a></li>
                                            <li><a runat="server" id="AdminTimelineLink" class="dropdown-item" href="/ui/admin/timeline/home" visible="false"><i class="me-1 far fa-timer"></i>Timeline</a></li>
                                            <li><a runat="server" id="AdminIntegrationLink" class="dropdown-item" href="/ui/admin/integration/home" visible="false"><i class="me-1 far fa-plug"></i>Integration</a></li>
                                        </ul>

                                    </div>
                                </div>
                            </div>

                        </li>
                        <li runat="server" id="ImpersonatorItem" class="nav-item fs-sm">
                            <a class="nav-link text-warning" href="/ui/portal/identity/impersonate">
                                <i class="fa-regular fa-user-secret me-1"></i>
                                <asp:Literal runat="server" ID="ImpersonatorName" />
                            </a>
                        </li>
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
