<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PassingRate.ascx.cs" Inherits="InSite.Admin.Records.Scores.PassingRate" %>

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
            border-bottom:1px solid grey;
            border-collapse: collapse;
        }

            table thead tr th {
                border: none;
                border-top: 1px solid grey;
                border-bottom: 1px solid grey;
                padding-left: 5px;
                padding-right: 5px;
                text-align: left;
            }

            table tr td {
                text-align: center;
            }

            table tr th + th {
                padding-left: 6px;
            }

            table tr td + td {
                padding-left: 6px;
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

    <asp:Repeater runat="server" ID="AchievementRepeater">
        <ItemTemplate>
            <div class="activity" style="page-break-inside:avoid;">
                <h1><%# Eval("AchievementTitle") %></h1>

                <asp:Repeater runat="server" ID="ScoreItemRepeater">
                    <HeaderTemplate>
                        <table>
                            <thead>
                                <tr style="page-break-inside:avoid;">
                                    <th>Item Name</th>
                                    <th>Students</th>
                                    <th>70% AND above</th>
                                    <th>Passign Rate</th>
                                </tr>
                            </thead>
                            <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr style="page-break-inside:avoid;">
                                <td style="text-align:left"><%# Eval("ScoreItemName") %></td>
                                <td><%# Eval("StudentsTotal") %></td>
                                <td><%# Eval("StudentsAbove70") %></td>
                                <td><%# Eval("PassingRate", "{0:p0}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>