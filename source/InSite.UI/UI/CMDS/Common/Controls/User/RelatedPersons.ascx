<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedPersons.ascx.cs" Inherits="InSite.Cmds.Controls.User.RelatedPersons" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencySummary.ascx" TagName="CompetencySummary" TagPrefix="uc" %>

<style type="text/css">
    div.row.contact + div.row.contact { padding-top:10px; }
    div.barchart { float: left; margin-right: 5px; }
    div.related-persons-section div.row + div.row { padding-top:20px;  }
    table.related-persons td { padding-right:10px; }
    strong.reporting-line { display:block; padding: 5px; color: #457897; }
</style>

<asp:Literal runat="server" ID="NoContacts">
    <ul>
        <li>Your contact list is empty.</li>
    </ul>
</asp:Literal>

<asp:PlaceHolder ID="MainPanel" runat="server">

    <div runat="server" id="LeaderRow" class="row contact">
        <div class="col-lg-12">
            <strong class="reporting-line">Leaders</strong>
            <table class="table table-striped related-persons">
                <asp:Repeater ID="Leaders" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("FullName") %></td>
                            <td>
                                <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                                <div><%# Eval("Phone") %></div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>

    <div runat="server" ID="ManagerRow" class="row contact">
        <div class="col-lg-12">
            <strong class="reporting-line">Managers</strong>
            <table class="table table-striped related-persons">
            <asp:Repeater ID="Managers" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FullName") %></td>
                        <td>
                            <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                            <div><%# Eval("Phone") %></div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            </table>
        </div>
    </div>

    <div runat="server" id="SupervisorRow" class="row contact">
        <div class="col-lg-12">
            <strong class="reporting-line">Supervisors</strong>
            <table class="table table-striped related-persons">
            <asp:Repeater ID="Supervisors" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FullName") %></td>
                        <td>
                            <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                            <div><%# Eval("Phone") %></div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            </table>
        </div>
    </div>

    <div runat="server" id="ValidatorRow" class="row contact">
        <div class="col-lg-12">
            <strong class="reporting-line">Validators</strong>
            <table class="table table-striped related-persons">
                <asp:Repeater ID="Validators" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("FullName") %></td>
                            <td>
                                <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                                <div><%# Eval("Phone") %></div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>

    <asp:PlaceHolder runat="server" ID="EmployeeRow">
        <div class="row contact">
            <div class="col-lg-12">
                <strong class="reporting-line">Workers</strong>
                <table class="table table-striped related-persons">
                <asp:Repeater ID="Employees" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <a target="_blank" href='/ui/portal/home/competencies?learner=<%# Eval("UserIdentifier").ToString() %>'>
                                    <insite:Icon runat="server" Type="Regular" Name="tachometer-alt-fast" ToolTip="Dashboard" />
                                </a>
                            </td>
                            <td><%# Eval("FullName") %> </td>
                            <td>
                                <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                                <div><%# Eval("Phone") %></div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                </table>

            </div>
        </div>

        <div class="row contact" style="padding-bottom:5px;">
            <div class="col-lg-12">
                <strong>Group Competency Summary (<asp:Literal ID="LearnerCount" runat="server" />)</strong>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <uc:CompetencySummary ID="GroupCompetencySummary" runat="server" MaxBarWidth="210" />
                <div class="form-text" style="margin-top: -10px; margin-bottom: 20px;">
                    This summary includes only <strong>primary</strong> profile competencies assigned to your group. 
                    Competencies that are found only under secondary profiles are <strong>not</strong> counted here.
                    Also, your own competencies are <strong>not</strong> counted in this summary.
                </div>
            </div>
        </div>

        <div runat="server" id="StudentsPanel" class="row">
            <div class="col-lg-12">
                
                <strong>Learners</strong>
                <table class="related-persons">
                <asp:Repeater ID="Students" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("FullName") %> </td>
                            <td><a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a></td>
                            <td><%# Eval("Phone") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                </table>
            </div>
        </div>

    </asp:PlaceHolder>

</asp:PlaceHolder>
