<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Messages.Clicks.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ClickedSince" runat="server" EmptyMessage="Clicked Since" />
                    </div>
    
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ClickedBefore" runat="server" EmptyMessage="Clicked Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="MessageTitle" runat="server" EmptyMessage="Message Title" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="UserName" runat="server" EmptyMessage="User Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="LinkText" runat="server" EmptyMessage="Link Text" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="LinkUrl" runat="server" EmptyMessage="Link Url" MaxLength="256" />
                    </div>                    
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="UserEmail" runat="server" EmptyMessage="User Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="UserBrowser" runat="server" EmptyMessage="User Browser" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="UserHostAddress" runat="server" EmptyMessage="User IP Address" MaxLength="256" />
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
