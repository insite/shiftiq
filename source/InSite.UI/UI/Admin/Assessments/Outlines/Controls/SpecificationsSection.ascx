<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationsSection.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.SpecificationsSection" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="SpecificationsUpdatePanel" />

<insite:UpdatePanel runat="server" ID="SpecificationsUpdatePanel">
    <ContentTemplate>
        <insite:Alert runat="server" ID="SpecificationWarning" />

        <div class="row mb-3">
            <div class="col-lg-6 mb-3 mb-lg-0">
                <insite:ComboBox runat="server" ID="SpecificationSelector" />
            </div>
            <div runat="server" id="CommandColumn" class="col-lg-6 text-end">
                <insite:DropDownButton runat="server" ID="CommandsDropDown" IconName="plus-circle" Text="Add" CssClass="d-inline-block">
                    <Items>
                        <insite:DropDownButtonItem Name="AddSpecification" ToolTip="Add a new specification to the bank" IconType="Regular" IconName="clipboard-list" Text="Specification" />
                        <insite:DropDownButtonItem Name="AddCriterion" ToolTip="Add new criteria to the selected specification" IconType="Regular" IconName="filter" Text="Criteria" />
                        <insite:DropDownButtonItem Name="AddForm" ToolTip="Add a new form to the selected specification" IconType="Regular" IconName="align-left" Text="Form" />
                    </Items>
                </insite:DropDownButton>

                <insite:Button runat="server" ID="SpecificationWorkshopButton" ButtonStyle="Default" ToolTip="Workshop" Text="Workshop" Icon="far fa-industry-alt" />
            </div>
        </div>

        <div runat="server" id="SpecificationCriteriaRow" class="row" visible="true">
            <div class="col-lg-2 col-4">
                <insite:Nav runat="server" ID="SpecificationCriteriaNav" ItemType="Pills" ItemAlignment="Vertical" ContentRendererID="SpecificationCriteriaNavContent">

                </insite:Nav>
            </div>
            <div id="spec-nav-content" class="col-lg-10 col-8">
                <insite:NavContent runat="server" ID="SpecificationCriteriaNavContent" />
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>
