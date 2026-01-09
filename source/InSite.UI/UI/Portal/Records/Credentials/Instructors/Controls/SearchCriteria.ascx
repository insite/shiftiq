<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Instructors.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Criteria" /></h4>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:FindAchievement runat="server" ID="AchievementIdentifier" EmptyMessage="Achievements" EnableTranslation="true" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="AchievementLabel" runat="server" MaxLength="256" EmptyMessage="Achievement Tag" />
                    </div>

                   <div class="mb-2">
                        <insite:TextBox ID="UserName" runat="server" MaxLength="256" EmptyMessage="User Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="UserEmail" runat="server" MaxLength="256" EmptyMessage="User Email" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="EmployerGroupName" runat="server" MaxLength="100" EmptyMessage="Employer Group Name" />
                    </div>

                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />

                </div>

                <div class="col-6">

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CredentialStatus" EnableTranslation="true" EmptyMessage="Credential Status">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Valid" Value="Valid" />
                                <insite:ComboBoxOption Text="Pending" Value="Pending" />
                                <insite:ComboBoxOption Text="Expired" Value="Expired" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="GrantedSince" runat="server" EmptyMessage="Granted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="GrantedBefore" runat="server" EmptyMessage="Granted Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="ExpirySince" runat="server" EmptyMessage="Expires Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="ExpiryBefore" runat="server" EmptyMessage="Expires Before" />
                    </div>


                </div>
            </div>
        </div>
    </div>

    <div class="col-3">
        <div class="mb-2">
            <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>

</div>