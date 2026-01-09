<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesPanel.ascx.cs" Inherits="InSite.Admin.Courses.Activities.Controls.CompetenciesPanel" %>

<%@ Register Src="./CompetenciesNode.ascx" TagName="CompetenciesNode" TagPrefix="uc" %>
<%@ Register Src="./CompetenciesSelector.ascx" TagName="CompetenciesSelector" TagPrefix="uc" %>

<insite:NavItem runat="server" ID="CompetenciesTab" Title="Competencies">

    <div runat="server" id="NoCompetenciesAlert" class="alert alert-warning">
        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
        This <span runat="server" id="AssetType" /> does not yet contain any competencies.
    </div>

    <asp:Repeater runat="server" ID="NodeRepeater">
        <HeaderTemplate>
            <div style="margin-left:-32px;">
        </HeaderTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <uc:CompetenciesNode runat="server" ID="CompetenciesNode" />
        </ItemTemplate>
    </asp:Repeater>

</insite:NavItem>

<insite:NavItem runat="server" ID="AddCompetenciesTab" Title="Add Competencies">

    <uc:CompetenciesSelector runat="server" ID="CompetenciesSelector" />

    <div class="row" style="margin-top:15px;">
        <div class="col-md-12">
            <insite:SaveButton runat="server" ID="SaveCompetenciesButton" />
        </div>
    </div>

</insite:NavItem>
            