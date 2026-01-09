<%@ Control AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Messages.Messages.Controls.SearchCriteria" Language="C#" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div runat="server" id="OrganizationIdentifierCriterion" class="mb-2">
                        <insite:FindOrganization runat="server" ID="OrganizationIdentifier" EmptyMessage="Organization" />
                    </div>

                    <div class="mb-2">
                        <insite:MessageTypeComboBox runat="server" ID="MessageType" EmptyMessage="Message Type" />
                    </div>                   
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Subject" EmptyMessage="Subject" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="MessageName" EmptyMessage="Message Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ModifiedSince" runat="server" EmptyMessage="Last Modified Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ModifiedBefore" runat="server" EmptyMessage="Last Modified Before" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="MessageDisabled" EmptyMessage="Message Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Only Disabled" Value="True" />
                                <insite:ComboBoxOption Text="Exclude Disabled" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderNickname" EmptyMessage="Sender Nickname" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderName" EmptyMessage="Sender Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderEmail" EmptyMessage="Sender Email" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SystemMailbox" EmptyMessage="System Mailbox" MaxLength="100" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">         
        <div class="mb-2">

            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>

            <div class="mb-2">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Subject" Value="MessageTitle" />
                        <insite:ComboBoxOption Text="Sort by Last Modified" Value="LastModified" />
                    </Items>
                </insite:ComboBox>
            </div>
            
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
