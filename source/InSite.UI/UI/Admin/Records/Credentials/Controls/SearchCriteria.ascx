<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Achievements.Credentials.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="AchievementTitle" runat="server" EmptyMessage="Achievement Title" MaxLength="200" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox ID="LearnerName" runat="server" EmptyMessage="Learner Name" MaxLength="120" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="LearnerEmail" runat="server" EmptyMessage="Learner Email" MaxLength="254" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PersonCode" MaxLength="20" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CredentialStatus" EmptyMessage="Achievement Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Undefined" Value="Undefined" />
                                <insite:ComboBoxOption Text="Pending" Value="Pending" />
                                <insite:ComboBoxOption Text="Valid" Value="Valid" />
                                <insite:ComboBoxOption Text="Expired" Value="Expired" />
                                <insite:ComboBoxOption Text="Revoked" Value="Revoked" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="AchievementLabel" EmptyMessage="Achievement Tag" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:FindEmployer ID="EmployerGroupIdentifier" runat="server" EmptyMessage="Employer" isBootStrap5="true" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="EmployerGroupStatus" EmptyMessage="Employer Group Status" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox ID="EmployerRegion" runat="server" EmptyMessage="Employer Region" isBootStrap5="true" />
                    </div>
                    
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialGrantedSince" runat="server" EmptyMessage="Achievement Granted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialGrantedBefore" runat="server" EmptyMessage="Achievement Granted Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialExpiredSince" runat="server" EmptyMessage="Achievement Expired Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialExpiredBefore" runat="server" EmptyMessage="Achievement Expired Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialRevokedSince" runat="server" EmptyMessage="Achievement Revoked Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="CredentialRevokedBefore" runat="server" EmptyMessage="Achievement Revoked Before" />
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
            <div class="mb-4">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Learner Name" Value="UserFullName" />
                        <insite:ComboBoxOption Text="Sort by Achievement Title" Value="AchievementTitle,UserFullName" />
                        <insite:ComboBoxOption Text="Sort by Achievement Granted Date" Value="CredentialGranted desc,UserFullName" />
                        <insite:ComboBoxOption Text="Sort by Achievement Expired Date" Value="CredentialExpirationExpected desc" />
                        <insite:ComboBoxOption Text="Sort by Achievement Tag" Value="AchievementLabel,UserFullName" />
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
