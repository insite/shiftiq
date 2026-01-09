<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassRegistrationsByCertificatesAndVenues.ascx.cs" Inherits="InSite.Admin.Events.Classes.Reports.ClassRegistrationsByCertificatesAndVenues" %>

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

        div.achievement + div.achievement {
            padding-top:40px;
        }
        
        div.achievement .header {
            margin-bottom: 6px;
            font-weight: bold;
        }

        div.criteria {
            padding-bottom: 20px;
        }

        table.table {
            border-bottom: 1px solid grey;
            border-collapse: collapse;
        }

            table.table thead tr th {
                border: none;
                border-top: 1px solid grey;
                border-bottom: 1px solid grey;
                text-align: left;
            }

            table.table tr.line-top td {
                border-top: 1px solid grey;
            }

            table.table tr th + th {
                padding-left: 6px;
            }

            table.table tr td + td {
                padding-left: 6px;
            }
            
        .text-center {
            text-align: center;
        }
        
        .text-right {
            text-align: right;
        }

        .text-bold {
            font-weight: bold;
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
            All Classes Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="AchievementRepeater">
        <ItemTemplate>
            <div class="achievement" style="page-break-inside: avoid;">
                <div class="header">
                    <%# Eval("AchievementTitle") %>
                </div>

                <table>
                    <tr>
                        <td style="width:70%;vertical-align:top;">
                            <table class="table">
                                <thead style="page-break-inside:avoid;">
                                    <tr>
                                        <th style="width:30px"></th>
                                        <th style="width:200px">Class</th>
                                        <th style="width:50px">Students</th>
                                        <th style="width:100px">Venue</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="ClassRepeater">
                                        <ItemTemplate>
                                            <tr style="page-break-inside: avoid;">
                                                <td><%# Eval("ClassNumber") %></td>
                                                <td><%# Eval("ClassTitle") %></td>
                                                <td class="text-center"><%# Eval("RegistrationCount") %></td>
                                                <td><%# Eval("Venue") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                         
                                    <tr class="line-top" style="page-break-inside: avoid;">
                                        <td></td>
                                        <td class="text-bold">TOTAL</td>
                                        <td class="text-center text-bold"><%# Eval("RegistrationCount") %></td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td style="width:30%;vertical-align:bottom;">
                           <table class="table">
                                <thead style="page-break-inside: avoid;">
                                    <tr>
                                        <th style="width:50px"></th>
                                        <th style="width:200px">Venue</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="VenueRepeater">
                                        <ItemTemplate>
                                            <tr style="page-break-inside: avoid;">
                                                <td style="text-align:center;"><%# Eval("RegistrationCount") %></td>
                                                <td><%# Eval("Venue") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <tr class="line-top" style="page-break-inside: avoid;">
                                        <td class="text-center text-bold"><%# Eval("RegistrationCount") %></td>
                                        <td class="text-bold">TOTAL</td>
                                    </tr>
                                </tbody>
                           </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
