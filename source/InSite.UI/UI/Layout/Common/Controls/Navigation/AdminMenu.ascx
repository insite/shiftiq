<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMenu.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Navigation.AdminMenu" %>

<li runat="server" id="AdminMenuItem" class="nav-item dropdown fs-sm">

    <a runat="server" id="AdminMenuAnchor" href="#" class="nav-link dropdown-toggle"
        data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i class="far fa-grid-round me-1 fa-width-auto"></i>Admin</a>

    <div class="dropdown-menu dropdown-menu-end p-0">
        <div class="d-lg-flex">

            <asp:Repeater runat="server" ID="MenuGroups">
                <ItemTemplate>
                    <div class="mega-dropdown-column pt-1 pt-lg-3 pb-lg-4">

                        <ul class="list-unstyled mb-0">
                            <li><span class="fs-lg ms-3 text-secondary"><%# Eval("Title") %></span></li>
                            <asp:Repeater runat="server" ID="MenuItems">
                                <ItemTemplate>
                                    <li>
                                        <a runat="server" class="dropdown-item" href='<%# Eval("Url") %>'>
                                            <i runat="server" class='<%# Eval("Icon", "me-1 far fa-{0}") %>'></i>
                                            <%# Eval("Title") %>
                                        </a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>

                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</li>