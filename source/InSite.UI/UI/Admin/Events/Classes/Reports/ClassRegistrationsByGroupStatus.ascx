<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassRegistrationsByGroupStatus.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ClassRegistrationsByGroupStatus" %>

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

            table tr th + th {
                padding-left: 6px;
            }

            table tr td + td {
                padding-left: 6px;
            }

        table.summary {
            padding: 0;
            font-weight: bold;
            margin-top: 40px;
            border-top: 1px solid grey;
        }

        div.achievement + div.achievement {
            padding-top: 40px;
        }

            div.achievement > table {
                width: 100%;
                padding: 0;
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

        .w-350 {
            width: 350px;
        }
        
        .w-100 {
            width: 100px;
        }
                
        .w-80 {
            width: 80px;
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

    <asp:Repeater runat="server" ID="AchievementRepeater">
        <ItemTemplate>
            <div class="achievement" style="page-break-inside: avoid;">
                <table>
                    <thead style="page-break-inside: avoid;">
                        <tr>
                            <th class="w-350"><%# Eval("AchievementTitle") %></th>
                            <th class="w-80 text-center">Member</th>
                            <th class="w-80 text-center">Non-Member</th>
                            <th class="w-80 text-center">No Employer</th>
                            <th class="w-80 text-center">Total</th>
                            <th class="w-80"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="ClassRepeater">
                            <ItemTemplate>
                                <tr style="page-break-inside: avoid;">
                                    <td><%# Eval("ClassTitle") %></td>
                                    <td class="text-center"><%# Eval("MemberCount") %></td>
                                    <td class="text-center"><%# Eval("NonMemberCount") %></td>
                                    <td class="text-center"><%# Eval("NoEmployerCount") %></td>
                                    <td class="text-center text-bold"><%# Eval("TotalCount") %></td>
                                    <td></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                        <tr class="text-bold" style="page-break-inside: avoid;">
                            <td class="w-350">TOTAL</td>
                            <td class="w-80 text-center"><%# Eval("MemberCount") %></td>
                            <td class="w-80 text-center"><%# Eval("NonMemberCount") %></td>
                            <td class="w-80 text-center"><%# Eval("NoEmployerCount") %></td>
                            <td class="w-80 text-center"><%# Eval("TotalCount") %></td>
                            <td class="w-80 text-center">
                                <%# Eval("MemberPercent", "{0:p0}") %>
                            </td>
                        </tr>
                     </tbody>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <table class="summary" style="page-break-inside: avoid;">
        <tr>
            <td class="w-350">Member Apprentices</td>
            <td class="w-100 text-right"><asp:Literal runat="server" ID="MemberPercent" /></td>
            <td class="w-100 text-right"><asp:Literal runat="server" ID="MemberCount" /></td>
        </tr>
        <tr>
            <td class="w-350">Non-Member Apprentices</td>
            <td class="w-100 text-right"><asp:Literal runat="server" ID="NonMemberPercent" /></td>
            <td class="w-100 text-right"><asp:Literal runat="server" ID="NonMemberCount" /></td>
        </tr>
        <tr>
            <td class="w-350">Total</td>
            <td class="w-100 text-right"></td>
            <td class="w-100 text-right"><asp:Literal runat="server" ID="TotalCount" /></td>
        </tr>
    </table>
</body>
</html>
