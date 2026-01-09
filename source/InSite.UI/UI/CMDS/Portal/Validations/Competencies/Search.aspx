<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Assessment.Find" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="SearchCriteria.ascx" TagName="EmployeeCompetencySearchCriteria" TagPrefix="uc" %>
<%@ Register Src="SearchResults.ascx" TagName="EmployeeCompetencySearchResults" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:EmployeeCompetencySearchResults ID="SearchResults" runat="server" EditorType="SelfAssessment" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:EmployeeCompetencySearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>