<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeAdminPanel.ascx.cs" Inherits="InSite.UI.Individual.HomeAdminPanel" %>

<div class="row row-cols-1 row-cols-md-6 g-4">

    <asp:Repeater runat="server" ID="AdminRepeater">
        <ItemTemplate>

            <div class="col">
                <a class="card card-hover card-tile border-0 shadow" href='<%# Eval("Url") %>'>
                    <div class="card-body text-center">
                        <asp:Label runat="server" ID="CardIcon" />
                        <asp:Literal runat="server" ID="CardTitle" />
                        <asp:Literal runat="server" ID="CardSummary" />
                    </div>
                </a>
            </div>
            
        </ItemTemplate>
    </asp:Repeater>

</div>