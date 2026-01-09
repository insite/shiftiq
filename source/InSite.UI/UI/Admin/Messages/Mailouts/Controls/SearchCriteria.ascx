<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Messages.Mailouts.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Sender" EmptyMessage="Sender" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Recipient" EmptyMessage="Recipient" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Subject" EmptyMessage="Subject" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Status" EmptyMessage="Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Scheduled" Text="Scheduled" />
                                <insite:ComboBoxOption Value="Started" Text="Started" />
                                <insite:ComboBoxOption Value="Completed" Text="Completed" />
                                <insite:ComboBoxOption Value="Cancelled" Text="Cancelled" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CompletedSince" runat="server" EmptyMessage="Completed &ge;" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CompletedBefore" runat="server" EmptyMessage="Completed &lt;" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="MessageType" EmptyMessage="Message Type">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Invitation" Text="Invitation" />
                                <insite:ComboBoxOption Value="Newsletter" Text="Newsletter" />
                                <insite:ComboBoxOption Value="Notification" Text="Notification" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="MinDeliveryCount" runat="server" EmptyMessage="Deliveries &ge;" NumericMode="Integer" DigitGrouping="false" MinValue="0" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox ID="MaxDeliveryCount" runat="server" EmptyMessage="Deliveries &le;" NumericMode="Integer" DigitGrouping="false" MinValue="0" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">       
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>