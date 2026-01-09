<%@ Page Language="C#" CodeBehind="GroupCompetencySummary.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Group.GroupCompetencySummary" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencySummary.ascx" TagName="CompetencySummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" ID="ReportSection" class="mb-3">

        <h2 class="h4 mb-3">
            <i class="far fa-chart-bar me-1"></i>
            Report
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <asp:Repeater runat="server" ID="ManagersRepeater">
                    <ItemTemplate>
                        <h3><%# Eval("FullName") %></h3>

                        <div class="row ms-3">
                            <asp:Repeater runat="server" ID="RelatedEmployees">
                                <ItemTemplate>
                                    <div class="col-md-6 mb-3">
                                        <h5><%# Eval("FullName") %> (<asp:Literal runat="server" ID="EmployeeCountOutput" />)</h5>

                                        <uc:CompetencySummary ID="GroupCompetencySummary" runat="server" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>
</asp:Content>