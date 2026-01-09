<%@ Control Language="C#" CodeBehind="DashboardNavigation.ascx.cs" Inherits="InSite.UI.Portal.Home.Learning.Controls.DashboardNavigation" %>

<div runat="server" id="ActiveLinks" class="list-group list-group-flush border-top">

    <a class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/home">
        <i class="fas fa-home fs-lg opacity-60 me-2"></i>
        My SkillsChecks
    </a>

    <a class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/badges">
        <i class="fas fa-copy fs-lg opacity-60 me-2"></i>
        My Badges
    </a>

    <a class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/catalog">
        <i class="fas fa-check fs-lg opacity-60 me-2"></i>
        SkillsCheck Catalog
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
                const isActive = element.pathname === window.location.pathname;
                if (isActive) {
                    element.classList.add("active");
                }
            });
        })();
    </script>
</insite:PageFooterContent>