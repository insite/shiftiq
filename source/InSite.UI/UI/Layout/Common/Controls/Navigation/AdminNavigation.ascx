<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminNavigation.ascx.cs" Inherits="InSite.UI.Layout.Admin.AdminNavigation" %>

<%@ Register Src="~/UI/Layout/Common/Controls/Navigation/AdminMenu.ascx" TagName="AdminMenu" TagPrefix="uc" %>

<header class="navbar admin-navbar navbar-expand-lg justify-content-end fixed-top shadow-sm bg-white py-0 px-3 px-lg-4" data-scroll-header="">
    <div class="container-fluid">

        <div runat="server" id="HomeContainer3" class="navbar-brand d-lg-none"></div>

        <button type="button" class="navbar-toggler" data-bs-toggle="offcanvas" data-bs-target="<%= "#" + MobileMenu.ClientID %>" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        
        <nav class="collapse navbar-collapse">
            
            <ul class="navbar-nav ms-auto">

                <li runat="server" id="RecentMenu" class="nav-item dropdown fs-sm">
                    <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i class="fa-regular fa-circle-bookmark me-2 fa-width-auto"></i>Recent</a>
                </li>

                <li class="nav-item fs-sm">
                    <a runat="server" id="PortalAnchor" class="nav-link" href="/ui/portal/home"><i class="fa-regular fa-chalkboard-user me-2 fa-width-auto"></i>Portal</a>
                </li>

                <uc:AdminMenu runat="server" ID="AdminMenu" />

                <li class="nav-item dropdown fs-sm">
            
                    <a runat="server" id="UserNameAnchor" href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"></a>

                    <ul class="dropdown-menu">
                        <li runat="server" id="MyDashboardItem">
                            <a runat="server" class="dropdown-item" id="MyDashboardLink"></a>
                        </li>
                        <li>
                            <a class="dropdown-item" href="/ui/portal/profile">My Profile</a>
                        </li>
                        <li runat="server" id="SelectEnvironmentItem" visible="false">
                            <a class="dropdown-item" href="/ui/portal/platform/environments">Select Environment</a>
                        </li>
                        <li runat="server" id="SelectOrganizationItem" visible="false">
                            <a runat="server" id="SelectOrganizationLink" class="dropdown-item" href="/ui/portal/security/organizations">Select Organization</a>
                        </li>
                        <li>
                            <a class="dropdown-item" href="/ui/portal/identity/password">Change Password</a>
                        </li>
                        <li runat="server" visible="false">
                            <a class="dropdown-item" href="/ui/portal/identity/authenticate">Multi-Factor Authentication</a>
                        </li>
                        <li runat="server" visible="false">
                            <a class="dropdown-item" href="/ui/portal/web3">Connect Your Wallet</a>
                        </li>
                        <li>
                            <a class="dropdown-item" runat="server" id="SignOutLink">Sign Out</a>
                        </li>
                    </ul>

                </li>

                <li runat="server" id="ImpersonatorMenu" class="nav-item fs-sm">
                    <a runat="server" id="ImpersonatorAnchor" class="nav-link text-danger">
                        <i class="fa-regular fa-user-secret me-2 fa-width-auto"></i>
                        <asp:Literal runat="server" ID="ImpersonatorName" />
                    </a>
                </li>

                <li class="nav-item dropdown fs-sm">
    
                    <a runat="server" id="HelpMenuAnchor" href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i class="fa-regular fa-question-circle me-2 fa-width-auto"></i>Help</a>

                    <ul class="dropdown-menu dropdown-menu-end">

                        <insite:Container runat="server" ID="CmdsContainer">
                            <li>
                                <h6 class="dropdown-header pb-1">Get Help</h6>
                            </li>
                            <li><a class="dropdown-item ms-2" href="/ui/portal/support">Submit a support request</a></li>
                            <li><a class="dropdown-item ms-2" target="_blank" href="https://www.keyeracmds.com/blog">Blog posts, news, and updates</a></li>
                            <li><a class="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/terminology.pdf">Terminology</a></li>
                            <li>
                                <h6 class="dropdown-header pb-1">CMDS Guides</h6>
                            </li>
                            <li><a class="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/learner.pdf">User guide</a></li>
                            <li><a class="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/validator.pdf">Validator guide</a></li>
                            <li><a class="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/administrator.pdf">Administrator guide</a></li>
                            <li>
                                <h6 class="dropdown-header pb-1">Orientation Guides</h6>
                            </li>
                            <li><a class="dropdown-item ms-2" href="https://hub.cmds.app/lobby/docs/guides/orientation.pdf">Orientations and certificates</a></li>
                            <li runat="server" id="KeyeraHeading">
                                <h6 class="dropdown-header pb-1">Course Registration Guides</h6>
                            </li>
                            <li runat="server" id="KeyeraLinks"><a class="dropdown-item ms-2" href="https://hub.cmds.app/lobby/docs/guides/learning-catalogue-and-registration.pdf">Keyera learning catalogue &amp; registration quick reference guide</a></li>
                        </insite:Container>

                        <li><h6 class="dropdown-header pt-2 pb-1">Resources</h6></li>
                        <li><a runat="server" id="ActionHelpAnchor" class="dropdown-item ms-2" href="#" data-bs-toggle="modal" data-bs-target="#modal-help">Help</a></li>
                        <li><a runat="server" id="HelpCenterAnchor" class="dropdown-item ms-2" href="https://docs.shiftiq.com/help">Help center</a></li>
                        <li><a runat="server" id="DocumentationAnchor" class="dropdown-item ms-2" href="https://docs.shiftiq.com/">Documentation</a></li>

                        <insite:Container runat="server" ID="ShiftContainer">
                            <li><h6 class="dropdown-header pb-1">Get Help</h6></li>
                            <li><a class="dropdown-item ms-2" href="/ui/portal/support">Contact Support</a></li>
                        </insite:Container>
                        
                    </ul>

                </li>

                <li runat="server" id="LanguageItem" class="nav-item fs-sm dropdown">

                    <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                        <i class="fa-regular fa-globe me-2 fa-width-auto"></i>
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

            <script type="text/javascript">
                (function () {
                    inSite.common.recentLinks.init('<%= GetRecentLinksKey() %>', '<%= RecentMenu.ClientID %>');
                })();
            </script>

        </nav>
    </div>
