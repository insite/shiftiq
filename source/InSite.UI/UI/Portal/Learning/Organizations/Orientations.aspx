<%@ Page Language="C#" CodeBehind="Orientations.aspx.cs" Inherits="InSite.UI.Portal.Learning.Organizations.Orientations" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        .card img { height: 130px !important; padding: 20px; }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="DummyAlert" Visible="false" />
    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <div class="row">
        <div class="col-lg-12">
            <p>
                Orientation training is available to employees and contractors for these organizations:
            </p>
        </div>
    </div>
    <div class="row row-cols-1 row-cols-lg-3 g-4">

        <asp:Repeater runat="server" ID="CardRepeater">
            <ItemTemplate>
                <div class="col">
                    <a class="card card-hover card-tile border-0 shadow" href='<%# Eval("OrganizationUrl") %>'>
                        <asp:Literal runat="server" ID="CardImage" />
                        <div class="card-body text-center">
                            <h3 class="h5 nav-heading mb-2"><%# Eval("OrganizationName") %></h3>
                            <p class="fs-sm text-body-secondary mb-2"><%# Eval("OrganizationDescription") %></p>
                        </div>
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
    </div>

</asp:Content>
