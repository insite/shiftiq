<%@ Page Language="C#" CodeBehind="Catalog.aspx.cs" Inherits="InSite.UI.Portal.Learning.Catalog" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Portal/Controls/LaunchCardRepeater.ascx" TagPrefix="uc" TagName="LaunchCardRepeater" %>

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

        <div class="row">
            <div class="col-lg-12 content mb-2 mb-sm-0 pb-sm-5">

                <!-- Active filters-->
                <div runat="server" id="FilterPanel" class="d-flex flex-wrap align-items-center mb-2">
                    <asp:Repeater runat="server" ID="FilterButtonRepeater">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="FilterButton" CssClass="active-filter me-2" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <!-- Sorting -->
                <div class="d-flex justify-content-between align-items-center py-3 mb-3">

                    <div class="d-flex justify-content-center align-items-center">
                        <insite:InputFilter runat="server" ID="SearchText" EmptyMessage="Search catalog" CssClass="me-2" Width="500" />

                        <insite:ComboBox runat="server" ID="SortBySelect" CssClass="me-2" Width="200" >
                            <Items>
                                <insite:ComboBoxOption Value="title" Text="Sort by title" />
                                <insite:ComboBoxOption Value="newest" Text="Sort by newest" />
                                <insite:ComboBoxOption Value="popularity" Text="Sort by popularity" />
                                <insite:ComboBoxOption Value="rating" Text="Sort by rating" />
                            </Items>
                        </insite:ComboBox>
                        <div class="d-none d-sm-block fs-sm text-nowrap ps-1 mb-1"><insite:Literal runat="server" ID="ItemCount" /></div>
                    </div>
                </div>

                <!-- Results -->

                <uc:LaunchCardRepeater runat="server" ID="CardRepeater" />
                                
                <asp:Repeater ID="PageRepeater" runat="server">
                    <HeaderTemplate>
                        <nav><ul class='pagination pagination-lg'>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class='page-item d-none d-sm-block <%# Eval("PageStatus") %>'>
                            <asp:LinkButton ID="PageButton" runat="server" Text='<%# Eval("PageNumber") %>' CommandArgument='<%# Eval("PageNumber") %>' CssClass='page-link' />
                            <asp:Label ID="PageLabel" runat="server" Text='<%# Eval("PageNumber") %>' CssClass='page-link' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul></nav>
                    </FooterTemplate>
                </asp:Repeater>

            </div>
        </div>

</asp:Content>