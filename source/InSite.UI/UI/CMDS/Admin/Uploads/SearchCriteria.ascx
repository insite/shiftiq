<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Cmds.Admin.Uploads.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>

            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ContainerType" EmptyMessage="Object Type">
                            <Items>
                                <insite:ComboBoxOption Value="Tenant" Text="Organization" />
                                <insite:ComboBoxOption Value="Asset" Text="Achievement" />
                                <insite:ComboBoxOption Value="Workflow" Text="Credential" />
                                <insite:ComboBoxOption Value="ContactExperience" Text="Other Training and Education" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="UploadType" EmptyMessage="Upload Type">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="CMDS File" Text="File" />
                                <insite:ComboBoxOption Value="Link" Text="Link" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Keyword" runat="server" EmptyMessage="Keyword" MaxLength="256" />
                    </div>

                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:DateSelector ID="PostedSince" EmptyMessage="Uploaded Since" runat="server" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="PostedBefore" EmptyMessage="Uploaded Before" runat="server" />
                    </div>

                </div>
            </div>

            <div class="mt-3">
	            <insite:FilterButton ID="SearchButton" runat="server" />
	            <insite:ClearButton ID="ClearButton" runat="server" />
            </div>
        </div>
    </div>
    <div class="col-3">
        <h4>Settings</h4>
        <insite:MultiComboBox ID="ShowColumns" runat="server" />
    </div>
    <div class="col-3">
        <h4>Saved Filters</h4>
        <uc:FilterManager runat="server" ID="FilterManager" />
    </div>
</div>