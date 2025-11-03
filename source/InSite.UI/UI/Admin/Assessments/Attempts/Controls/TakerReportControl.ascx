<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TakerReportControl.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.TakerReportControl" %>

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

        table.logo-and-address {
            margin-top: 0;
            margin-bottom: 30px;
            width: 100%;
        }
        table.logo-and-address td {
            width: 50%;
            vertical-align: top;
        }
        table.logo-and-address img {
            width: 400px;
        }
        table.logo-and-address div {
            float: right;
        }

        .header {
            font-weight: bold;
            font-size: x-large;
            margin-bottom: 12px;
            color: #000;
        }

        table.data {
            border: 1px solid grey;
            width: 100%;
            border-collapse: collapse;
        }

        table.data + table.data {
            margin-top: 20px;
        }

        table.data th, table.data td {
            padding: 6px;
            border: 1px solid grey;
        }

        table.data tr th {
            background-color: #b9cfd9;
            font-weight: bold;
            text-align: left;
        }

        table.data tr + tr th {
            background-color: #d5e1e7;
        }

        table.info td {
            width: 17%;
            font-weight: bold;
        }
        table.info td + td {
            width: 83%;
            font-weight: normal;
        }

        table.results td {
            width: 21%;
        }
        table.results td:first-child {
            width: 37%;
        }

    </style>
</head>
<body>
    <asp:Repeater runat="server" ID="AttemptRepeater">
        <SeparatorTemplate>
            <div style="page-break-after: always;"></div>
        </SeparatorTemplate>
        <ItemTemplate>

            <table class="logo-and-address">
                <tr>
                    <td>
                        <img runat="server" id="Logo" src="#" alt="" />
                    </td>
                    <td>
                        <div>
                            <%# GetAddress() %>
                        </div>
                    </td>
                </tr>
            </table>

            <div class="header">
                <%# Translate("TakerReport.Title") %>
            </div>

            <table class="data info">
                <thead>
                    <tr>
                        <th colspan="2"><%# Translate("Applicant Information") %></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><%# Translate("Person Code") %></td>
                        <td><%# Eval("PersonCode") %></td>
                    </tr>
                    <tr>
                        <td><%# Translate("Full Legal Name") %></td>
                        <td><%# Eval("FullName") %></td>
                    </tr>
                    <tr>
                        <td><%# Translate("Date of Birth") %></td>
                        <td><%# Eval("Birthdate") %></td>
                    </tr>
                </tbody>
            </table>

            <table class="data info">
                <thead>
                    <tr>
                        <th colspan="2"><%# Translate("Exam Information") %></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><%# Translate("Title") %></td>
                        <td><%# Eval("ExamTitle") %></td>
                    </tr>
                    <tr>
                        <td><%# Translate("Language") %></td>
                        <td><%# CurrentLanguageName %></td>
                    </tr>
                    <tr>
                        <td><%# Translate("Date of Exam") %></td>
                        <td><%# Eval("ExamDate") %></td>
                    </tr>
                </tbody>
            </table>

            <table class="data results">
                <thead>
                    <tr>
                        <th colspan="2"><%# Translate("Exam Results") %></th>
                    </tr>
                    <tr>
                        <th><%# Translate("Field of Practice") %></th>
                        <th><%# Translate("Exam Result") %></th>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="NoDataRow">
                        <td colspan="2"><%# Translate("No scores") %></td>
                    </tr>
                    <asp:Repeater runat="server" ID="FrameworkRepeater">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("FrameworkTitle") %></td>
                                <td><%# Eval("PassOrFail") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="2">
                            <b><%# Translate("Exam Passing Criteria") %>:</b>
                            <%# Translate("TakerReport.ExamPassingCriteria") %>
                        </td>
                    </tr>
                </tfoot>
            </table>

        </ItemTemplate>
    </asp:Repeater>

</body>
</html>
