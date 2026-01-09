<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.CMDS.Portal.Achievements.Credentials.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./SearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmployeeCertificateGrid.ascx" TagName="EmployeeCertificateGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="CriteriaSection" Icon="fas fa-filter" Title="Criteria">

            <div class="row">
                <div class="col-8">
                    <div id="toolbox" class="toolbox-section">
                        <h4>Criteria</h4>

                        <div class="row">
                            <div class="col-6">
                                <div class="mb-2">
                                    <cmds:AchievementTypeSelector runat="server" ID="AchievementType" EmptyMessage="Achievement Type" NullText="" />
                                </div>
                                <div class="mb-2">
                                    <insite:TextBox runat="server" ID="AchievementTitle" EmptyMessage="Achievement Title" />
                                </div>
                                <div class="mb-2">
                                    <insite:ComboBox runat="server" ID="ProgressionStatus" EmptyMessage="Progression Status">
                                        <Items>
                                            <insite:ComboBoxOption />
                                            <insite:ComboBoxOption Value="Completed" Text="Completed" />
                                            <insite:ComboBoxOption Value="Not Completed" Text="Not Completed" />
                                            <insite:ComboBoxOption Value="Expired" Text="Expired" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                                <div class="mb-2">
                                    <insite:FilterButton ID="SearchButton" runat="server" />
                                    <insite:ClearButton ID="ClearButton" runat="server" />
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector runat="server" ID="CompletionDateSince" EmptyMessage="Completion Date &ge;" />
                                </div>
                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector runat="server" ID="CompletionDateBefore" EmptyMessage="Completion Date &lt;" />
                                </div>
                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector runat="server" ID="ExpirationDateSince" EmptyMessage="Expiration Date &ge;" />
                                </div>
                                <div class="mb-2">
                                    <insite:DateTimeOffsetSelector runat="server" ID="ExpirationDateBefore" EmptyMessage="Expiration Date &lt;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <h4>Settings</h4>
                        <insite:ComboBox runat="server" ID="GroupBy">
                            <Items>
                                <insite:ComboBoxOption Value="Type" Text="Group by Type" Selected="true" />
                                <insite:ComboBoxOption Value="TypeAndCategory" Text="Group by Type and Category" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AchievementsTab" Icon="fas fa-users-class" Title="CMDS Achievements" Visible="false">
            <h2 class="h3 mb-3">
                CMDS Achievements
            </h2>
            <uc:SearchResults runat="server" ID="AchievementsGrid" IsCMDSTraining="true" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ExperiencesTab" Icon="fas fa-users-class" Title="Other Achievements" Visible="false">
            <h2 class="h3 mb-3">
                Other Achievements
            </h2>
            <uc:SearchResults runat="server" ID="ExperiencesGrid" IsCMDSTraining="false" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CertificatesTab" Icon="fas fa-file-certificate" Title="Other Certificates" Visible="false">
            <h2 runat="server" id="CertificatesPanelTitle" class="h3 mb-3">
                Other Certificates
            </h2>
            <uc:EmployeeCertificateGrid ID="CertificatesGrid" runat="server" />
        </insite:NavItem>
    </insite:Nav>
</asp:Content>
