<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Achievements.Achievements.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox ID="AchievementTitle" runat="server" EmptyMessage="Title" MaxLength="256" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="AchievementLabel" runat="server" EmptyMessage="Tag" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="AchievementDescription" runat="server" EmptyMessage="Description" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="AchievementLockStatus" EmptyMessage="Lock Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="False" Text="Locked" />
                                <insite:ComboBoxOption Value="True" Text="Unlocked" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <asp:Panel runat="server" ID="ExpirationCriteria">
                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ExpirationalUpdatePanel" />
                        
                        <insite:UpdatePanel runat="server" ID="ExpirationalUpdatePanel">
                          <ContentTemplate>
                            <div class="mb-2">
                                <insite:ComboBox runat="server" ID="ExpirationType" EmptyMessage="Expiration Type">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="None" Text="No Expiry" />
                                        <insite:ComboBoxOption Value="Fixed" Text="Fixed Date" />
                                        <insite:ComboBoxOption Value="Relative" Text="Relative Date" />
                                    </Items>
                                </insite:ComboBox>
                            </div>

                            <div runat="server" id="ExpirationFixedDateSinceField" class="mb-2">
                                <insite:DateTimeOffsetSelector ID="ExpirationFixedDateSince" runat="server" EmptyMessage="Expiration Fixed Since" />
                            </div>

                            <div runat="server" id="ExpirationFixedDateBeforeField" class="mb-2">
                                <insite:DateTimeOffsetSelector ID="ExpirationFixedDateBefore" runat="server" EmptyMessage="Expiration Fixed Before" />
                            </div>

                            <div runat="server" id="ExpirationLifetimeQuantityField" class="mb-2">
                                <insite:NumericBox ID="ExpirationLifetimeQuantity" runat="server" EmptyMessage="Expiration Lifetime Quantity" NumericMode="Integer" DigitGrouping="false" MinValue="0" />
                            </div>

                            <div runat="server" id="ExpirationLifetimeUnitField" class="mb-2">
                                <insite:ComboBox runat="server" ID="ExpirationLifetimeUnit" EmptyMessage="Expiration Lifetime Unit">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="Month" Text="Month" />
                                        <insite:ComboBoxOption Value="Year" Text="Year" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                          </ContentTemplate>
                        </insite:UpdatePanel>                                               
                    </asp:Panel>
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