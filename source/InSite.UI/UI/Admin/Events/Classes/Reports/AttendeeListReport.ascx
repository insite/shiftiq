<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendeeListReport.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.AttendeeListReport" %>

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

        .header {
            text-align: center;
            font-weight: bold;
            font-size: x-large;
        }

        tr th {
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
    <div class="header">
        Class List<br />
        <asp:Literal ID="ClassTitle" runat="server" /><br />
        <asp:Literal ID="ClassStartDate" runat="server" /> - <asp:Literal ID="ClassEndDate" runat="server" /><br />
        Instructor(s): <asp:Literal ID="ClassInstructors" runat="server" />
    </div>
    <div style="height:60px;"></div>
    <div>
        <asp:Repeater runat="server" ID="ParticipationsRepeater">
            <HeaderTemplate>
                <table style="width:100%;padding:0;">
                    <tr>
                        <th>Apprentice Name</th>
                        <th style="text-align:center;">ID #</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Employer</th>
                        <th></th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("UserFullName") %></td>
                    <td style="text-align:center;"><%# Eval("PersonCode") %></td>
                    <td><%# Eval("Email") %></td>
                    <td><%# Eval("Phone") %></td>
                    <td><%# Eval("EmployerName") %></td>
                    <td><%# IsMinor() ? "*" : "" %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</body>
</html>
