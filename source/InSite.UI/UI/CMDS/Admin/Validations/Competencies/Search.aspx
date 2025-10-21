<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Admin.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Portal/Validations/Competencies/SearchCriteria.ascx" TagName="EmployeeCompetencySearchCriteria" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Portal/Validations/Competencies/SearchResults.ascx" TagName="EmployeeCompetencySearchResults" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:EmployeeCompetencySearchResults ID="SearchResults" runat="server" EditorType="AdminEditor" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:EmployeeCompetencySearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>
