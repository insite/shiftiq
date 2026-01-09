<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Courses.Links.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox ID="Publisher" runat="server" EmptyMessage="Publisher">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Nelson" Text="Nelson" />
                                <insite:ComboBoxOption Value="Pearson" Text="Pearson" />
                                <insite:ComboBoxOption Value="PanGlobal" Text="PanGlobal" />
                            </Items>
                        </insite:ComboBox>
                    </div>
    
                    <div class="mb-2">
                        <insite:ComboBox ID="Subtype" runat="server" EmptyMessage="Subtype">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="LTI" Text="LTI Links" />
                                <insite:ComboBoxOption Value="Moodle" Text="Moodle Links" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" EmptyMessage="Title" />
                    </div>                   
                    
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
                <div class="col-6">
                    <div class="mb-2"> 
                        <insite:TextBox ID="Location" runat="server" EmptyMessage="Location" />
                    </div>

                    <div class="mb-2">
                       <insite:TextBox ID="Code" runat="server" EmptyMessage="Code" />
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