<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-8">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Name" EmptyMessage="Internal Name" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Title" EmptyMessage="External Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="LastModifiedSince" runat="server" EmptyMessage="Last Modified Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="LastModifiedBefore" runat="server" EmptyMessage="Last Modified Before" />
                    </div>
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-6">

                    <div class="mb-2">
                        <insite:MultiComboBox runat="server" ID="CurrentStatus" EmptyMessage="Publication Status">
                            <Items>
                                <insite:ComboBoxOption Value="Drafted" Text="Draft" />
                                <insite:ComboBoxOption Value="Opened" Text="Open (Published)" />
                                <insite:ComboBoxOption Value="Closed" Text="Closed" />
                                <insite:ComboBoxOption Value="Archived" Text="Archived" />
                            </Items>
                        </insite:MultiComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="LockStatus" EmptyMessage="Lock Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Locked" />
                                <insite:ComboBoxOption Value="False" Text="Unlocked" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FindMessage runat="server" ID="MessageIdentifier" EmptyMessage="Notifications"/>
                    </div>
                </div>               
            </div> 
        </div>
    </div>
    <div class="col-4">  
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div class="mb-2">
            <insite:ComboBox ID="SortColumns" runat="server">
                <Items>
                    <insite:ComboBoxOption Text="Sort by Name" Value="SurveyFormTitle" />
                    <insite:ComboBoxOption Text="Sort by Last Action Time" Value="LastChangeTime DESC" />
                </Items>
            </insite:ComboBox>
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
