<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AverageAgeByAchievements.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Reports.AverageAgeByAchievements" %>

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
            padding-top: 40px;
        }

        div.achievement > table {
            width: 100%;
            padding: 0;
        }

        div.criteria {
            padding-bottom: 20px;
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
            All Registrations Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="AchievementRepeater">
        <ItemTemplate>
            <div class="achievement" style="page-break-inside: avoid;">
                <table>
                    <thead>
                        <tr>
                            <th style="width:350px">Class</th>
                            <th style="width:200px">Student</th>
                            <th style="width:85px">Birthday</th>
                            <th style="width:85px">Age</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="RegistrationRepeater">
                            <ItemTemplate>
                                <tr style="page-break-inside: avoid;">
                                    <td><%# Eval("ClassTitle") %></td>
                                    <td><%# Eval("UserFullName") %></td>
                                    <td><%# Eval("BirthDate", "{0:yyyy-MM-dd}") %></td>
                                    <td><%# Eval("Age") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr class="line-top" style="page-break-inside: avoid;">
                            <td><b><%# Eval("AchievementTitle") %></b></td>
                            <td>Totals: <b><%# Eval("RegistrationCount") %></b> students</td>
                            <td colspan="2" style="text-align:right;">Average age <b><%# Eval("AverageAge") %></b> years</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
