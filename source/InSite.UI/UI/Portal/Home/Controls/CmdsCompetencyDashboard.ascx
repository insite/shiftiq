<%@ Control Language="C#" CodeBehind="CmdsCompetencyDashboard.ascx.cs"
    Inherits="InSite.UI.Portal.Home.Controls.CmdsCompetencyDashboard" %>

<%@ Register Src="./CmdsCompetencySummary.ascx"  TagName="CmdsCompetencySummary"  TagPrefix="uc" %>
<%@ Register Src="./CmdsComplianceSummary.ascx"  TagName="CmdsComplianceSummary"  TagPrefix="uc" %>
<%@ Register Src="./CmdsLearningSummary.ascx"    TagName="CmdsLearningSummary"    TagPrefix="uc" %>
<%@ Register Src="./CmdsOrientationSummary.ascx" TagName="CmdsOrientationSummary" TagPrefix="uc" %>
<%@ Register Src="./UserCompetencySummary.ascx"  TagName="UserCompetencySummary"  TagPrefix="uc" %>

<style type="text/css">
    table.my-summary td.my-summary-flag { width:30px; }
    table.my-summary td.my-summary-text { width: 360px; }
    table.my-summary td.my-summary-score { width: 100px; text-align:right; }
    table.my-summary td.my-summary-progress { }
</style>

<insite:Alert runat="server" ID="StatusAlert" />
    
<!-- Title -->
<div runat="server" id="TitlePanel" class="d-sm-flex align-items-center justify-content-between mb-4 text-center text-sm-start">
    <div class="d-flex align-items-center">
        <div>
            <insite:ComboBox runat="server" ID="ShowWhat" ButtonSize="Small">
                <Items>
                    <insite:ComboBoxOption Value="Primary" Text="Primary profile only" Selected="true" />
                    <insite:ComboBoxOption Value="Mandatory" Text="All profiles requiring compliance" />
                </Items>
            </insite:ComboBox>
        </div>
        <div runat="server" id="CmdsPrimaryProfilePanel" class="ms-3">
            <strong><span runat="server" id="CmdsPrimaryProfileName"></span></strong>
        </div>
    </div>
</div>

<div runat="server" id="CmdsCompetencySummaryPanel" class="row mb-3">
    <div class="col-lg-6 mb-3 mb-lg-0">
        <h3>Competency Summary</h3>
        <uc:CmdsCompetencySummary ID="CmdsPrimaryCompetencySummary" runat="server" MaxBarWidth="80" />
    </div>
    <div class="col-lg-6 mb-3 mb-lg-0">
        <insite:UpdatePanel runat="server" ID="CmdsSecondaryProfileContainer">
            <ContentTemplate>
            
                <h3>Secondary Profile</h3>
                
                <asp:Panel runat="server" ID="CmdsSecondaryProfileSummary" Visible="false">
                    <uc:CmdsCompetencySummary ID="CmdsSecondaryCompetencySummary" runat="server" MaxBarWidth="80" />
                </asp:Panel>
            
                <cmds:EmployeeProfileSelector ID="CmdsSecondaryEmployeeProfile" runat="server" Width="100%" AllowNull="true" MaxHeight="200" IncludePrimary="false" EmptyMessage="Select a profile to display chart" />

            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
</div>

<insite:Container runat="server" id="CmdsLearningSummaryPanel">
    <uc:CmdsComplianceSummary runat="server" ID="CmdsComplianceSummary" />
    <uc:CmdsLearningSummary runat="server" ID="CmdsLearningSummary" />
    <uc:CmdsOrientationSummary runat="server" ID="CmdsOrientationSummary" />
</insite:Container>

<uc:UserCompetencySummary runat="server" ID="UserCompetencySummary" />