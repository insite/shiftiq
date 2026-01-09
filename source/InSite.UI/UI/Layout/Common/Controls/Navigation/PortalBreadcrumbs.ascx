<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalBreadcrumbs.ascx.cs" Inherits="InSite.UI.Layout.Portal.Controls.PortalBreadcrumbs" %>

<nav aria-label="breadcrumb">
    <a runat="server" id="ActionHelp" visible="false" href="#" class="fs-sm fw-medium ps-md-4 text-nowrap float-end" target="_blank" rel="noopener">
        <asp:Literal runat="server" ID="ActionHelpTitle" Text="Help" />
        <i class="far fa-chevron-right ms-1"></i>
    </a>
    <ol runat="server" id="BreadcrumbList" class="breadcrumb">
        <insite:Container runat="server" id="Breadcrumb">
            <li class="breadcrumb-item"><a runat="server" id="RootItem" href="/ui/portal/home">Portal</a></li>
            <asp:Repeater runat="server" ID="BreadcrumbRepeater">
                <ItemTemplate>
                    <li class='breadcrumb-item <%# Eval("Active") %>' aria-current="page"><%# Eval("Anchor") %></li>
                </ItemTemplate>
            </asp:Repeater>
        </insite:Container>
        <li runat="server" id="GoToCalendar" visible="false" class="ms-5"><a runat="server" id="Calendar" href="/ui/portal/events/calendar"><i class="fas fal fa-calendar-alt me-1"></i>Calendar</a></li>
        <li runat="server" id="AddNewItem" visible="false" class="ms-5"><a runat="server" id="AddNewAnchor" href="#"></a></li>
        <li runat="server" id="AddNewList" visible="false" class="ms-5">
            <div class="btn-group" role="group" aria-label="button group with nested dropdown">
                <div class="btn-group" role="group">
                    <span type="button" class="dropdown-toggle" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i class="fas fa-plus-circle"></i>
                        <insite:Literal runat="server" Text="Add New" />
                    </span>
                    <div class="dropdown-menu">
                        <asp:Repeater runat="server" ID="AddNewAnchors">
                            <ItemTemplate>
                                <a href='<%# Eval("Href") %>' class="dropdown-item"><%# Eval("Text") %></a>
                            </ItemTemplate>
                        </asp:Repeater>
                      
                    </div>
                </div>
            </div>
        </li>
    </ol>

    <script type="text/javascript">
        (function () {
            const root = document.getElementById('<%= BreadcrumbList.ClientID %>');
            const firstItem = root.querySelector(':scope > li');

            if (!firstItem)
                root.remove();
            else if (firstItem.classList.contains('ms-5'))
                firstItem.classList.remove('ms-5');
        })();
    </script>

</nav>

<div runat="server" id="TitlePanel" class="d-md-flex justify-content-between pb-2">
    <h1 class="me-3">
        <asp:Literal runat="server" ID="ActionTitle" />
        <span runat="server" id="ActionSubtitle" class="d-block fw-normal fs-sm text-body-secondary text-md-nowrap pt-2"></span>
    </h1>
</div>