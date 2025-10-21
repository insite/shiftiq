<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoresReport.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ScoresReport" %>


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
            margin-bottom: 6px;
        }

        table {
            border-bottom: 1px solid grey;
            width: 100%;
            border-collapse: collapse;
        }

            table thead tr th {
                border: none;
                border-top: 1px solid grey;
                border-bottom: 1px solid grey;
            }

            table tr th + th {
                padding-left: 6px;
            }

            table tr td + td {
                padding-left: 6px;
            }

        tr th, tr td {
            text-align: center;
        }

            tr th:nth-child(1), tr td:nth-child(1) {
                text-align: left;
            }
    </style>
</head>
<body>
    <div class="header">
        <asp:Literal ID="ClassTitle" runat="server" /><br />
        <asp:Literal ID="ClassStartDate" runat="server" /> - <asp:Literal ID="ClassEndDate" runat="server" /><br />
    </div>

    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Registered</th>
                <th>Approval</th>
                <th>ID #</th>

                <asp:Repeater runat="server" ID="ScoreHeaderRepeater">
                    <ItemTemplate>
                        <th><%# Eval("ItemName") %></th>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="StudentRepeater">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("StudentName") %></td>
                        <td><%# Eval("RegisteredDate") %></td>
                        <td><%# Eval("Approval") %></td>
                        <td><%# Eval("PersonCode") %></td>

                        <asp:Repeater runat="server" ID="ScoreRepeater">
                            <ItemTemplate>
                                <td><%# Container.DataItem %></td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>

    <div style="padding-top:10px;">
        <b>Total Number:</b>
        <b><%= StudentCount %></b>
        Students
    </div>
</body>
</html>
