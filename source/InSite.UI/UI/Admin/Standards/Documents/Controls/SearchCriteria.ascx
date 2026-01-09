<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="DocumentType" EmptyMessage="Document Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Keyword" runat="server" EmptyMessage="Keyword" MaxLength="30" />
                    </div>
                    
                    <div class="mb-2">
                        <insite:ComboBox ID="Level" runat="server" EmptyMessage="Level">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Frontline" Text="Frontline" />
                                <insite:ComboBoxOption Value="Supervisory" Text="Supervisory" />
                                <insite:ComboBoxOption Value="Management/Executive" Text="Management/Executive" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="IsTemplate" EmptyMessage="Template Setting" TrueText="Templates" FalseText="Non-Templates" />
                    </div>
    
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="PrivacyScope" EmptyMessage="Privacy Scope">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Tenant" Text="Organization" />
                                <insite:ComboBoxOption Value="User" Text="User" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:FindUser runat="server" ID="CreatedBy" EmptyMessage="Created By" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcPostedSince" runat="server" EmptyMessage="Date Posted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="UtcPostedBefore" runat="server" EmptyMessage="Date Posted Before" />
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
    </div>
    <div class="col-3">              
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
