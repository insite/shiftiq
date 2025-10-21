<%@ Page Language="C#" CodeBehind="Catalog.aspx.cs" Inherits="InSite.UI.Portal.Learning.Programs.Catalog" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/ProgramSearchControl.ascx" TagPrefix="uc" TagName="ProgramSearchControl" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <asp:Panel runat="server" ID="CategoryPanel">

        <h3 class="rounded-top d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Categories</h3>

        <div class="widget widget-categories p-4">
            <asp:Repeater runat="server" ID="CatalogRepeater">
                <HeaderTemplate>
                    <ul id="catalogs">
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <a class='widget-link <%# Eval("CatalogCollapsed") %>' href='#<%# Eval("CatalogSlug") %>' data-bs-toggle="collapse"><%# Eval("CatalogName") %><small class="text-body-secondary ps-1 ms-2"><%# Eval("CatalogSize") %></small></a>
                        <ul class="collapse <%# Eval("CategoriesShow") %>" id='<%# Eval("CatalogSlug") %>' data-bs-parent="#catalogs">
                        <asp:Repeater runat="server" ID="CategoryRepeater">
                            <ItemTemplate>
                                <li>
                                    <asp:LinkButton runat="server" ID="CategoryButton" Text='<%# Eval("CategoryName") %>'
                                        CssClass='<%# (bool)Eval("IsSelected") ? "widget-link active" : "widget-link" %>'
                                    />
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                        </ul>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    
    </asp:Panel>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <uc:ProgramSearchControl runat="server" ID="ProgramSearchControl" />

</asp:Content>