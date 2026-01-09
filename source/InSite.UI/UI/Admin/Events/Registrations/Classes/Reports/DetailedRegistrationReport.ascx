<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailedRegistrationReport.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Reports.DetailedRegistrationReport" %>

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
            text-align: left;
            font-size: large;
            padding-bottom: 10px;
            margin-bottom: 6px;
        }

        div.activity + div.activity {
            padding-top:40px;
        }

        div.person {
            padding-bottom: 6px;
        }

            div.person + div.person {
                padding-top: 6px;
            }
    </style>
</head>
<body>
    <asp:Repeater runat="server" ID="EventRepeater">
        <ItemTemplate>
            <div class="activity">
                <div class="header" style="border-bottom:1px solid grey;page-break-inside: avoid;">
                    <b><%# Eval("EventTitle") %></b> (<%# Eval("EventStart") %> - <%# Eval("EventEnd") %>)
                </div>
                <div>
                    <asp:Repeater runat="server" ID="RegistrationRepeater">
                        <ItemTemplate>
                            <div class="person" style="border-bottom:1px solid grey;page-break-inside: avoid;">
                            <div style="width:25%;float:left;">
                                <div>
                                    <%# Eval("Number") %>. <%# Eval("UserFullName") %>
                                </div>
                                <div>
                                    <label>Email:</label>
                                    <%# Eval("UserEmail") %>
                                </div>
                                <div>
                                    <label>ID #:</label>
                                    <%# Eval("PersonCode") %>
                                </div>
                                <div>
                                    <label>Birthdate:</label>                                    
                                    <%# Eval("Birthdate", "{0:MMM d, yyyy}") %>
                                    <label>Age:</label>
                                    <%# Eval("Age") %><%# (bool)Eval("IsMinor") ? " *" : "" %>
                                </div>
                                <div>
                                    <label>ESL:</label>
                                    <%# (bool)Eval("ESL") ? "Yes" : "No" %>
                                </div>
                            </div>
                            <div style="width:25%;float:left;">
                                <div>
                                    <label>Phones:</label>
                                    <%# Eval("UserPhones") %></td>
                                </div>
                                <div>
                                    <label>Home Address:</label>
                                    <%# Eval("UserAddress") %>
                                </div>
                                <div>
                                    <label>Emergency contact:</label>
                                    <div><%# Eval("EmergencyContact") %></div>
                                </div>
                            </div>
                            <div style="width:25%;float:left;">
                                <div>
                                    <lavel>Employer:</lavel>
                                    <%# Eval("EmployerName") %>
                                </div>
                                <div>
                                    <label>Mailing Address:</label>
                                    <div>
                                        <%# Eval("EmployerAddress") %> 
                                    </div>
                                    <diV>
                                        <%# Eval("EmployerPhones") %>
                                    </diV>
                                </div>
                                <div>
                                    <label>Employer Contact:</label>
                                    <div>
                                        <div> <%# Eval("EmployerContactName") %></div>
                                        <div><%# Eval("EmployerContactPhone") %></div>
                                        <div><%# Eval("EmployerContactEmail") %></div>
                                    </div>
                                </div>
                            </div>
                            <div style="width:25%;float:left;">
                                <div><%# Eval("ApprovalStatus") %> on <%# Eval("RegistrationDate", "{0:MMM d, yyyy}") %></div>
                                <div runat="server" visible='<%# Eval("AttendanceStatus") != null %>'>
                                    <%# Eval("AttendanceStatus") %>
                                </div>
                                <div>
                                    <label>Hours Worked:</label>
                                    <%# Eval("HoursWorked") %>
                                </div>
                                <div>
                                    <label>Fee:</label>
                                    <%# Eval("Fee","{0:c2}") %>
                                </div>
                                <div>
                                    <label>Paid By:</label>
                                    <%# Eval("CustomerName") %>
                                </div>
                            </div>
                            <div style="clear:both;"></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>