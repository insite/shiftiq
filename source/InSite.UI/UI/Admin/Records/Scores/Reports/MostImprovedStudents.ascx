<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MostImprovedStudents.ascx.cs" Inherits="InSite.Admin.Records.Scores.MostImprovedStudents" %>

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
                border-bottom: solid 1px Black;
            }

                table tr th + th {
                    padding-left: 6px;
                }

                table tr td + td {
                    padding-left: 6px;
                }

        div.trade + div.trade {
            padding-top: 40px;
        }
    </style>
</head>
<body>
    <div style="padding-bottom:20px;">
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
            All Scores Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="TradeRepeater">
        <ItemTemplate>
            <div class="trade" style="page-break-inside:avoid;">
                <h1 style="margin-bottom:5px;"><%# Eval("AchievementDescription") %></h1>

                <div style="margin-bottom:10px;">
                    Out of&nbsp;&nbsp;<b><%# Eval("TotalStudentCount") %></b> students
                </div>

                <asp:Repeater runat="server" ID="StudentRepeater">
                    <HeaderTemplate>
                        <table style="width:100%;padding:0;">
                            <thead>
                                <tr style="page-break-inside:avoid;">
                                    <th>Student</th>
                                    <th>Employer</th>
                                    <th>Started</th>
                                    <th>Completed</th>
                                    <th>Grades</th>
                                    <th>Difference</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr style="page-break-inside:avoid;">
                            <td>
                                <%# Eval("UserFullName") %> <br />
                                <%# Eval("UserEmail") %>
                            </td>
                            <td>
                                <%# Eval("EmployerName") %> <br />
                                <%# Eval("EmployerRegion") %>
                            </td>
                            <td><%# GetLocalDate(Eval("EventScheduledStart")) %></td>
                            <td><%# GetLocalDate(Eval("EventScheduledEnd")) %></td>
                            <td>
                                <asp:Repeater runat="server" ID="LevelRepeater">
                                    <ItemTemplate>
                                        <div>
                                            <%# Eval("AchievementTitle") %> : <%# Eval("Percent", "{0:p2}") %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                            <td><%# Eval("Difference", "{0:p2}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
