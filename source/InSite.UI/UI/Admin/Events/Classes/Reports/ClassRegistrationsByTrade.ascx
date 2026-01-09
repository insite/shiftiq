<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassRegistrationsByTrade.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ClassRegistrationsByTrade" %>

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
    </style>
</head>
<body>
    <div class="criteria">
        <asp:Repeater runat="server" ID="SearchCriteriaRepeater">
            <HeaderTemplate>
                <div>Search Criteria</div>
            </HeaderTemplate>
            <ItemTemplate>
                <div>
                    <%# Eval("Name") %> = <%# Eval("Value") %>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    
        <div runat="server" id="NoCriteriaPanel">
            All Classes Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="TradeRepeater">
        <ItemTemplate>
            <div class="achievement">
                <div class="header">
                    Classes for: <%# Eval("AchievementDescription") %>
                </div>
                <table>
                    <thead>
                        <tr>
                            <th style="width:300px">Class</th>
                            <th style="width:150px">Instructors</th>
                            <th style="width:200px">Program</th>
                            <th style="width:100px">Start</th>
                            <th style="width:100px">End</th>
                            <th style="width:30px">Reg</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="ClassRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("ClassTitle") %></td>
                                    <td><%# Eval("ClassInstructors") %></td>
                                    <td><%# Eval("AchievementTitle") %></td>
                                    <td><%# Eval("ClassStart") %></td>
                                    <td><%# Eval("ClassEnd") %></td>
                                    <td class="text-right"><%# Eval("RegistrationCount") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr class="line-top">
                            <td colspan="2"><b><%# Eval("AchievementDescription") %></b></td>
                            <td>Totals: <b><%# Eval("ClassCount") %></b> Classes</td>
                            <td colspan="3" class="text-right"><b><%# Eval("RegistrationCount") %></b> Registrations</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
