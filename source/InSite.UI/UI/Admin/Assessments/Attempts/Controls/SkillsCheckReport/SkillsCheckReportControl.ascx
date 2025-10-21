<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SkillsCheckReportControl.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport.SkillsCheckReportControl" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>
    <style type="text/css">
        body {
            margin: 0;
            font-family: Calibri, Helvetica, Arial;
        }

        table.header-table {
            padding-bottom: 20px;
            width: 100%;
        }
        table.header-table td {
            width: 33%;
        }
        table.header-table td img {
            max-width: 100%;
        }
        table.header-table td:nth-child(2) {
            font-size: 36px;
            text-align: center;
            padding-left: 20px;
        }

        table.info-table {
            width: 100%;
            padding-bottom: 10px;
        }
        table.info-table td:first-child {
            white-space: nowrap;
        }
        table.info-table td:nth-child(2) {
            width: 100%;
            padding-left: 15px;
            font-weight: bold;
        }
        table.info-table td:last-child {
            white-space: nowrap;
            padding-left: 20px;
            padding-right: 20px;
        }

        div.congratulations {
            margin-top: 25px;
            border: 1px solid green;
            padding: 15px;
        }
        div.congratulations table td:last-child {
            padding-left: 50px;
            padding-right: 15px;
        }
        div.congratulations table td:last-child span.badge {
            font-size: 20px;
        }

        table.occupation-table {
            padding-top: 25px;
            width: 100%;
        }
        table.occupation-table td {
            vertical-align: top;
            padding: 6px 3px;
            border-bottom: 1px solid rgb(245, 245, 245);
        }
        table.occupation-table td:last-child {
            padding-left:10px;
            text-align: right;
            white-space: nowrap;
        }

        table.occupation-table .row-occupation td:first-child {
            font-weight: bold;
        }
        table.occupation-table .row-framework td:first-child {
            padding-left: 30px;
            color: #337ab7;
        }
        table.occupation-table .row-area td:first-child {
            padding-left: 60px;
            font-weight: bold;
        }
        table.occupation-table .row-competency td:first-child {
            padding-left: 90px;
        }

        span.badge {
            border-radius: 6px;
            color: #fff;
            font-size: 12px;
            display: inline-block;
            padding: 2.6px 6px
        }
        span.badge.bg-danger {
            background-color: rgb(247, 79, 120);
        }
        span.badge.bg-success {
            background-color: rgb(22, 201, 149);
        }
        span.badge.bg-warning {
            background-color: rgb(255, 177, 92);
        }

    </style>
</head>
<body>
    <table class="header-table">
        <tr>
            <td><img runat="server" id="Logo" src="/" alt="Logo" /></td>
            <td>Competency Report</td>
            <td></td>
        </tr>
    </table>

    <table class="info-table">
        <tr>
            <td>SkillsCheck Name:</td>
            <td><asp:Literal runat="server" ID="FormTitle" /></td>
            <td>Assigned by:</td>
        </tr>
        <tr>
            <td>User Name:</td>
            <td><asp:Literal runat="server" ID="UserName" /></td>
            <td><asp:Literal runat="server" ID="ManagerName" /></td>
        </tr>
        <tr>
            <td>Date Completed:</td>
            <td><asp:Literal runat="server" ID="Completed" /></td>
            <td><asp:Literal runat="server" ID="EmployerName" /></td>
        </tr>
    </table>

    <hr />

    <div class="congratulations">
        <table>
            <tr>
                <td>
                    <strong>Congratulations on completing your SkillsCheck!</strong><br />
                    Your overall score reflects your combined performance across all assessed competencies, highlighting
                    both your current strengths and areas to develop. Review the breakdown below to see how your skills  
                    align with the requirements.
                </td>
                <td>
                    <asp:Literal runat="server" ID="Score" />
                </td>
            </tr>
        </table>
    </div>

    <asp:Repeater runat="server" ID="OccupationRepeater">
        <HeaderTemplate><table class="occupation-table"></HeaderTemplate>
        <FooterTemplate></table></FooterTemplate>
        <ItemTemplate>
            <tr class="row-occupation">
                <td><%# Eval("Title") %></td>
                <td><%# GetStatus() %></td>
            </tr>
            <asp:Repeater runat="server" ID="FrameworkRepeater">
                <ItemTemplate>
                    <tr class="row-framework">
                        <td><%# Eval("Title") %></td>
                        <td><%# GetStatus() %></td>
                    </tr>
                    <asp:Repeater runat="server" ID="AreaRepeater">
                        <ItemTemplate>
                            <tr class="row-area">
                                <td><%# Eval("Title") %></td>
                                <td><%# GetStatus() %></td>
                            </tr>
                            <asp:Repeater runat="server" ID="CompetencyRepeater">
                                <ItemTemplate>
                                    <tr class="row-competency">
                                        <td><%# Eval("Title") %></td>
                                        <td><%# GetStatus() %> </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </ItemTemplate>
    </asp:Repeater>

</body>
</html>
