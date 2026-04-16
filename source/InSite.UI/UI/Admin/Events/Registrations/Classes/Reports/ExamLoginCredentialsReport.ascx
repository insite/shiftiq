<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamLoginCredentialsReport.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Reports.ExamLoginCredentialsReport" %>

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
    </style>
</head>
<body>
    <div style="page-break-inside: avoid;">
        <asp:Repeater runat="server" ID="RegistrationRepeater">
            <HeaderTemplate>
                <table style="width:100%;padding:0;">
                    <thead>
                        <tr>
                            <th style="text-align:center;width:40px;">#</th>
                            <th style="width:200px;">Name</th>
                            <th style="width:100px;">ID #</th>
                            <th style="width:200px;">Email</th>
                            <th style="width:120px;">Password</th>
                            <th style="width:250px;">Exam Login URL</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr style="page-break-inside: avoid;">
                    <td style="text-align:center;"><%# Eval("Number") %></td>
                    <td><%# Eval("UserFullName") %></td>
                    <td><%# Eval("PersonCode") %></td>
                    <td><%# Eval("UserEmail") %></td>
                    <td><%# Eval("Password") %></td>
                    <td><%# Eval("ExamLoginUrl") %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <div style="padding-top:10px;">
            <b>Total registrations: <asp:Literal runat="server" ID="TotalRegistrations" /></b>
        </div>
    </div>
</body>
</html>
