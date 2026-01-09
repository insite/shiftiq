<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationReport.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Reports.RegistrationReport" %>

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
        
        div.criteria {
            padding-bottom: 20px;
        }
        
        div.activity + div.activity {
            padding-top:40px;
        }

        .header { 
            text-align: left;
            font-size:large;
            padding-bottom:10px;
            margin-bottom: 6px;
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
            All Registrations Shown
        </div>
    </div>

    <asp:Repeater runat="server" ID="EventRepeater">
        <ItemTemplate>
            <div class="activity" style="page-break-inside: avoid;">
                <div class="header">
                    <b><%# Eval("EventTitle") %></b> (<%# Eval("EventStart") %> - <%# Eval("EventEnd") %>)
                </div>
                <div>
                    <asp:Repeater runat="server" ID="RegistrationRepeater">
                        <HeaderTemplate>
                            <table style="width:100%;padding:0;">
                                <thead>
                                    <tr>
                                        <th style="text-align:center;width:40px;">#</th>
                                        <th style="width:100px;">ID #</th>
                                        <th style="width:300px;">Name</th>
                                        <th style="width:100px;">Status</th>
                                        <th style="width:300px;">Employer</th>
                                        <th style="width:20px;"></th>
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
                                <td><%# Eval("PersonCode") %></td>
                                <td><%# Eval("UserFullName") %></td>
                                <td><%# Eval("ApprovalStatus") %></td>
                                <td><%# Eval("EmployerName") %></td>
                                <td><%# (bool)Eval("IsMinor") ? "*" : "" %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div style="padding-top:10px;">
                    <span><b>Total registrations: <%# Eval("RegistrationCount") %></b></span>
                    <span runat="server" visible='<%# Eval("CapacityMax") != null %>' style="margin-left:15px;">(Maximum <%# Eval("CapacityMax") %>)</span>
                    <span style="margin-left:30px;">Waitlisted: <%# Eval("WaitlistedCount") %></span>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>