</header>

<aside runat="server" id="DesktopMenu" class="position-fixed d-none d-lg-block admin-sidebar">
    
    <div class="admin-sidebar-header">
        <div class="admin-sidebar-toggle">
            <div title="Toggle Sidebar">
                <i class="fas fa-angle-left"></i>
            </div>
        </div>
        <h5 runat="server" id="HomeContainer1" class="m-0 d-inline-block">-</h5>
    </div>

    <div class="admin-simplebar-wrapper" style="height: calc(100% - 2.9rem - 0.25rem);">
        <div data-simplebar>
            <div class="admin-simplebar-content">

                <asp:Repeater runat="server" ID="SidebarCategoryRepeater">
                    <ItemTemplate>
                        <h3 runat="server" id="ItemHeader" class="h6 text-light pt-2 pb-2 border-bottom border-light compact-h3"><span class="hide-compact"><%# Eval("Category") %></spa></h3>
                        <ul class="nav flex-column">
                            <asp:Repeater runat="server" ID="ItemRepeater">
                                <ItemTemplate>
                                    <li class="nav-item">
                                        <asp:Repeater runat="server" ID="SubItemRepeater">
                                            <HeaderTemplate>
                                                <div class="dropdown dropend end-0 d-none d-lg-block">
                                                    <a class="dropdown-toggle text-light" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    </a>
                                                    <ul class="dropdown-menu position-fixed">
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                    </ul>
                                                </div>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <li>
                                                    <a class="dropdown-item"  href='<%# Eval("Href") %>'>
                                                        <i class='me-2 <%# Eval("Icon") %>'></i>
                                                        <%# Eval("Text") %>
                                                    </a>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <a class="nav-link text-light fs-sm" href='<%# Eval("Href") %>' title='<%# Eval("Text") %>'>
                                            <i class='me-2 <%# Eval("Icon") %>'></i>
                                            <span class='hide-compact'><%# Eval("Text") %></span>
                                        </a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </ItemTemplate>
                </asp:Repeater>
                            
                <nav class="widget-nav nav nav-light flex-column hide-compact" style="margin-top: 1rem; border-top: 1px solid rgba(255, 255, 255, 0.13) !important">
                    <div class="text-body-secondary fs-sm mt-3">
                        <div class="mb-1"><i class="far fa-stopwatch me-2"></i>Session <span runat="server" id="SessionTimer" class="sessionTimer"></span></div>
                        <div class="mb-1"><i class="far fa-weight-hanging me-2"></i><asp:Label runat="server" ID="PageSizeLiteral" ViewStateMode="Disabled" /> <asp:Label runat="server" ID="CreateTimeLiteral" ViewStateMode="Disabled" /></div>
                        <div class="mb-1"><i class="far fa-network-wired me-2"></i><%= Page.Request.UserHostAddress %></div>
                        <div class="mb-1"><i class="far fa-code-commit me-2"></i><asp:Literal runat="server" ID="ReleaseVersion" /></div>
                    </div>
                </nav>

            </div>
        </div>
    </div>
