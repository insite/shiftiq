<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassRegistrationsByAchievements.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ClassRegistrationsByAchievements" %>

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

            table tr td {
                vertical-align: top;
                padding-left: 6px;
            }
                        
            table tr.line-top td {
                border-top: 1px solid grey;
            }

        div.achievement + div.achievement {
            padding-top:40px;
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
        
        .w-250 {
            width: 200px;
        }
        
        .w-200 {
            width: 200px;
        }
        
        .w-100 {
            width: 100px;
        }
                
        .w-50 {
            width: 50px;
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
                            <th class="w-350">Class</th>
                            <th class="w-200">Instructors</th>
                            <th class="w-100">Start</th>
                            <th class="w-100">End</th>
                            <th class="w-50">Registrations</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="ClassRepeater">
                            <ItemTemplate>
                                <tr style="page-break-inside: avoid;">
                                    <td><%# Eval("ClassTitle") %></td>
                                    <td><%# Eval("ClassInstructors") %></td>
                                    <td><%# Eval("ClassStart") %></td>
                                    <td><%# Eval("ClassEnd") %></td>
                                    <td class="text-right"><%# Eval("RegistrationCount") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr class="line-top" style="page-break-inside: avoid;">
                            <td><b><%# Eval("AchievementTitle") %></b></td>
                            <td>Totals: <b><%# Eval("ClassCount") %></b> Classes</td>
                            <td class="text-right" colspan="3"><b><%# Eval("RegistrationCount") %></b> Registrations</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
