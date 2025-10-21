<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopStudents.ascx.cs" Inherits="InSite.Admin.Records.Scores.TopStudents" %>

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

        div.scoreItem {
            padding-bottom:20px;
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

    <asp:Repeater runat="server" ID="RegionRepeater">
        <ItemTemplate>
            <div class="region" style='<%# "page-break-before:" + (Container.ItemIndex == 0 ? "avoid" : "always")  + ";margin-top:1.34em;" %>'>
                <h1><%# Eval("EmployerRegion") %> Employer's Region</h1>
                <asp:Repeater runat="server" ID="AchievementRepeater">
                    <ItemTemplate>
                        <div class="achievement" style="page-break-inside:avoid;margin-top:1.245em;">
                            <div class="scoreItem" style="page-break-inside:avoid;">
                                <h2><%# Eval("AchievementTitle") %> Achievement</h2>

                                <asp:Repeater runat="server" ID="ScoreItemRepeater">
                                    <ItemTemplate>
                                        <div style="margin-bottom:6px;">
                                            Score Item: <b><%# Eval("ScoreItemName") %></b>
                                        </div>

                                        <asp:Repeater runat="server" ID="StudentRepeater">
                                            <HeaderTemplate>
                                                <table>
                                                    <thead>
                                                        <tr style="page-break-inside: avoid;">
                                                            <th>Student</th>
                                                            <th>Employer</th>
                                                            <th>Score</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr style="page-break-inside: avoid;">
                                                    <td><%# Eval("StudentFullName") %></td>
                                                    <td><%# Eval("EmployerName") %></td> 
                                                    <td><%# Eval("ScoreValue") %></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </tbody>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    
                                    </ItemTemplate>
                                    <SeparatorTemplate>
                                        </div><div class="scoreItem" style="page-break-inside: avoid;">
                                    </SeparatorTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>