<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Logs.Commands.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">

            <h4>Criteria</h4>

            <div class="row">
                <div class="col-6">
                    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="UserIdentifier" EmptyMessage="User Identifier" MaxLength="36" />
                    </div>
                    
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:ComboBox runat="server" ID="CommandState" EmptyMessage="Command State">
                                    <Items>
                                        <insite:ComboBoxOption Selected="true" />
                                        <insite:ComboBoxOption Text="Scheduled" Value="Scheduled" />
                                        <insite:ComboBoxOption Text="Started" Value="Started" />
                                        <insite:ComboBoxOption Text="Completed" Value="Completed" />
                                        <insite:ComboBoxOption Text="Cancelled" Value="Cancelled" />
                                        <insite:ComboBoxOption Text="Bookmarked" Value="Bookmarked" />
                                    </Items>
                                </insite:ComboBox>
                            </div>

                            <div class="mb-2">
                                <insite:ComboBox runat="server" ID="IsRecurring" EmptyMessage="Recurrence">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Text="Not Recurring" Value="False" Selected="true" />
                                        <insite:ComboBoxOption Text="Recurring" Value="True" />
                                    </Items>
                                </insite:ComboBox>
                            </div>

                            <insite:Container runat="server" ID="DateContainer">
                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector ID="DateSince" runat="server" />
                                </div>

                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector ID="DateBefore" runat="server" />
                                </div>
                            </insite:Container>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>

                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CommandClass" EmptyMessage="Command Type" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CommandType" EmptyMessage="Command Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CommandData" EmptyMessage="Command Data" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SendError" EmptyMessage="Send Error" MaxLength="256" />
                    </div>
                </div>
            </div>

            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-3">
        <div>
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>       
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>    
</div>