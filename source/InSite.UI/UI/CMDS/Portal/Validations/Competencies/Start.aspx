<%@ Page Language="C#" CodeBehind="Start.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Assessment.Begin" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/CompetencySummary.ascx" TagName="CompetencySummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <h2 class="h4 mb-3"><i class="far fa-balance-scale me-2"></i>No Self-Assessments</h2>

    <div class="card border-0 shadow-lg">
        <div class="card-body">
            <p class="alert alert-success">No action is needed. None of your competencies require a self-assessment.</p>
            <h3 class="h5 mb-3">Individual Competency Summary</h3>
            <uc:CompetencySummary ID="CompetencySummary" runat="server" />
        </div>
    </div>

</asp:Content>
