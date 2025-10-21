<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SummaryTables.ascx.cs" Inherits="InSite.Admin.Contacts.Memberships.Controls.SummaryTables" %>

<div class="row settings">
    <asp:Repeater runat="server" ID="TableRepeater">
        <ItemTemplate>
            <div class="col-md-3">

                <h2><%# Eval("Name") %></h2>
                
                <table class="table table-striped">

                <asp:Repeater runat="server" ID="TableRepeater" Visible="false">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Text") %></td>
                            <td class="text-end"><%# Eval("Count") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <tr>
                    <td></td>
                    <td class="text-end"><strong><%# Eval("Sum") %></strong></td>
                </tr>
                </table>

                <asp:PlaceHolder runat="server" ID="NoDataMessage" Visible="false">
                    <p>No Data</p>
                </asp:PlaceHolder>

            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>