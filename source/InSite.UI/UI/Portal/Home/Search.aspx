<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Home.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <div class="d-flex justify-content-center align-items-center">
            <div class="hstack">
                <insite:InputFilter runat="server" ID="SearchText" EmptyMessage="Ask your question..." CssClass="me-2" />

                <insite:ComboBox runat="server" ID="SortBySelect" CssClass="w-50">
                    <Items>
                        <insite:ComboBoxOption Value="title" Text="Sort by title" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
    </div>

    <div runat="server" id="NoResults" class="alert alert-warning" role="alert">
        No results found
    </div>

    <div runat="server" id="FoundDiv" class="mb-3">
        <b>
            <asp:Literal runat="server" ID="FoundLiteral" />
        </b>
    </div>

    <asp:Repeater runat="server" ID="ResultRepeater">
        <ItemTemplate>
            <div class='<%# Container.ItemIndex == 0 ? "border-bottom": "pt-grid-gutter border-bottom" %>' >
                <div class="d-sm-flex align-items-center mb-2 pb-1">
                    <div class="d-flex align-items-center">
                        <h6 class="nav-heading mb-0">
                            <a href='<%# GetPageUrl() %>'>
                                <%# Eval("WebPageTitle") %>
                            </a>
                        </h6>
                    </div>
                </div>
                <p class="fs-md">
                    <%# Eval("ContentSnip") %>
                </p>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>