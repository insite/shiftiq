<%@ Control Language="C#" CodeBehind="DashboardNavigation.ascx.cs" Inherits="InSite.UI.Portal.Home.Management.Controls.DashboardNavigation" %>

<div runat="server" id="ActiveLinks" class="list-group list-group-flush border-top">

    <a class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/management/dashboard/home">
        <i class="fas fa-home fs-lg me-2"></i>
        Home
    </a>

    <a runat="server" id="MyContacts" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/contacts/people/search">
        <i class="fas fa-user fs-lg me-2"></i>
        My Contacts
    </a>

    <a runat="server" id="A1" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/billing/invoices">
        <i class="fas fa-file-invoice-dollar fs-lg me-2"></i>
        My Invoices
    </a>

    <a runat="server" id="MyCatalog" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/management/dashboard/catalog">
        <i class="fas fa-check fs-lg me-2"></i>
        SkillsCheck Catalog
    </a>

    <a runat="server" id="MyBadges" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/management/dashboard/reports" visible="false">
        <i class="fas fa-copy fs-lg me-2"></i>
        Reports
    </a>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            const root = document.getElementById("<%= ActiveLinks.ClientID %>");
            if (!root) {
                return;
            }
            root.querySelectorAll("a").forEach(element => {
                const isActive = element.pathname === window.location.pathname
                    || (
                        element.pathname === "/ui/portal/management/dashboard/catalog"
                        && (
                            window.location.pathname === "/ui/portal/billing/checkout"
                            || window.location.pathname === "/ui/portal/billing/cart"
                        )
                    )
                    || (
                        element.pathname === "/ui/portal/contacts/people/search"
                        && (
                            window.location.pathname === "/ui/portal/contacts/people/create"
                            || window.location.pathname === "/ui/portal/contacts/people/outline"
                        )
                    )
                    || (
                        element.pathname === "/ui/portal/billing/invoices"
                        && (
                            window.location.pathname === "/ui/portal/billing/invoice"
                       )
                    )
                    ;
                if (isActive) {
                    element.classList.add("active");
                }
            });
        })();
    </script>
</insite:PageFooterContent>