<%@ Page Language="C#" CodeBehind="Page.aspx.cs" Inherits="InSite.UI.Portal.Sites.PortalPage" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <h3 runat="server" id="RelatedPageHeading" class="rounded-top d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Related Pages</h3>

    <ul class="p-4">
        <asp:Repeater runat="server" ID="RelatedPageRepeater">
            <ItemTemplate>
                <li class="d-flex">
                    <a class="widget-link" href='<%# Eval("Url") %>'><%# Eval("Title") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:PlaceHolder runat="server" ID="BodyHolder" />

    <section class="bg-secondary py-5" runat="server" id="SupportPanel">
        <div class="container text-center">
            <h2 class="h3 pb-2 mb-4"><insite:Literal runat="server" Text="Haven't found what you're looking for? We can help." /></h2>
            <i class="far fa-life-ring d-block h2 pb-2 mb-4 text-primary"></i>
            <insite:Button runat="server" ID="HelpRequestButton" ButtonStyle="Primary" CssClass="mb-4" Text="Submit a request" />
            <p class="fs-sm mb-0"><insite:Literal runat="server" Text="Contact us and we'll get back to you as soon as possible." /></p>
        </div>
    </section>

</asp:Content>
