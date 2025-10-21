<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Senders.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderNickname" EmptyMessage="Sender Nickname" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:SenderTypeComboBox ID="SenderType" runat="server" EmptyMessage="Sender Type" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderName" EmptyMessage="Sender Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SenderEmail" EmptyMessage="Sender Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="SystemMailbox" EmptyMessage="System Mailbox" MaxLength="256" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="SenderEnabled" EmptyMessage="Sender Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Enabled" />
                                <insite:ComboBoxOption Value="False" Text="Disabled" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyAddress" EmptyMessage="Company Address" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyCity" EmptyMessage="Company City" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyPostalCode" EmptyMessage="Company Postal Code" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CompanyCountry" EmptyMessage="Company Country" MaxLength="256" />
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