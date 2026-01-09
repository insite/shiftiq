<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassCountsByTrade.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ClassCountsByTrade" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="us-ascii">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>
    <style type="text/css">
        body {
            margin: 0;
            font-family: Calibri, Helvetica, Arial;
        }

        table {
            border-bottom: 1px solid grey;
            border-collapse: collapse;
        }

            table thead tr th {
                border: none;
                border-top: 1px solid grey;
                border-bottom: 1px solid grey;
                text-align: left;
            }

            table tr.line-top td {
                border-top: 1px solid grey;
            }

            table tr th + th {
                padding-left: 6px;
            }

            table tr td + td {
                padding-left: 6px;
            }

        div.achievement + div.achievement {
            padding-top:40px;
        }

        div.achievement > table {
            width: 100%;
            padding: 0;
        }

        div.achievement .header {
            margin-bottom: 6px;
            font-weight: bold;
        }

        div.criteria {
            padding-bottom: 20px;
        }

        .text-center {
            text-align: center;
        }
        
        .text-right {
            text-align: right;
        }

        .text-bold {
            font-weight: bold;
        }

        .w-140 {
            width: 140px;
        }
        
        .w-40 {
            width: 40px;
        }
                
        .w-30 {
            width: 30px;
        }
    </style>
</head>
<body>
    <div class="criteria">
        <asp:Repeater runat="server" ID="SearchCriteriaRepeater">
            <HeaderTemplate>
                <div>Search Criteria</div>
            </HeaderTemplate>
            <ItemTemplate>
                <div><%# Eval("Name") %> = <%# Eval("Value") %></div>
            </ItemTemplate>
        </asp:Repeater>
    
        <div runat="server" id="NoCriteriaPanel">
            All Classes Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="TradeRepeater">
        <ItemTemplate>
            <div class="achievement" style="page-break-inside: avoid;">
                <div class="header">
                    <%# Eval("AchievementDescription") %>
                </div>
                <table>
                    <thead style="page-break-inside: avoid;">
                        <tr>
                            <th class="w-30 text-center">#</th>
                            <th class="w-140">Time frame</th>

                            <asp:Repeater runat="server" ID="AchievementRepeater">
                                <ItemTemplate>
                                    <th class="w-140" style="text-align:center;">
                                        <%# Container.DataItem %>
                                    </th>
                                </ItemTemplate>
                            </asp:Repeater>

                            <th class="w-40"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="TimeFrameRepeater">
                            <ItemTemplate>
                                <tr style="page-break-inside: avoid;">
                                    <td class="text-center"><%# Eval("Sequence") %></td>
                                    <td><%# Eval("TimeFrame") %></td>
                                    <asp:Repeater runat="server" ID="AchievementRegistrationRepeater">
                                        <ItemTemplate>
                                            <td class="text-center">
                                                <%# Container.DataItem %>
                                            </td>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <td class="text-center text-bold">
                                        <%# Eval("Total") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr class="line-top" style="page-break-inside: avoid;">
                            <td></td>
                            <td class="text-bold">TOTAL</td>

                            <asp:Repeater runat="server" ID="TotalRepeater">
                                <ItemTemplate>
                                    <td class="text-center text-bold"><%# Container.DataItem %></td>
                                </ItemTemplate>
                            </asp:Repeater>

                            <td class="text-center text-bold">
                                <asp:Literal runat="server" ID="Total" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <div class="achievement" style="page-break-inside: avoid;">
        <table>
            <tr>
                <td class="w-30"></td>
                <td class="w-140 text-bold">COMBINED TOTAL</td>
                <td class="w-140"></td>
                <td class="w-140"></td>
                <td class="w-140"></td>
                <td class="w-40 text-center text-bold">
                    <asp:Literal runat="server" ID="Total" />
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
