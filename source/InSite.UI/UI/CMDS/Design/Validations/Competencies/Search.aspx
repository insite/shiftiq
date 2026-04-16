<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Actions.Talent.Employee.Competency.Validation.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Portal/Validations/Competencies/SearchCriteria.ascx" TagName="EmployeeCompetencySearchCriteria" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Portal/Validations/Competencies/SearchResults.ascx" TagName="EmployeeCompetencySearchResults" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:EmployeeCompetencySearchResults ID="SearchResults" runat="server" EditorType="Validation" />

            <div class="mt-3 alert d-flex alert-warning">
                <i class="fa-solid fa-info-square fs-xl me-2"></i>
                <p><strong>System Notice:</strong> A known issue may allow validators to complete validations for competencies they are not validated in. Please ensure you are validated in each competency before proceeding. This issue is currently being addressed.</p>
            </div>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:EmployeeCompetencySearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>