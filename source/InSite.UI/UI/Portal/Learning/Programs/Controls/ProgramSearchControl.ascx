<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramSearchControl.ascx.cs" Inherits="InSite.UI.Portal.Learning.Programs.Controls.ProgramSearchControl" %>

<%@ Register Src="./ProgramCardRepeater.ascx" TagPrefix="uc" TagName="ProgramCardRepeater" %>

<insite:Alert runat="server" ID="CatalogAlert" />

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
                    </Items>
                </insite:ComboBox>
                <div class="d-none d-sm-block fs-sm text-nowrap ps-1 mb-1"><insite:Literal runat="server" ID="ItemCount" /></div>
            </div>
        </div>

        <!-- Results -->

        <uc:ProgramCardRepeater runat="server" ID="CardRepeater" />
                                
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