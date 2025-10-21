<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntryDetail.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.EntryDetail" %>

<div class="row">
    <div class="col-md-6">

        <asp:Repeater runat="server" ID="FieldRepeater1">
            <ItemTemplate>
                <div class="mb-3">
                    <h5 class="mt-2 mb-1">
                        <%# Eval("Title") %>
                    </h5>
                    <div>
                        <insite:DynamicControl runat="server" ID="Value" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
    <div class="col-md-6">

        <asp:Repeater runat="server" ID="FieldRepeater2">
            <ItemTemplate>
                <div class="mb-3">
                    <h5 class="mt-2 mb-1">
                        <%# Eval("Title") %>
                    </h5>
                    <div>
                        <insite:DynamicControl runat="server" ID="Value" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>