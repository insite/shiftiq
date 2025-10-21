<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Messages.Emails.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderName" EmptyMessage="Sender Name" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderEmail" EmptyMessage="Sender Email" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ToName" EmptyMessage="To Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ToEmail" EmptyMessage="To Email" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Subject" EmptyMessage="Subject" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Body" EmptyMessage="Body" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="StatusCodeSelector" EmptyMessage="Select Status Code" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="StatusMessageSelector" EmptyMessage="Select Message Status" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DeliveryTimeSince" EmptyMessage="Delivery Time &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DeliveryTimeBefore" EmptyMessage="Delivery Time &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsSuccessful" EmptyMessage="Delivery Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Delivery Successful" />
                                <insite:ComboBoxOption Value="False" Text="Delivery Failed" />
                            </Items>
                        </insite:ComboBox>
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