</aside>

<script type="text/javascript">inSite.common.adminSidebar.init();</script>

<div runat="server" id="MobileMenu" class="offcanvas offcanvas-start admin-offcanvas">
    <div class="admin-offcanvas-header">
        <div>
            <h5 runat="server" id="HomeContainer2" class="offcanvas-title">-</h5>
            <button class="btn-close btn-close-white" type="button" data-bs-dismiss="offcanvas"></button>
        </div>
    </div>
    <div class="d-flex align-items-center py-4 px-3 border-bottom border-light">
        <a class="btn btn-outline-light w-100 btn-xs me-2" href="/ui/portal/home">Portal</a>
        <a runat="server" id="SignOutLink2" class="btn btn-outline-light w-100 btn-xs mt-0" href="#">Sign Out</a>
    </div>
    <div class="admin-simplebar-wrapper" style="height: calc(100% - 7.3rem - 0.25rem);">
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .blink_me {
          animation: blinker 1s linear infinite;
        }
        @keyframes blinker {
          50% {
            opacity: 0;
          }
        }
    </style>

    <script defer type="text/javascript">
        (function () {
            const instance = window.sessionTime = Object.freeze({
                startTimer: start,
                resetTimer: () => reset()
            });
            const minuteInMs = 60 * 1000;
            const sessionTime = <%= HttpContext.Current.Session.Timeout %> * minuteInMs;
            const blinkTime = 5 * minuteInMs;
            const storageItem = 'inSite.page.loadTime';

            let loadTime = Date.now();
            let outputs = null;
            let isBlinking = false;
            let updateHandler = null;

            function start() {
                if (outputs !== null)
                    return;

                outputs = document.querySelectorAll('#<%= SessionTimer.ClientID %>,.<%= SessionTimer.ClientID %>');

                reset(loadTime);
            }

            function update() {
                let time = sessionTime - Date.now() + loadTime;

                if (time <= 0) {
                    time = 0;
                    clearInterval(updateHandler);
                    updateHandler = null;
                }

                if (!isBlinking && time <= blinkTime) {
                    updateOutput(output => output.classList.add('blink_me', 'text-danger'));
                    isBlinking = true;
                }

                const minutes = Math.floor(time / minuteInMs);
                const seconds = Math.floor((time - minutes * minuteInMs) / 1000);
                const value = ('00' + String(minutes)).slice(-2) + ":" + ('00' + String(seconds)).slice(-2);

                updateOutput(output => output.innerHTML = value);
            }

            function updateOutput(action) {
                for (var i = 0; i < outputs.length; i++)
                    action(outputs[i]);
            }

            function reset(time, save) {
                loadTime = time ?? Date.now();

                if (save !== false)
                    window.localStorage.setItem(storageItem, String(loadTime));

                if (isBlinking) {
                    updateOutput(output => output.classList.remove('blink_me', 'text-danger'));
                    isBlinking = false;
                }

                if (!updateHandler)
                    updateHandler = setInterval(update, 1000);

                update();
            }

            $(window).on('storage', function () {
                const value = parseInt(window.localStorage.getItem(storageItem));
                if (value && !isNaN(value) && loadTime < value)
                    reset(value, false);
            });
        })();
    </script>

</insite:PageHeadContent>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">
        {
            const mobileMenu = document.querySelector('#<%= MobileMenu.ClientID %> .admin-simplebar-wrapper');
            mobileMenu.innerHTML =
                '<div data-simplebar><div class="admin-simplebar-content">' +
                document.querySelector('#<%= DesktopMenu.ClientID %> .admin-simplebar-content').innerHTML +
                '</div></div>';

            const elementsWithId = mobileMenu.querySelectorAll('[id]');
            for (var i = 0; i < elementsWithId.length; i++) {
                const el = elementsWithId[i];
                const id = el.id;

                el.removeAttribute('id');
                el.classList.add(id);
            }
        }

        if (sessionTime?.startTimer) {
            sessionTime.startTimer();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(() => sessionTime.resetTimer());
        }

        $(function () {
            var outputs = document.querySelectorAll('#<%= PageSizeLiteral.ClientID %>,.<%= PageSizeLiteral.ClientID %>');
            if (outputs.length == 0)
                return;

            var pagebytes = document.getElementsByTagName('HTML')[0].innerHTML.length;
            var kbytes = Math.round(pagebytes / 1024);

            for (var i = 0; i < outputs.length; i++)
                outputs[i].innerText = String(kbytes) + 'KB';
        });
    </script>
</insite:PageFooterContent>