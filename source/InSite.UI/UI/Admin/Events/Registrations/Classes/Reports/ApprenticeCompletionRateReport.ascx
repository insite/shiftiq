<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprenticeCompletionRateReport.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Reports.ApprenticeCompletionRateReport" %>

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

            table tr th + th {
                padding-left: 6px;
            }

            table tr td + td {
                padding-left: 6px;
            }

        tr td {
            vertical-align:top;
        }

        div.trade + div.trade {
            padding-top:40px;
        }

        div.line {
            padding-bottom:6px;
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
            All Registrations Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="TradeRepeater">
        <ItemTemplate>
            <div class="trade" style="page-break-inside: avoid;">
                <h1><%# Eval("AchievementDescription") %></h1>
                <p>Total of <b><%# Eval("StudentCount") %></b> students</p>

                <asp:Repeater runat="server" ID="TimeRepeater">
                    <ItemTemplate>
                        <div style="padding-top:15px;page-break-inside:avoid;">
                            <div style="margin-bottom:6px;"><b>
                                <%# Eval("CompletedPercent", "{0:p0}") %> (<%# Eval("CompletedCount") %>) students completed all <%# Eval("CredentialCount") %> levels in <%# Eval("Years") %> year following training
                            </b></div>

                            <asp:Repeater runat="server" ID="StudentRepeater">
                                <HeaderTemplate>
                                    <table class="students" style="width:100%;padding:0;">
                                        <thead>
                                            <tr>
                                                <th>Student</th>
                                                <th>Classes</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </tbody></table>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <tr style="page-break-inside: avoid;">
                                        <td style="border-bottom:1px solid grey;">
                                            <div class="line"><%# Eval("UserFullName") %></div>
                                            <div class="line">
                                                Started: <%# Eval("Started") %>
                                                Completed: <%# Eval("Completed") %>
                                            </div>
                                            <div>Time to Pass: <%# Eval("TimeToPass") %></div>
                                        </td>
                                        <td style="border-bottom:1px solid grey;">
                                            <asp:Repeater runat="server" ID="ClassRepeater">
                                                <ItemTemplate>
                                                    <div class="line">
                                                        Name: <%# Eval("ClassName") %>
                                                    </div>
                                                    <div class="line">
                                                        <div style="float:left; width:150px;">Start: <%# Eval("StartDate") %></div>
                                                        <div style="float:left; width:150px;">End: <%# Eval("EndDate") %></div>
                                                        <div style="float:left; width:100px; text-align:right;">Score: <%# Eval("Score") %></div>
                                                        <div style="clear:both;"></div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>