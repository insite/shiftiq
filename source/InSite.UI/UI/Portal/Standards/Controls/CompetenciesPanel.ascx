<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesPanel.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.CompetenciesPanel" %>

<%@ Register Src="CompetenciesNode.ascx" TagName="CompetenciesNode" TagPrefix="uc" %>
<%@ Register Src="CompetenciesSelector.ascx" TagName="CompetenciesSelector" TagPrefix="uc" %>

<div class="row settings">
    <div class="col-lg-12">
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CompetenciesUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="CompetenciesUpdatePanel">
            <ContentTemplate>
                <insite:Alert runat="server" ID="Status" />

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" ID="CompetenciesTab" Title="Competencies">

                        <div runat="server" id="NoContainedCompetenciesWarning" class="alert alert-warning">
                        
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

                        <div class="row mt-3">
                            <div class="col-md-12">
                                <insite:SaveButton runat="server" ID="SaveCompetenciesButton" />
                            </div>
                        </div>

                    </insite:NavItem>

                </insite:Nav>
            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
</div>