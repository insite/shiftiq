<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatisticsAnalyticsTable.ascx.cs" Inherits="InSite.Admin.Events.Reports.Controls.StatisticsAnalyticsTable" %>

<div class="row settings">
    <asp:Repeater runat="server" ID="GroupRepeater">
        <ItemTemplate>
            <div class="col-lg-2">

                <h5><%# Eval("Name") %></h5>

                <table class="table table-striped">

                <asp:Repeater runat="server" ID="TableRepeater" Visible="false">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Html") %></td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <tr>
                    <td></td>
                    <td class="text-end"><strong><%# Eval("Sum", "{0:n0}") %></strong></td>
                </tr>
                </table>

                <asp:PlaceHolder runat="server" ID="NoDataMessage" Visible="false">
                    <p>No Data</p>
                </asp:PlaceHolder>

            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>