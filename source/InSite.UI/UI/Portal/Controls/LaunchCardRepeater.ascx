<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LaunchCardRepeater.ascx.cs" Inherits="InSite.UI.Portal.Controls.LaunchCardRepeater" %>

<div runat="server" id="SummaryPanel" class="text-body-secondary mb-3"></div>

<div class="row">
    <asp:Repeater runat="server" ID="CardRepeater">
        <ItemTemplate>

            <div class="col-lg-<%= ColumnSize %> col-sm-6 mb-4">
                <a class="card card-hover card-tile border-0 shadow h-100" href='<%# Eval("Url") %>' target='<%# Eval("Target") %>'>
                    <asp:Literal runat="server" ID="Category" />
                    <asp:Literal runat="server" ID="Flag" />
                    <asp:Literal runat="server" ID="Image" />
                    <div class="card-body text-center">
                        <asp:Literal runat="server" ID="Icon" />
                        <h3 runat="server" id="Title" class="h5 nav-heading mb-2"></h3>
                        <div runat="server" id="Summary" class="fs-sm text-body-secondary mb-2"></div>
                        <div runat="server" id="Progress" />
                    </div>
                </a>
            </div>

        </ItemTemplate>
    </asp:Repeater>
</div>